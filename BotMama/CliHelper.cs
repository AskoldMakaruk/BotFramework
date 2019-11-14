using System;
using System.Runtime.InteropServices;
using System.Threading.Tasks;
using CliWrap.Models;

namespace BotMama
{
    public static partial class Moma
    {
        private static async Task CloneRepo(string giturl, string dirname, string branch)
        {
            var result = await RunCommand("git", $"clone {giturl} -b {branch} --single-branch {dirname}");
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
            //todo this from configuration or better from dotnet --info command
            var r = "win10-x64";
            if (RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                r = "ubuntu.18.04-x64";
            }
            var result = await RunCommand("dotnet", $"publish {dirname}  -o {outputDir} -r {r}");
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