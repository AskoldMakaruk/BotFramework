using System;
using System.Collections.Concurrent;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Helpers;
using Serilog;
using Serilog.Context;
using Telegram.Bot;
using Telegram.Bot.Types;

#nullable enable
namespace BotFramework.Handlers
{
    public class DictionaryInMemoryHandler : IUpdateHandler
    {
        private readonly TelegramBotClient BotClient;

        private readonly CommandSearcher CommandSearcher;
        private readonly ILogger         _logger;

        public DictionaryInMemoryHandler(HandlerConfiguration configuration)
        {
            _logger = configuration.Logger;
            _logger.Information($"Using {nameof(DictionaryInMemoryHandler)} handler");
            Clients         = new ConcurrentDictionary<long, Client>();
            BotClient       = configuration.BotClient;
            CommandSearcher = new CommandSearcher(configuration.StaticCommands, configuration.CommandInjector);
        }

        private ConcurrentDictionary<long, Client> Clients { get; }

        public void Handle(Update update)
        {
            Task.Run(() =>
            {
                try
                {
                    var parsedUpdate = update.GetInfoFromUpdate();

                    var fromNullanble = parsedUpdate.From?.Id ?? parsedUpdate.Chat?.Id;
                    if (fromNullanble == null)
                    {
                        //todo handle messages with no user idk
                        CommandSearcher.FindSuitableFirst(update)?.Execute(new Client(BotClient, default));
                        CommandSearcher.FindSuitableLast(update)?.Execute(new Client(BotClient,  default));
                        _logger.Information("User Id cannot be parsed.");
                        return;
                    }

                    var from = (long) fromNullanble;

                    var client = Clients.GetOrAdd(from, from => new Client(BotClient, from));
                    lock (client)
                    {
                        var currentCommand = CommandSearcher.FindSuitableFirst(update);
                        if (currentCommand == null)
                        {
                            if (client.CurrentTask == null || client.CurrentTask.IsCompleted)
                            {
                                client.CurrentBasicBotTask = null;
                                client.CurrentTask         = CommandSearcher.FindSuitableLast(update)?.Execute(client);
                            }
                        }
                        else
                        {
                            client.CurrentBasicBotTask = null;
                            client.CurrentTask         = currentCommand.Execute(client);
                        }

                        var task = client.CurrentTask;

                        using (LogContext.PushProperty("UpdateType", update.Type))
                        using (LogContext.PushProperty("MessageType", update.Message?.Type))
                        using (LogContext.PushProperty("From", parsedUpdate.FromName))
                        using (LogContext.PushProperty("Contents", parsedUpdate.Contents))
                        {
                            _logger.Debug("{UpdateType} {MessageType} | {From} {Contents}");
                        }

                        if (task == null)
                        {
                            _logger.Debug("No command matched update.");
                            return;
                        }

                        task.ContinueWith(_ =>
                        {
                            if (task.Exception != null)
                            {
                                _logger.Error(task.Exception, "Error handling command");
                            }

                            client.CurrentTask = task.Result.NextCommand?.Execute(client);
                        });
                        client.HandleUpdate(update);
                    }
                }
                catch (Exception ex)
                {
                    _logger.Error(ex, "Error handling command.");
                }
            });
        }
    }
}