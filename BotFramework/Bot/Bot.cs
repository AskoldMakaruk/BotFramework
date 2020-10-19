using BotFramework.Commands;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.BotTask;
using BotFramework.Clients;
using BotFramework.Responses;
using BotFramework.Storage;
using Serilog.Context;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Bot
{
    public class Bot
    {
        protected ILogger Logger { get; set; }

        protected TelegramBotClient BotClient;

        protected List<(IStaticCommand, Type)> StaticCommands  { get; set; }
        protected List<(IStaticCommand, Type)> OnStartCommands { get; set; }
        protected IInjector                    CommandInjector { get; set; }

        protected string Token      { get; }
        protected bool   UseWebhook { get; set; }

        public Bot(BotConfiguration configuration)
        {
            Token           = configuration.Token;
            UseWebhook      = configuration.Webhook;
            Logger          = configuration.Logger;
            ClientStorage   = configuration.Storage;
            CommandInjector = configuration.Injector;

            BotClient = new TelegramBotClient(Token);

            Logger.Debug("Loading static commands...");
            StaticCommands  = configuration.StaticCommands.Select(t => (CommandInjector!.Create(t) as IStaticCommand, t)).ToList()!;
            OnStartCommands  = configuration.StartCommands.Select(t => (CommandInjector!.Create(t) as IStaticCommand, t)).ToList()!;
            Logger.Debug("Loaded {StaticCommandsCount} commands.", StaticCommands.Count);
            Logger.Debug("{StaticCommands}",
                string.Join(',', StaticCommands.Select(c => c.GetType().Name)));
        }

        public void Run()
        {
            Logger.Information("Starting bot...");
            var me = BotClient.GetMeAsync().Result;
            Logger.Information("Name: {BotFirstName} UserName: @{BotName}", me.FirstName, me.Username);
            if (!UseWebhook)
            {
                BotClient.StartReceiving();
                BotClient.DeleteWebhookAsync();
            }

            BotClient.OnUpdate += OnUpdateReceived;

            new ManualResetEvent(false).WaitOne();
        }

        private IClientStorage ClientStorage { get; set; }

        private long GetIdFromUpdate(Update update)
        {
            long   from;
            string fromName, contents = "";
            switch (update.Type)
            {
                case UpdateType.Message:
                    var message = update.Message;
                    from     = message.From.Id;
                    fromName = message.From.Username;
                    switch (update.Message.Type)
                    {
                        case MessageType.Text:
                            contents = message.Text;
                            break;
                        case MessageType.Sticker:
                            contents = message.Sticker.SetName;
                            break;
                        case MessageType.Photo:
                        case MessageType.Audio:
                        case MessageType.Video:
                        case MessageType.Document:
                            //Logger.Debug("{UpdateType}.{MessageType} | {From} {Caption}", update.Type, update.Message.Type, fromName, message.Caption);
                            //return from;
                            contents = message.Caption;
                            break;
                        case MessageType.Poll:
                            contents = message.Poll.Question;
                            break;
                        case MessageType.ChatTitleChanged:
                            contents = message.Chat.Title;
                            break;
                        case MessageType.Contact:
                            contents = $"{message.Contact.FirstName} {message.Contact.LastName} {message.Contact.PhoneNumber}";
                            break;
                        default:
                            //      Logger.Debug("{UpdateType}.{MessageType} | {From}", update.Type, message.Type, fromName);
                            //    return from;
                            break;
                    }

                    //Logger.Debug("{UpdateType}.{MessageType} | {From}: {Contents}", update.Type, message.Type, fromName, contents);
                    break;
                case UpdateType.InlineQuery:
                    from     = update.InlineQuery.From.Id;
                    fromName = update.InlineQuery.From.Username;
                    contents = update.InlineQuery.Query;
                    break;
                case UpdateType.ChosenInlineResult:
                    from     = update.ChosenInlineResult.From.Id;
                    fromName = update.ChosenInlineResult.From.Username;
                    contents = update.ChosenInlineResult.Query;
                    break;
                case UpdateType.CallbackQuery:
                    from     = update.CallbackQuery.From.Id;
                    fromName = update.CallbackQuery.From.Username;
                    contents = update.CallbackQuery.Data;
                    break;
                case UpdateType.EditedMessage:
                    from     = update.EditedMessage.From.Id;
                    fromName = update.EditedMessage.From.Username;
                    contents = update.EditedMessage.Text;
                    break;
                case UpdateType.ChannelPost:
                    from     = update.ChannelPost.From.Id;
                    fromName = update.ChannelPost.From.Username;
                    contents = update.ChannelPost.Text;
                    break;
                case UpdateType.EditedChannelPost:
                    from     = update.EditedChannelPost.From.Id;
                    fromName = update.EditedChannelPost.From.Username;
                    contents = update.EditedChannelPost.Text;
                    break;
                case UpdateType.ShippingQuery:
                    from     = update.ShippingQuery.From.Id;
                    fromName = update.ShippingQuery.From.Username;
                    contents = update.ShippingQuery.InvoicePayload;
                    break;
                case UpdateType.PreCheckoutQuery:
                    from     = update.PreCheckoutQuery.From.Id;
                    fromName = update.PreCheckoutQuery.From.Username;
                    contents = "";
                    break;
                default:
                    var ex = new NotImplementedException($"We don't support {update.Type} right now");
                    Logger.Error(ex, ex.Message);
                    throw ex;
            }

            using (LogContext.PushProperty("UpdateType", update.Type))
            using (LogContext.PushProperty("MessageType", update.Message?.Type))
            using (LogContext.PushProperty("From", fromName))
            using (LogContext.PushProperty("Contents", contents))
            {
                Logger.Debug("{UpdateType} {MessageType} | {From} {Contents}");
            }

            return from;
        }

        private Client InitClient(long id, Update update)
        {
            var currentCommand = OnStartCommands.Concat(StaticCommands)
                                                .Where(t => t.Item1.Suitable(update))
                                                .Select(t => CommandInjector.Create(t.Item2))
                                                .Cast<IStaticCommand>()
                                                .FirstOrDefault(t => t.Suitable(update));
            var client = new Client(BotClient, id);
            ClientStorage.SetClient(id, client);
            if (currentCommand != null)
                client.CurrentTask = currentCommand.Execute(client);
            return client;
        }

        private ICommand? TryFindPossible(Update update)
        {
            return StaticCommands.Where(t => t.Item1.Suitable(update))
                                 .Select(t => CommandInjector.Create(t.Item2))
                                 .Cast<IStaticCommand>()
                                 .FirstOrDefault();
        }

        private void SetOnCompleted(Client client)
        {
            var task = client.CurrentTask;
            if(task == null) return;
            task.OnCompleted = () =>
            {
                if (task.Exception != null)
                {
                    Logger.Error("Error handling command", task.Exception);
                }

                client.CurrentTask = task.Result.NextCommand?.Execute(client);
            };
        }

        public void HandleUpdate(Update? update)
        {
            if (update == null)
                return;

            var from = GetIdFromUpdate(update);

            var client = ClientStorage.GetClient(from);
            if (client == null)
            {
                client = InitClient(@from, update);
                SetOnCompleted(client);
            }
            if (client.CurrentTask == null)
            {
                client.CurrentTask = TryFindPossible(update)?.Execute(client);
                SetOnCompleted(client);
            }
            if (client.CurrentTask == null) return;
            client.HandleUpdate(update);
        }


        public void HandleUpdate(string json)
        {
            var update = JsonConvert.DeserializeObject<Update>(json);
            HandleUpdate(update);
        }

        public void OnUpdateReceived(object sender, UpdateEventArgs e)
        {
            var update = e.Update;
            try
            {
                HandleUpdate(update);
            }
            catch (Exception ex)
            {
                Logger.Error(ex, "Error handling command.");
            }
        }
    }
}