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

        private static async Task DotnetBuild(string dirname)
        {
            var result = await RunCommand("dotnet", $"build {dirname}");
            Log(result.StandardOutput);
        }

        public static Task<ExecutionResult> RunCommand(string command, string args)
        {
            if (System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Linux))
            {
                return CliWrap.Cli.Wrap(command)
                    .SetArguments(args)
                    .EnableExitCodeValidation(false)
                    .ExecuteAsync();
            }
            else if ((System.Runtime.InteropServices.RuntimeInformation.IsOSPlatform(OSPlatform.Windows)))
            {
                return CliWrap.Cli.Wrap("cmd.exe")
                    .SetArguments($"/c {command} {args}")
                    .EnableExitCodeValidation(false)
                    .ExecuteAsync();
            }
            else throw new Exception("What the actual fuck you're using");
        }

    }
}