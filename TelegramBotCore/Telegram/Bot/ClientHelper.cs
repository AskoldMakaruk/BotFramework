using System.Diagnostics;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using TelegramBotCore.DB.Model;

namespace TelegramBotCore.Telegram.Bot
{
    public partial class Client
    {
        private async Task<Message> SendTextMessageAsync(Response m)
        {
            if (m.EditMessageId == 0)
            {
                var message = await Bot.SendTextMessageAsync(m.Account, m.Text, replyToMessageId: m.ReplyToMessageId,
                                  replyMarkup: m.ReplyMarkup);
                return message;
            }
            else
            {
                var message = await Bot.EditMessageTextAsync(m.Account, m.EditMessageId, m.Text,
                                  replyMarkup: m.ReplyMarkup as InlineKeyboardMarkup);
                return message;
            }
        }

        private async Task<Message> SendTextMessageAsync(Account           account, string text,
                                                         ParseMode         parseMode             = ParseMode.Default,
                                                         bool              disableWebPagePreview = false,
                                                         bool              disableNotification   = false,
                                                         int               replyToMessageId      = 0,
                                                         IReplyMarkup      replyMarkup           = null,
                                                         CancellationToken cancellationToken     = default)
        {
            var message = await Bot.SendTextMessageAsync(account, text, parseMode, disableWebPagePreview,
                              disableNotification, replyToMessageId, replyMarkup, cancellationToken);
            return message;
        }

        public async Task GetInfoAndDownloadFileAsync(string documentFileId, MemoryStream ms) =>
        await Bot.GetInfoAndDownloadFileAsync(documentFileId, ms);
    }
}