using System;
using System.IO;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Responses;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Bot
{
    public partial class Client
    {
        protected async Task SendResponse(Response m)
        {
            try
            {
                foreach (var message in m.Responses)
                    if (message.Type == ResponseType.AnswerCallbackQuery && message is AnswerCallbackQuery query)
                    {
                        await Bot.AnswerCallbackQueryAsync(query.CallbackQueryId, query.Text);
                    }
                    else if (message.Type == ResponseType.EditTextMesage && message is EditTextMessage editTextMessage)
                    {
                        await Bot.EditMessageTextAsync(editTextMessage.ChatId, editTextMessage.EditMessageId,
                            editTextMessage.Text,
                            replyMarkup: editTextMessage.ReplyMarkup as InlineKeyboardMarkup,
                            parseMode: editTextMessage.ParseMode);
                    }
                    else if (message.Type == ResponseType.SendDocument && message is SendDocument sendDocument)
                    {
                        await Bot.SendDocumentAsync(sendDocument.Account, sendDocument.Document, sendDocument.Caption);
                    }
                    else if (message.Type == ResponseType.SendPhoto && message is SendPhoto sendPhoto)
                    {
                        await Bot.SendPhotoAsync(sendPhoto.ChatId, sendPhoto.Photo, sendPhoto.Caption, sendPhoto.ParseMode,
                            sendPhoto.DisableNotification, sendPhoto.ReplyToMessageId, sendPhoto.ReplyMarkup);
                    }
                    else if (message.Type == ResponseType.TextMessage && message is TextMessage textMessage)
                    {
                        await Bot.SendTextMessageAsync(textMessage.ChatId, textMessage.Text, textMessage.ParseMode,
                            textMessage.DisableWebPagePreview, textMessage.DisableNotification, textMessage.ReplyToMessageId,
                            textMessage.ReplyMarkup);
                    }
                    else if (message.Type == ResponseType.Album && message is SendMediaGroup media)
                    {
                        await Bot.SendMediaGroupAsync(media.InputMedia, media.ChatId, media.DisableNotification,
                            media.ReplyToMessageId);
                    }
                    else if (message.Type == ResponseType.EditMessageMarkup && message is EditMessageReplyMarkup edit)
                    {
                        await Bot.EditMessageReplyMarkupAsync(edit.ChatId, edit.MessageId, edit.ReplyMarkup);
                    }
                    else if (message.Type == ResponseType.SendSticker && message is SendSticker sticker)
                    {
                        await Bot.SendStickerAsync(sticker.ChatId, sticker.Sticker, sticker.DisableNotification,
                            sticker.ReplyToMessageId, sticker.ReplyMarkup);
                    }
                    else if (message.Type == ResponseType.AnswerInlineQuery && message is AnswerInlineQuery answerInlineQuery)
                    {
                        await Bot.AnswerInlineQueryAsync(answerInlineQuery.InlineQueryId, answerInlineQuery.Results,
                            answerInlineQuery.CacheTime, answerInlineQuery.IsPersonal, answerInlineQuery.NextOffset,
                            answerInlineQuery.SwitchPmText, answerInlineQuery.SwitchPmParameter);
                    }
                    else throw new ArgumentOutOfRangeException();
            }
            catch (Exception e)
            {
                Console.WriteLine(e.StackTrace);
            }
        }

        protected async Task<Message> SendTextMessage(long              account, string text,
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