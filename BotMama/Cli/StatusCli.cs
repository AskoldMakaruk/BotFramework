using System;
using System.Linq;
using BotFramework.Bot;
using CommandLine;
using Newtonsoft.Json;

namespace BotMama.Cli
{
    [Verb("status")]
    internal class StatusCli : CliAnswer
    {
        [Option('w', Required = true, Default = 0, Separator = ' ', HelpText = "ConsoleWidth")]
        public int ConsoleWidth { get; set; }

        protected override string Answer(string[] args)
        {
            var clients = Moma.Clients;
            Console.WriteLine(ConsoleWidth);
            return $"Total clients: {clients.Count}     Active: {clients.Count(c => c.Status == ClientStatus.Running)}";
        }
    }

    [Verb("list")]
    internal class ListBots : CliAnswer
    {
        protected override string Answer(string[] args)
        {
            var clients = Moma.Clients;
            return JsonConvert.SerializeObject(clients);
        }
    }
}