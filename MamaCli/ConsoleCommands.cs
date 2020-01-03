using System;
using System.Reflection;
using Monads;
using static MamaCli.CliStart;

namespace MamaCli
{
    public class ConsoleCommands
    {
        [ConsoleCommand(ConsoleKey.H, "help")]
        static void Help()
        {
            ClearAllButHeader();
            foreach (var method in Methods)
                method.GetCustomAttributes<ConsoleCommandAttribute>().FirstAsOptional()
                    .FromOptional(attr => DisplayText($"{attr.Key} — {attr.HelpText}\n"));
        }

        [ConsoleCommand(ConsoleKey.Q, "quit")]
        static void Quit()
        {
            Environment.Exit(0);
        }

        [ConsoleCommand(ConsoleKey.S, "get moma status")]
        static void Status()
        {
            ClearAllButHeader();
            DisplayText(SendMessage("status -w " + Console.WindowWidth));
        }

        [ConsoleCommand(ConsoleKey.M, "get moma help")]
        static void MomaHelp()
        {
            ClearAllButHeader();
            DisplayText(SendMessage("--help"));
        }
    }
}