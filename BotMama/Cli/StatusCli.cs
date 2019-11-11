using System.Linq;
using BotFramework.Bot;
using CommandLine;

namespace BotMama.Cli
{
    [Verb("status")]
    class StatusCli : CliAnswer
    {
        protected override string Answer(string[] args)
        {
            var clients = Moma.Clients;
            return $"Total clients: {clients.Count}\n\nActive: {clients.Count(c => c.Status == ClientStatus.Running)}";
        }
    }
}