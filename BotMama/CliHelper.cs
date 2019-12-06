using System;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CliWrap.Models;
using Monads;

namespace BotMama
{
    public static partial class Moma
    {
        private static async Task CloneRepo(string giturl, string dirname, string branch)
        {
            var result = await RunCommand("git", $"clone {giturl} -b {branch} --single-branch {dirname}");
            Log(result.StandardOutput);
        }

        private static async Task GitPull(string dirname)
        {
            var result = await RunCommand("git", $"-C {dirname} pull");
            Log(result.StandardOutput);
        }

        private static async Task DotnetAddReference(string project, string reference)
        {
            var result = await RunCommand("dotnet", $"add {project} reference  {reference}");
            Log(result.StandardOutput);
        }

        private static async Task DotnetRestore(string dirname)
        {
            var result = await RunCommand("dotnet", $"restore {dirname}");
            Log(result.StandardOutput);
        }

        private static async Task DotnetPublish(string dirname, string outputDir)
        {
            var    dotnetInfo = await RunCommand("dotnet", "--info");
            string systemRID  = null;

            if (dotnetInfo.ExitCode == 0)
            {
                var strings = dotnetInfo.StandardOutput.Split('\n');
                var rid = strings.FirstAsOptional(s => s.StartsWith("RID:"));

                rid.FromOptional(t => systemRID = t.Substring(5).Trim());
            }

            if (systemRID == null)
            {
                systemRID = "win10-x64";
                if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
                {
                    systemRID = "ubuntu.18.04-x64";
                }
            }

            var result = await RunCommand("dotnet", $"publish {dirname}  -o {outputDir} -r {systemRID}");
            Log(result.StandardOutput);
        }

        public static Task<ExecutionResult> RunCommand(string command, string args)
        {
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return CliWrap.Cli.Wrap(command)
                              .SetArguments(args)
                              .EnableExitCodeValidation(false)
                              .ExecuteAsync();
            }

            if ((RuntimeInformation.IsOSPlatform(OSPlatform.Windows)))
            {
                return CliWrap.Cli.Wrap("cmd.exe")
                              .SetArguments($"/c {command} {args}")
                              .EnableExitCodeValidation(false)
                              .ExecuteAsync();
            }

            throw new Exception("What the actual fuck you're using");
        }
    }
}