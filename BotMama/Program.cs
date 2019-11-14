using System;
using System.Globalization;
using System.IO;
using System.IO.Pipes;
using System.Threading;
using System.Threading.Tasks;
using BotMama.Cli;
using CommandLine;
using Microsoft.AspNetCore;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Options;

namespace BotMama
{
    public class Program
    {
        public static readonly NamedPipeServerStream Server =
        new NamedPipeServerStream("BotMamaPipe", PipeDirection.InOut);

        private static void StartServer()
        {
            Task.Run(() =>
            {
                while (true)
                {
                    Server.WaitForConnection();

                    var streamReader = new StreamReader(Server);
                    while (true)
                    {
                        var st = streamReader.ReadLine();
                        if (!string.IsNullOrEmpty(st))
                        {
                            Parser.Default.ParseArguments<StatusCli>(new[] {st}).WithParsed<StatusCli>(s => s.Run());

                            Console.WriteLine(st);
                        }

                        if (Server.IsConnected) continue;
                        Server.Disconnect();
                        break;
                    }
                }
            });
        }

        public static void Main(string[] args)
        {
            StartServer();
            Moma.LoadConfiguration("config.json");
            Moma.ValidateBots();
            Moma.StartBots();
            // CreateWebHostBuilder(args)
            //     .UseUrls("http://localhost:8444")
            //     .Build()
            //     .Run();
            Task.Run(() =>
            {
                while (true)
                {
                    Thread.Sleep(10000);
                }
            });
        }

        public static IWebHostBuilder CreateWebHostBuilder(string[] args)
        {
            return WebHost.CreateDefaultBuilder(args)
                          .UseStartup<Startup>();
        }
    }
}