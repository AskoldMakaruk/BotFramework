using BotFramework.Commands;
using Newtonsoft.Json;
using Serilog;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Responses;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Bot
{
    public class Client
    {
        private class GetOnlyClient : TelegramBotClient, IGetOnlyClient
        {
            public GetOnlyClient(string token, HttpClient httpClient = null) : base(token, httpClient) { }
        }

        protected ILogger Logger { get; set; }

        protected virtual IGetOnlyClient    GetOnlyBot => _bot;
        protected virtual TelegramBotClient Bot        => _bot;
        private           GetOnlyClient     _bot       { get; set; }

        protected List<ICommand> StaticCommands  { get; set; }
        protected List<ICommand> OnStartCommands { get; set; }

        protected string Token      { get; }
        protected bool   UseWebhook { get; set; }

        public Client(BotConfiguration configuration)
        {
            Token      = configuration.Token;
            UseWebhook = configuration.Webhook;
            Logger     = configuration.Logger;

            _bot         = new GetOnlyClient(Token);
            NextCommands = new Dictionary<long, IEnumerable<ICommand>>();

            Logger.Debug("Loading static commands...");
            StaticCommands  = configuration.Commands;
            OnStartCommands = configuration.StartCommands;
            Logger.Debug("Loaded {StaticCommandsCount} commands.", StaticCommands.Count);
            Logger.Debug("{StaticCommands}",
                string.Join(',', StaticCommands.Select(c => c.GetType().Name)));
        }

        public void Run()
        {
            Logger.Information("Starting bot...");
            var me = Bot.GetMeAsync().Result;
            Logger.Information("Name: {BotFirstName} UserName: @{BotName}", me.FirstName, me.Username);
            if (!UseWebhook)
            {
                Bot.StartReceiving();
                Bot.DeleteWebhookAsync();
            }

            Bot.OnUpdate += OnUpdateReceived;

            new ManualResetEvent(false).WaitOne();
        }

        private static Dictionary<long, IEnumerable<ICommand>> NextCommands { get; set; }

        private long GetIdFromUpdate(Update update)
        {
            long   from;
            string fromName, contents;
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
                            Logger.Debug("{UpdateType}.{MessageType} | {From} {Caption}", update.Type,
                                update.Message.Type, fromName, message.Caption);
                            return from;
                        case MessageType.Poll:
                            contents = message.Poll.Question;
                            break;
                        case MessageType.ChatTitleChanged:
                            contents = message.Chat.Title;
                            break;
                        default:
                            Logger.Debug("{UpdateType}.{MessageType} | {From}", update.Type, message.Type, fromName);
                            return from;
                    }

                    Logger.Debug("{UpdateType}.{MessageType} | {From}: {Contents}", update.Type, message.Type, fromName,
                        contents);
                    return from;
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

            Logger.Debug("{UpdateType} | {From}: {Contents}", update.Type, fromName, contents);
            return from;
        }

        public async Task HandleUpdate(Update update)
        {
            var from = GetIdFromUpdate(update);

            if (!NextCommands.ContainsKey(from))
            {
                NextCommands.Add(from, OnStartCommands.Concat(StaticCommands));
            }

            var nextPossible = NextCommands[from].ToList();

            try
            {
                var suitable = nextPossible.Where(t => t.Suitable(update)).ToList();
                Logger.Debug("Suitable commands: {SuitableCommands}", string.Join(", ", suitable.Select(s => s.GetType().Name)));
                var newPossible = new HashSet<ICommand>(StaticCommands);
                foreach (var response in suitable.Select(t => t.Execute(update, GetOnlyBot)))
                {
                    if (response.UsePreviousCommands)
                    {
                        newPossible.UnionWith(nextPossible);
                    }

                    newPossible.UnionWith(response.NextPossible);

                    await SendMessages(response.Responses);
                }

                NextCommands[from] = newPossible;
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error handling command.");
            }
        }

        public virtual async Task SendMessages(IEnumerable<IResponseMessage> responses)
        {
            foreach (var message in responses)
                await message.Send(Bot);
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