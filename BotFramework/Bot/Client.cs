using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Commands;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Bot
{
    public delegate void Log(Client sender, string value);

    public class Client
    {
        private string _workingdir;
        public  string WorkingDir { get => _workingdir; set => _workingdir = value ?? Directory.GetCurrentDirectory(); }

        public string       Name   { get; set; }
        public ClientStatus Status { get; set; }

        protected TelegramBotClient           Bot            { get; set; }
        protected IEnumerable<IStaticCommand> StaticCommands { get; set; }

        protected IEnumerable<T> LoadTypeFromAssembly<T>(Assembly assembly)
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
            Token      =  configuration.Token;
            UseWebhook =  configuration.Webhook;
            OnLog      += configuration.OnLog;

            Bot          = new TelegramBotClient(Token);
            NextCommands = new Dictionary<long, IEnumerable<ICommand>>();

            var assembly = configuration.Assembly;
            StaticCommands = LoadTypeFromAssembly<IStaticCommand>(assembly);
        }

        public void Run()
        {
            if (!UseWebhook)
            {
                Bot.StartReceiving();
                Bot.DeleteWebhookAsync();
            }

            Bot.OnUpdate += OnUpdateReceived;

            new ManualResetEvent(false).WaitOne();
        }

        public void StartReceiving()
        {
            Status = ClientStatus.Running;
            Bot.StartReceiving();
        }

        public void StopReceiving()
        {
            Status = ClientStatus.Stoped;
            Bot.StopReceiving();
        }

        private static Dictionary<long, IEnumerable<ICommand>> NextCommands { get; set; }

        public void HandleUpdate(Update update)
        {
            long from;
            switch (update.Type)
            {
                case UpdateType.Message:
                    from = update.Message.From.Id;
                    Write(update.Message.From.Username + ": " + update.Message.Text);
                    break;
                case UpdateType.InlineQuery:
                    from = update.InlineQuery.From.Id;
                    break;
                case UpdateType.ChosenInlineResult:
                    from = update.ChosenInlineResult.From.Id;
                    break;
                case UpdateType.CallbackQuery:
                    from = update.CallbackQuery.From.Id;
                    break;
                case UpdateType.EditedMessage:
                    from = update.EditedMessage.From.Id;
                    break;
                case UpdateType.ChannelPost:
                    from = update.ChannelPost.From.Id;
                    break;
                case UpdateType.EditedChannelPost:
                    from = update.EditedChannelPost.From.Id;
                    break;
                case UpdateType.ShippingQuery:
                    from = update.ShippingQuery.From.Id;
                    break;
                case UpdateType.PreCheckoutQuery:
                    from = update.PreCheckoutQuery.From.Id;
                    break;
                default:
                    return;
            }

            if (!NextCommands.ContainsKey(from))
            {
                NextCommands.Add(from, StaticCommands);
            }

            var nextPossible = NextCommands[from];

            try
            {
                nextPossible
                .Where(t => t.Suitable(update))
                .Select(t => t.Run(update, this))
                .ToList()
                .ForEach(async response =>
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
                });
            }
            catch (Exception e)
            {
                Write(e.Message);
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
            //Write($"{DateTime.Now.ToShortTimeString()} {update.Message.From.Username}: {update.Message.Text}");
            try
            {
                HandleUpdate(update);
            }
            catch (Exception ex)
            {
                Write(ex.ToString());
            }
        }

        public async Task GetInfoAndDownloadFileAsync(string documentFileId, MemoryStream ms)
        {
            await Bot.GetInfoAndDownloadFileAsync(documentFileId, ms);
        }

        public void      Write(string message) => OnLog?.Invoke(this, message);
        public event Log OnLog;
    }
}