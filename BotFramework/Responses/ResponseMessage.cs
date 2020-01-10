using System;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Responses
{
    public abstract class ResponseMessage<T> : IResponseMessage
    {
        async Task IResponseMessage.Send(TelegramBotClient botClient)
        {
            var mess = await Send(botClient); // I am not sure that this await should be here
            OnSend?.Invoke(mess);
        }

        protected abstract Task<T> Send(TelegramBotClient botClient);

        public delegate void SendMessageHandler(T message);

        public event SendMessageHandler OnSend;
    }
}