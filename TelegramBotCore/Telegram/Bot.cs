using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types.InputFiles;
using TelegramBotCore.DB;
using TelegramBotCore.Telegram.Commands;

namespace TelegramBotCore.Telegram
{
    public class Bot : TelegramBotClient
    {

        private static List<Command> _commandsList;
        public static List<Account> AccountsList;
        public static IReadOnlyList<Command> Commands => _commandsList.AsReadOnly();

        public Bot(string token) : base(token)
        {
            _commandsList = new List<Command>();
            AccountsList = new List<Account>();
            Type baseType = typeof(Command);
            IEnumerable<Type> botCommands =
                Assembly.GetAssembly(baseType).GetTypes().
                    Where(types => types.BaseType != null &&
                                   (types.BaseType.BaseType != null &&
                                    (types.BaseType != null &&
                                     (!types.IsAbstract && (types.BaseType == baseType || types.BaseType.BaseType == baseType || types.BaseType.BaseType.BaseType == baseType)))));

            foreach (Type botCommand in botCommands)
            {
                var c = (Command)Activator.CreateInstance(botCommand);
                _commandsList.Add(c);
            }

            OnMessage += OnMessageRecieved;
            StartReceiving();
        }

        public void OnMessageRecieved(object sender, MessageEventArgs e)
        {
            try
            {
                Console.WriteLine(DateTime.Now.ToShortTimeString() + " " + e.Message.From.Username + ": " + e.Message.Text);

                #region Account
                Account senderAccount = AccountsList?.FirstOrDefault(a => a.ChatId == e.Message.Chat.Id);
                if (senderAccount == null)
                {
                    var accountFromDb =
                        Controller.AccountsController.GetList(new Account
                        {
                            ChatId = e.Message.Chat.Id,
                        });
                    if (accountFromDb.Count == 0)
                    {
                        senderAccount = new Account
                        {
                            ChatId = e.Message.Chat.Id,
                            Name = e.Message.Chat.Username,
                            Status = AccountStatus.Start,
                        };
                        if (e.Message.Chat.Username == null)
                            senderAccount.Name = e.Message.Chat.FirstName + " " + e.Message.Chat.LastName;
                    }
                    else
                    {
                        senderAccount = accountFromDb[0];
                        AccountsList?.Add(senderAccount);
                    }
                }
                #endregion

                foreach (var command in Commands)
                {
                    if (command.HasSameStatus(senderAccount.Status))
                    {
                        if (e.Message.Text != null && command.ContainsHome(e.Message?.Text, senderAccount))
                        {
                            command.Relieve(e.Message, this, senderAccount);
                            break;
                        }
                        command.Execute(e.Message, this, senderAccount);
                        break;
                    }
                }
            }
            catch (Exception ex) { Console.WriteLine(ex); }
        }

        public async void SendMessage(long chatId, object mes)
        {
            await SendTextMessageAsync(chatId, mes.ToString());
        }
        public async void SendMessage(Account account, object mes)
        {
            await SendTextMessageAsync(account.ChatId, mes.ToString());
        }
        public void SendPhoto(InputOnlineFile file, long chatId)
        {
            SendPhotoAsync(chatId, file);
        }
    }
}
