using System;
using System.Linq;
using System.Reflection;
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
            {
                var attr = method.GetCustomAttributes<ConsoleCommandAttribute>().FirstOrDefault();
                if (attr == null) continue;
               
                DisplayText($"{attr.Key} — {attr.HelpText}\n");
            }
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
            DisplayText(SendMessage("status"));
        }
    }
}