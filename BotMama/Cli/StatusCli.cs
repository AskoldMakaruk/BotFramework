using System.Linq;
using System.Text;
using System.Text.Json.Serialization;
using BotFramework.Bot;
using CommandLine;
using Newtonsoft.Json;

namespace BotMama.Cli
{
    [Verb("status")]
    internal class StatusCli : CliAnswer
    {
        protected override string Answer(string[] args)
        {
            var clients = Moma.Clients;
            return $"Total clients: {clients.Count}\n\nActive: {clients.Count(c => c.Status == ClientStatus.Running)}";
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