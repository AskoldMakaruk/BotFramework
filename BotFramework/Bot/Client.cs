using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Commands;
using Newtonsoft.Json;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Bot
{
    public class Client
    {
        public    string       Name   { get; set; }
        public    ClientStatus Status { get; set; }
        protected ILogger      Logger { get; set; }

        protected TelegramBotClient Bot { get; set; }

        protected IEnumerable<IStaticCommand> StaticCommands { get; set; }

        protected static IEnumerable<T> LoadTypeFromAssembly<T>(Assembly assembly)
        {
            return assembly
                   .GetTypes()
                   .Where(t => (t.IsSubclassOf(typeof(T)) || t.GetInterfaces().Contains(typeof(T))) && !t.IsAbstract)
                   .Select(Activator.CreateInstance)
                   .Cast<T>()
                   .Where(c => c != null);
        }

        protected string Token      { get; }
        protected bool   UseWebhook { get; set; }

        protected internal Client(BotConfiguration configuration)
        {
            Token      = configuration.Token;
            UseWebhook = configuration.Webhook;
            Logger     = configuration.Logger;

            Bot          = new TelegramBotClient(Token);
            NextCommands = new Dictionary<long, IEnumerable<ICommand>>();

            var assembly = configuration.Assembly;
            Logger.Debug("Loading static commands...");

            StaticCommands = LoadTypeFromAssembly<IStaticCommand>(assembly);

            Logger.Debug("Loaded {StaticCommandsCount} commands.", StaticCommands.Count());
        }

        public void Run()
        {
            Logger.Information("Starting bot...");
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
                    from     = update.Message.From.Id;
                    fromName = update.Message.From.Username;
                    contents = update.Message.Text;
                    Logger.Debug("{UpdateType} {MessageType} | {From}: {Contents}", update.Type, update.Message.Type, fromName,
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

        public async void HandleUpdate(Update update)
        {
            var from = GetIdFromUpdate(update);

            if (!NextCommands.ContainsKey(from))
            {
                NextCommands.Add(from, StaticCommands);
            }

            var nextPossible = NextCommands[from];

            try
            {
                var suitable = nextPossible.Where(t => t.Suitable(update)).ToList();
                Logger.Debug("Suitable commands: {SuitableCommands}", string.Join(", ", suitable.Select(s => s.GetType().Name)));
                foreach (var response in suitable.Select(t => t.Execute(update, this)))
                {
                    if (!response.UsePreviousCommands)
                    {
                        NextCommands[from] = response.NextPossible;
                    }

                    if (response.UseStaticCommands)
                    {
                        var newPossible = NextCommands[from].ToList();
                        newPossible.AddRange(StaticCommands);
                        NextCommands[from] = newPossible;
                    }

                    foreach (var message in response.Responses)
                        await message.Send(Bot);
                }
            }
            catch (Exception e)
            {
                Logger.Error(e, "Error handling command.");
            }
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

        public async Task<Telegram.Bot.Types.File> GetInfoAndDownloadFileAsync(string documentFileId, MemoryStream ms)
        {
            return await Bot.GetInfoAndDownloadFileAsync(documentFileId, ms);
        }
    }
}