using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using BotFramework.Commands;
using BotFramework.Queries;
using Microsoft.EntityFrameworkCore;
using Monad;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace BotFramework.Client
{
    public partial class Client
    {
        public static Dictionary<long, EitherStrict<ICommand, IEnumerable<IOneOfMany>>?> nextCommands;
        public Client(string token, Assembly assembly = null)
        {
            assembly = assembly ??
            //повертає ассебмлю з якої викликається конструктор
            Assembly.GetCallingAssembly();

            var baseType = typeof(Query);
            assembly = baseType.Assembly;

            Queries = assembly
                .GetTypes()
                .Where(t => t.IsSubclassOf(baseType) && !t.IsAbstract)
                .Select(c => Activator.CreateInstance(c) as Query)
                .Where(c => c != null)
                .ToDictionary(x => new Func<CallbackQuery, long, bool>(x.IsSuitable), x => x);

            Bot = new TelegramBotClient(token);
            Bot.OnMessage += OnMessageRecieved;
            Bot.OnCallbackQuery += OnQueryReceived;
            Bot.StartReceiving();
            Bot.SendTextMessageAsync(249258727, "Hi");
            Bot.DeleteWebhookAsync();
        }

        private TelegramBotClient Bot { get; }
        protected Dictionary<Func<CallbackQuery, long, bool>, Query> Queries { get; set; }
        protected IEnumerable<StaticCommand> StaticCommands { get; set; }

        //public async void OnUpdateReceived(object sender, UpdateEventArgs e)
        //{
        //    if (e.Update.Type == UpdateType.ChannelPost)
        //    {
        //        var message    = e.Update.ChannelPost;
        //        var controller = new TelegramController();
        //        controller.Start();
        //        if (message.Document != null || message.Photo != null)
        //        {
        //            var meme = await ImageDownloader.DownloadFromMessage(message, this,
        //                           new ChatId() {ChatId = message.Chat.Id, UserName = message.Chat.Username, Id = -1},
        //                           controller);
        //            await EditMessageReplyMarkupAsync(message.Chat, message.MessageId,
        //                Command.AddMemeButton(controller.GetChannelLanguage(message.Chat.Id), controller, meme.Id,
        //                    controller.CountLikes(meme.Id)));
        //        }
        //    }
        //}

        public async void HandleQuery(CallbackQuery query)
        {
            try
            {
                var command = GetQuery(query, query.From.Id);

                Console.WriteLine($"Command: {command}");

                await SendTextMessageAsync(command.Execute(query, query.From.Id));
            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }
        }

        public async void HandleMessage(Message message)
        {
            var nextPossible = nextCommands[message.Chat.Id];
            var toExecute = nextPossible.HasValue
                ? nextPossible.Value.Match(
                    right => right.Where(t => t.Suitable(message, message.Chat.Id)),
                    left => Enumerable.Repeat(left, 1))
                : StaticCommands;
            var responses = toExecute.Select(t => t.Execute(message, this, message.Chat.Id));
            foreach (var response in responses)
            {
                if (response.NextPossible.HasValue)
                    nextCommands[message.Chat.Id] = response.NextPossible;
                await SendTextMessageAsync(response);
            }
        }
        
        protected Query GetQuery(CallbackQuery message, long account)
        {
            var func = Queries.Keys.FirstOrDefault(s => s.Invoke(message, account));
            return func != null ? Queries[func] : default;
        }

        public void OnMessageRecieved(object sender, MessageEventArgs e)
        {
            Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + e.Message.From.Username + ": " + e.Message.Text);
            try
            {
                HandleMessage(e.Message);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }

        public void OnQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            Console.WriteLine(
                $"{DateTime.Now.ToShortTimeString()} {e.CallbackQuery.From.Username}: {e.CallbackQuery.Data}");
            try
            {
                HandleQuery(e.CallbackQuery);
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex);
            }
        }
    }
}