using System;
using Telegram.Bot.Types;
using TelegramBotCore.DB.Model;
using TelegramBotCore.Telegram.Bot;

namespace TelegramBotCore.Telegram.Commands
{
    public abstract class Command
    {
        public abstract bool Suitable(Message message, Account account);
        public abstract Response Execute(Message message, Client client, Account account);
        public abstract InputType InputTypes { get; }
    }
    public abstract class KeyboardButtonCommand : Command
    {
        public abstract string Name { get; }
        public abstract CommandStatus Keyboard { get; }
        public override bool Suitable(Message message, Account account)
        {
            return message.Text == Name && account.NextCommand == Keyboard;
        }
    }
    public abstract class InputCommand : Command
    {

    }
    public enum CommandStatus
    {
        Main
    }

    [Flags]
    public enum InputType
    {
        Text = 1,
        Photo = 2,
        Document = 4,
        Stiker = 8,
        SuccessfulPayment = 16
    }
}