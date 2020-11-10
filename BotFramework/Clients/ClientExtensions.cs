using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Clients
{
    public static class ClientExtensions
    {
        public static Task<Message> SendTextMessageAsync(this IClient      client, string text,
                                                         ChatId?           chatId                = null,
                                                         ParseMode         parseMode             = default,
                                                         bool              disableWebPagePreview = default,
                                                         bool              disableNotification   = default,
                                                         int               replyToMessageId      = default,
                                                         IReplyMarkup?     replyMarkup           = default,
                                                         CancellationToken cancellationToken     = default
        )
        {
            chatId ??= client.UserId;
            return client.MakeRequestAsync(new SendMessageRequest(chatId, text)
            {
                ParseMode             = parseMode,
                DisableWebPagePreview = disableWebPagePreview,
                DisableNotification   = disableNotification,
                ReplyToMessageId      = replyToMessageId,
                ReplyMarkup           = replyMarkup
            }, cancellationToken);
        }

        public static async Task<Message> GetTextMessageAsync(this IClient client)
        {
            var update = await client.GetUpdateAsync(u => !string.IsNullOrEmpty(u.Message?.Text));
            return update.Message;
        }
    }
}