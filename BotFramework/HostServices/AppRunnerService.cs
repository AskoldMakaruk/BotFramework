using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.Hosting;
using Telegram.Bot;

namespace BotFramework.HostServices
{
    public class AppRunnerService : IHostedService
    {
        private readonly ITelegramBotClient _client;

        public AppRunnerService(UpdateDelegate app, ITelegramBotClient client)
        {
            _client          =  client;
            _client.OnUpdate += (sender, args) => app(args.Update);
        }

        public Task StartAsync(CancellationToken cancellationToken)
        {
            _client.StartReceiving(cancellationToken: cancellationToken);
            return Task.CompletedTask;
        }

        public Task StopAsync(CancellationToken cancellationToken)
        {
            _client.StopReceiving();
            return Task.CompletedTask;
        }
    }
}