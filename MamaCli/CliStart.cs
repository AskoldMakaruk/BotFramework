using System;
using System.Collections.Generic;
using System.Drawing;
using System.Drawing.Text;
using System.IO;
using System.IO.Pipes;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using static System.Console;

namespace MamaCli
{
    internal static class CliStart
    {
        public enum TextColor
        {
            WhiteOnBlack,
            BlackOnGray
        }

        private static int HeaderHeight { get; set; }

        public static List<MethodInfo> Methods { get; set; }

        public static void WriteHeader()
        {
            ForegroundColor = ConsoleColor.Black;
            BackgroundColor = ConsoleColor.Gray;
            WriteLine("BotMama\n");
            HeaderHeight = 1;
        }

        public static void ClearAllButHeader()
        {
            BackgroundColor = ConsoleColor.Black;
            ForegroundColor = ConsoleColor.White;
            CursorLeft      = 0;
            CursorTop       = HeaderHeight + 1;
            var line = new string(' ', WindowWidth);
            for (var i = CursorTop; i < WindowHeight - 1; i++)
            {
                WriteLine(line);
            }

            CursorLeft = 0;
            CursorTop  = HeaderHeight + 1;
        }

        public static void DisplayText(string text, TextColor color = TextColor.WhiteOnBlack)
        {
            var (back, consoleColor) = GetColors(color);
            BackgroundColor          = back;
            ForegroundColor          = consoleColor;
            Write(text);
        }

        public static string SendMessage(string message)
        {
            using var client = new NamedPipeClientStream(".", "BotMamaPipe", PipeDirection.InOut, PipeOptions.Asynchronous);
            try
            {
                client.Connect(2000);
            }
            catch (Exception e)
            {
                return "Can't connect to moma";
            }

            var buffer = Encoding.UTF8.GetBytes(message + "\n");
            client.WriteAsync(buffer, 0, buffer.Length);
            client.Flush();

            var streamReader = new StreamReader(client);
            var builder      = new StringBuilder();
            while (true)
            {
                var st = streamReader.ReadLine();
                if (st == "end")
                {
                    client.Dispose();
                    return builder.ToString();
                }

                builder.Append(st);
            }
        }

        private static void Main(string[] args)
        {
            Methods = typeof(ConsoleCommands).GetMethods(BindingFlags.Static | BindingFlags.Instance | BindingFlags.Public | BindingFlags.NonPublic).Where(m => m.IsDefined(typeof(ConsoleCommandAttribute))).ToList();

            WriteHeader();
            while (true)
            {
                var key = ReadKey(true);
                Title = key.KeyChar.ToString();
                var method = Methods.FirstOrDefault(m => m.GetCustomAttribute<ConsoleCommandAttribute>().Key == key.Key);


                using var invokeTask   = Task.Run(() => method?.Invoke(null, null));
                using var resetCliTask = Task.Run(() => key = ReadKey(true));
                var       tasks        = new Task[] {invokeTask, resetCliTask};

                Task.WhenAny(tasks);
            }
        }
        public static (ConsoleColor back, ConsoleColor text) GetColors(TextColor input)
        {
            return input switch
                   {
                   TextColor.WhiteOnBlack => (ConsoleColor.Black, ConsoleColor.White),
                   TextColor.BlackOnGray => (ConsoleColor.Gray, ConsoleColor.Black),
                   };
        }
    }
}