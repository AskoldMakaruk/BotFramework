using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;

namespace BotFramework.Responses
{
    public class DeleteMessage : IResponseMessage
    {
        public readonly long ChatId;
        public readonly int MessageId;
        public readonly CancellationToken Token;
        public DeleteMessage(long chatId, int messageId, CancellationToken token = default)
        {
            ChatId = chatId;
            MessageId = messageId;
            Token = token;
        }

        public Task Send(TelegramBotClient botClient) =>
        botClient.DeleteMessageAsync(ChatId, MessageId, Token);
    }
}