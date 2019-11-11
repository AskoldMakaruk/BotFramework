using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Reflection;
using BotFramework.Commands;
using BotFramework.Queries;
using Monad;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Newtonsoft;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Bot
{
    public partial class Client : IClient
    {
        public string       Name   { get; set; }
        public ClientStatus Status { get; set; }

        public static Dictionary<long, EitherStrict<ICommand, IEnumerable<IOneOfMany>>?> nextCommands;

        private   TelegramBotClient                                  Bot            { get; set; }
        protected Dictionary<Func<CallbackQuery, long, bool>, Query> Queries        { get; set; }
        protected IEnumerable<StaticCommand>                         StaticCommands { get; set; }

        public void Configure(Configuration configuration)
        {
            var assembly = configuration.Assembly;
            var baseType = typeof(Query);

            Queries = assembly
                      .GetTypes()
                      .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract)
                      .Select(c => Activator.CreateInstance(c) as Query)
                      .Where(c => c != null)
                      .ToDictionary(x => new Func<CallbackQuery, long, bool>(x.IsSuitable), x => x);

            Bot                 =  new TelegramBotClient(configuration.Token);
            Bot.OnMessage       += OnMessageRecieved;
            Bot.OnCallbackQuery += OnQueryReceived;

            if (!configuration.Webhook)
            {
                Bot.StartReceiving();
                Bot.DeleteWebhookAsync();
            }

            Bot.SendTextMessageAsync(249258727, "Hi");
        }

        protected Query GetQuery(CallbackQuery message, long account)
        {
            var func = Queries.Keys.FirstOrDefault(s => s.Invoke(message, account));
            return func != null ? Queries[func] : default;
        }

        public async void HandleQuery(CallbackQuery query)
        {
            try
            {
                var command = GetQuery(query, query.From.Id);

                InvokeLog($"Command: {command}");

                await SendTextMessageAsync(command.Execute(query, query.From.Id));
            }
            catch (Exception e)
            {
                InvokeLog(e.ToString());
            }
        }

        public async void HandleMessage(Message message)
        {
            var nextPossible = nextCommands[message.Chat.Id];
            var toExecute = nextPossible.HasValue
                            ? nextPossible.Value.Match(
                                right => right.Where(t => t.Suitable(message)),
                                left => Enumerable.Repeat(left, 1))
                            : StaticCommands;
            var responses = toExecute.Select(t => t.Execute(message, this));
            foreach (var response in responses)
            {
                if (response.NextPossible.HasValue)
                    nextCommands[message.Chat.Id] = response.NextPossible;
                await SendTextMessageAsync(response);
            }
        }

        public void OnMessageRecieved(object sender, MessageEventArgs e)
        {
            InvokeLog(DateTime.Now.ToShortTimeString() + " " + e.Message.From.Username + ": " + e.Message.Text);
            try
            {
                HandleMessage(e.Message);
            }
            catch (Exception ex)
            {
                InvokeLog(ex.ToString());
            }
        }

        public void OnQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            InvokeLog(
                $"{DateTime.Now.ToShortTimeString()} {e.CallbackQuery.From.Username}: {e.CallbackQuery.Data}");
            try
            {
                HandleQuery(e.CallbackQuery);
            }
            catch (Exception ex)
            {
                InvokeLog(ex.ToString());
            }
        }

        public void HandleUpdate(string json)
        {
            var update = JsonConvert.DeserializeObject<Update>(json);
            switch (update.Type)
            {
                case UpdateType.CallbackQuery:
                    HandleQuery(update.CallbackQuery);
                    break;
                case UpdateType.Message:
                    HandleMessage(update.Message);
                    break;
            }
        }

        private void InvokeLog(string message) => OnLog?.Invoke(this, message);

        public event Log OnLog;
    }
}