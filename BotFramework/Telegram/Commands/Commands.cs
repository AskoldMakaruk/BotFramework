using System;
using System.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Commands
{
    public interface ICommand
    {
        Response Execute(Message message, Client.Client client, long accId);
    }

    

    public interface IOneOfMany : ICommand
    {
        bool Suitable(Message message, long accId);
    }

    public abstract class KeyboardButtonCommand : IOneOfMany
    {
        public abstract string Name { get; }

        public virtual bool Suitable(Message message, long accId) =>
        message.Text == Name;

        public abstract Response Execute(Message message, Client.Client client, long accId);
    }

    public abstract class InputCommand : ICommand
    {
        public abstract MessageType[] InputTypes { get; }

        public virtual bool Suitable(Message message, long accId) =>
        InputTypes.Contains(message.Type);

        public Response Execute(Message message, Client.Client client, long accId)
        {
            return !Suitable(message, accId)
                   ? new Response().TextMessage(accId, "BadInput")
                   : Run(message, client, accId);
        }

        protected abstract Response Run(Message message, Client.Client client, long accId);
    }

    public abstract class StaticCommand : IOneOfMany
    {
        public abstract string Alias { get; }

        public virtual bool Suitable(Message message, long accId) =>
        message.Text == Alias;

        public abstract Response Execute(Message message, Client.Client client, long accUd);
    }
}