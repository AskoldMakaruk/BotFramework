﻿using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Reflection;
using BotFramework.Commands;
using BotFramework.Queries;
using Monad;
using Newtonsoft.Json;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Bot
{
    public delegate void Log(Client sender, string value);

    public abstract partial class Client
    {
        private string _workingdir;

        public string WorkingDir { get => _workingdir; set => _workingdir = value ?? Directory.GetCurrentDirectory(); }

        public string Name { get; set; }
        public ClientStatus Status { get; set; }

        protected TelegramBotClient Bot { get; set; }
        protected Dictionary<Func<CallbackQuery, bool>, Query> Queries { get; set; }
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

        protected abstract string Token { get; }
        protected bool UseWebhook { get; set; } = false;

        //todo maybe we should load configuration from the working dir
        protected Client()
        {
            Bot = new TelegramBotClient(Token);

            NextCommands = new Dictionary<long, EitherStrict<ICommand, IEnumerable<IOneOfMany>> ? > ();
        }

        protected void IDontCareJustMakeItWork(Assembly assembly)
        {
            Bot.OnMessage += OnMessageRecieved;
            Bot.OnCallbackQuery += OnQueryReceived;

            if (!UseWebhook)
            {
                Bot.StartReceiving();
                Bot.DeleteWebhookAsync();
            }

            StaticCommands = LoadTypeFromAssembly<IStaticCommand>(assembly);
            Queries = LoadTypeFromAssembly<Query>(assembly)
                .ToDictionary(x => new Func<CallbackQuery, bool>(x.IsSuitable), x => x);
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

        public async void HandleQuery(CallbackQuery query)
        {
            var func = Queries.Keys.FirstOrDefault(s => s.Invoke(query));
            var command = func != null ? Queries[func] : default;
            if (command == null)
            {
                Write("Query is null.");
                return;
            }

            Write($"Command: {command}");

            await SendResponse(command.Execute(query));
        }

        //it`s already private no need to make it readonly
        private static Dictionary<long, EitherStrict<ICommand, IEnumerable<IOneOfMany>> ? > NextCommands { get; set; }

        public async void HandleMessage(Message message)
        {
            if (!NextCommands.ContainsKey(message.Chat.Id))
                NextCommands.Add(message.Chat.Id, null);
            var nextPossible = NextCommands[message.Chat.Id];

            ICommand command = null;
            try
            {
                if (nextPossible.HasValue)
                    if ((nextPossible?.IsRight ?? false) || (nextPossible?.IsLeft ?? false))
                    {
                        command = nextPossible.Value.Match(
                                //todo right is null plis fix
                                right => right.Where(t => t.Suitable(message)),
                                left => Enumerable.Repeat(left, 1))
                            .FirstOrDefault();
                    }
            }
            catch
            {
                /*I guess someone has to fix this*/
            }

            if (command == null)
            {
                command = StaticCommands.FirstOrDefault(i => i.Suitable(message));
            }

            if (command == null)
            {
                await SendTextMessage(message.From.Id, "Internal server error: 504");
                return;
            }

            var response = command.Execute(message, this);
            if (response.NextPossible.HasValue)
                NextCommands[message.Chat.Id] = response.NextPossible;
            await SendResponse(response);
        }

        public virtual void OnMessageRecieved(object sender, MessageEventArgs e)
        {
            Write(DateTime.Now.ToShortTimeString() + " " + e.Message.From.Username + ": " + e.Message.Text);
            try
            {
                HandleMessage(e.Message);
            }
            catch (Exception ex)
            {
                Write(ex.ToString());
            }
        }

        public virtual void OnQueryReceived(object sender, CallbackQueryEventArgs e)
        {
            Write(
                $"{DateTime.Now.ToShortTimeString()} {e.CallbackQuery.From.Username}: {e.CallbackQuery.Data}");
            try
            {
                HandleQuery(e.CallbackQuery);
            }
            catch (Exception ex)
            {
                Write(ex.ToString());
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

        public void Write(string message) => OnLog?.Invoke(this, message);

        public event Log OnLog;
    }
}