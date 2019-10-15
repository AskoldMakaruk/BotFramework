using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using TelegramBotCore.DB.Model;
using TelegramBotCore.Telegram.Bot;

namespace TelegramBotCore.Telegram.Commands
{
    public abstract class Command
    {
        public abstract bool     Suitable(Message message, Account account);
        public abstract Response Execute(Message  message, Client  client, Account account);
    }

    public abstract class KeyboardButtonCommand : Command
    {
        public abstract   string        Name     { get; }
        public abstract   CommandStatus Keyboard { get; }

        public override bool Suitable(Message message, Account account)
        {
            return message.Text == Name && account.Status == Keyboard;
        }
    }

    public abstract class InputCommand : Command
    {
        public abstract MessageType[] InputTypes { get; }
        public abstract CommandStatus Status     { get; }

        public override bool Suitable(Message message, Account account)
        {
            return InputTypes.Contains(message.Type) && account.Status == Status;
        }
    }

    public abstract class StaticCommand : Command
    {
        public abstract string Alias { get; }

        public override bool Suitable(Message message, Account account)
        {
            return message.Text == Alias;
        }
    }

    public enum CommandStatus
    {
        Main
    }
}