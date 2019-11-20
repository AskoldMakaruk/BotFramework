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
        private async Task SendResponse(Response m)
        {
            try
            {
                //todo response count is 0
                foreach (var message in m.Responses)
                    switch (message.Type)
                    {
                        case ResponseType.AnswerQuery:
                            await Bot.AnswerCallbackQueryAsync(message.AnswerToMessageId, message.Text);
                            break;
                        case ResponseType.EditTextMesage:
                            await Bot.EditMessageTextAsync(message.ChatId, message.EditMessageId, message.Text,
                                replyMarkup : message.ReplyMarkup as InlineKeyboardMarkup, parseMode : message.ParseMode);
                            break;
                        case ResponseType.SendDocument:
                            await Bot.SendDocumentAsync(message.ChatId, message.Document, message.Text);
                            break;
                        case ResponseType.SendPhoto:
                            await Bot.SendPhotoAsync(message.ChatId, message.Document, message.Text);
                            break;
                        case ResponseType.TextMessage:
                            await Bot.SendTextMessageAsync(message.ChatId, message.Text,
                                replyToMessageId : message.ReplyToMessageId,
                                replyMarkup : message.ReplyMarkup, parseMode : message.ParseMode);
                            break;
                        case ResponseType.Album:
                            await Bot.SendMediaGroupAsync(message.Album, message.ChatId);
                            break;
                        case ResponseType.EditMessageMarkup:
                            await Bot.EditMessageReplyMarkupAsync(message.ChatId, message.MessageId,
                                message.ReplyMarkup as InlineKeyboardMarkup);
                            break;
                        default:
                            throw new ArgumentOutOfRangeException();
                    }
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        private async Task<Message> SendTextMessage(long account, string text,
            ParseMode parseMode = ParseMode.Default,
            bool disableWebPagePreview = false,
            bool disableNotification = false,
            int replyToMessageId = 0,
            IReplyMarkup replyMarkup = null,
            CancellationToken cancellationToken = default)
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