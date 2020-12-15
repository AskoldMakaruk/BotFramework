using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using BotFramework.Clients;
using BotFramework.Commands;
using BotFramework.Injectors;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Handlers
{
    public class DictionaryInMemoryHandler : IUpdateHandler
    {
        private TelegramBotClient BotClient;

        private CommandSearcher CommandSearcher;
        private ILogger         Logger;

        public DictionaryInMemoryHandler(HandlerConfiguration configuration)
        {
            Logger = configuration.Logger;
            Logger.Information($"Using {nameof(DictionaryInMemoryHandler)} handler");
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
                    var from = DefaultHandlerCreator.GetIdFromUpdate(update, Logger);

                    var client = Clients.GetOrAdd(from, from => new Client(BotClient, @from));
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
                        if (task == null) return;
                        task.ContinueWith(_ =>
                        {
                            if (task.Exception != null)
                            {
                                Logger.Error(task.Exception, "Error handling command");
                            }

                            client.CurrentTask = task.Result.NextCommand?.Execute(client);
                        });
                        client.HandleUpdate(update);
                    }
                }
                catch (Exception ex)
                {
                    Logger.Error(ex, "Error handling command.");
                }
            });
        }
    }
}