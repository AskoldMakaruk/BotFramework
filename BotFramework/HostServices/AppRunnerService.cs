using System;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;

namespace BotFramework.HostServices;

// todo handle obsolete update handling
public class AppRunnerService : IHostedService
{
    private readonly BotDelegate        _app;
    private readonly ITelegramBotClient _client;
    private readonly ILogger            _logger;

    public AppRunnerService(BotDelegate app, ITelegramBotClient client, ILogger<AppRunnerService> logger = null)
    {
        _app    = app;
        _client = client;
        _logger = logger;
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        var me = await _client.GetMeAsync(cancellationToken);
        _logger?.LogInformation("Started bot @{UserName}", me.Username);
        _client.StartReceiving(
            (client, update,    arg3) => _app(update),
            (client, exception, arg3) => { Console.WriteLine(exception); },
            cancellationToken: cancellationToken
        );
    }

    //todo idk how to do it
    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}