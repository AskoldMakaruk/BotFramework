using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Bot
{
    public partial class Client
    {
        private async Task SendTextMessageAsync(Response m)
        {
            try
            {
                foreach (var message in m.Responses)
                {
                    if (message.Type == ResponseType.AnswerQuery)
                    {
                        await Bot.AnswerCallbackQueryAsync(message.AnswerToMessageId, message.Text);
                    }
                    else if (message.Type == ResponseType.EditTextMesage)
                    {
                        await Bot.EditMessageTextAsync(message.ChatId, message.EditMessageId, message.Text,
                            replyMarkup: message.ReplyMarkup as InlineKeyboardMarkup);
                    }
                    else if (message.Type == ResponseType.SendDocument)
                    {
                        await Bot.SendDocumentAsync(message.ChatId, message.Document, message.Text);
                    }
                    else if (message.Type == ResponseType.SendPhoto)
                    {
                        await Bot.SendPhotoAsync(message.ChatId, message.Document, message.Text);
                    }
                    else if (message.Type == ResponseType.TextMessage)
                    {
                        await Bot.SendTextMessageAsync(message.ChatId, message.Text,
                            replyToMessageId: message.ReplyToMessageId,
                            replyMarkup: message.ReplyMarkup);
                    }
                    else if (message.Type == ResponseType.Album)
                    {
                        await Bot.SendMediaGroupAsync(message.Album, message.ChatId);
                    }
                    else if (message.Type == ResponseType.EditMessageMarkup)
                    {
                        await Bot.EditMessageReplyMarkupAsync(message.ChatId, message.MessageId,
                            message.ReplyMarkup as InlineKeyboardMarkup);
                    }
                    else
                    {
                        throw new ArgumentOutOfRangeException();
                    }
                }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private async Task<Message> SendTextMessageAsync(long           account, string text,
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

        public async Task GetInfoAndDownloadFileAsync(string documentFileId, MemoryStream ms)
        {
            await Bot.GetInfoAndDownloadFileAsync(documentFileId, ms);
        }
    }
}