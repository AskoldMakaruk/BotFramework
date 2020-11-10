using System;
using System.Threading;
using BotFramework.Storage;
using Serilog;
using Serilog.Context;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Bot
{
    public static class DefaultHandlerCreator
    {
        public static DictionaryInMemoryHandler CreateDictionaryInMemoryHandler(this HandlerBuilder builder)
        {
            return new DictionaryInMemoryHandler(builder);
        }

        public static void CreateAndRunDictionaryInMemoryHandler(this HandlerBuilder builder)
        {
            var handler = new DictionaryInMemoryHandler(builder);
            builder.BotClient.OnUpdate += (sender, args) => handler.Handle(args.Update);
            builder.BotClient.StartReceiving();
            new ManualResetEvent(false).WaitOne();
        }
        
        public static long GetIdFromUpdate(Update update, ILogger logger)
        {
            long   from;
            string fromName, contents = "";
            switch (update.Type)
            {
                case UpdateType.Message:
                    var message = update.Message;
                    from     = message.From.Id;
                    fromName = message.From.Username;
                    switch (update.Message.Type)
                    {
                        case MessageType.Text:
                            contents = message.Text;
                            break;
                        case MessageType.Sticker:
                            contents = message.Sticker.SetName;
                            break;
                        case MessageType.Photo:
                        case MessageType.Audio:
                        case MessageType.Video:
                        case MessageType.Document:
                            //Logger.Debug("{UpdateType}.{MessageType} | {From} {Caption}", update.Type, update.Message.Type, fromName, message.Caption);
                            //return from;
                            contents = message.Caption;
                            break;
                        case MessageType.Poll:
                            contents = message.Poll.Question;
                            break;
                        case MessageType.ChatTitleChanged:
                            contents = message.Chat.Title;
                            break;
                        case MessageType.Contact:
                            contents = $"{message.Contact.FirstName} {message.Contact.LastName} {message.Contact.PhoneNumber}";
                            break;
                        default:
                            //      Logger.Debug("{UpdateType}.{MessageType} | {From}", update.Type, message.Type, fromName);
                            //    return from;
                            break;
                    }

                    //Logger.Debug("{UpdateType}.{MessageType} | {From}: {Contents}", update.Type, message.Type, fromName, contents);
                    break;
                case UpdateType.InlineQuery:
                    from     = update.InlineQuery.From.Id;
                    fromName = update.InlineQuery.From.Username;
                    contents = update.InlineQuery.Query;
                    break;
                case UpdateType.ChosenInlineResult:
                    from     = update.ChosenInlineResult.From.Id;
                    fromName = update.ChosenInlineResult.From.Username;
                    contents = update.ChosenInlineResult.Query;
                    break;
                case UpdateType.CallbackQuery:
                    from     = update.CallbackQuery.From.Id;
                    fromName = update.CallbackQuery.From.Username;
                    contents = update.CallbackQuery.Data;
                    break;
                case UpdateType.EditedMessage:
                    from     = update.EditedMessage.From.Id;
                    fromName = update.EditedMessage.From.Username;
                    contents = update.EditedMessage.Text;
                    break;
                case UpdateType.ChannelPost:
                    from     = update.ChannelPost.From.Id;
                    fromName = update.ChannelPost.From.Username;
                    contents = update.ChannelPost.Text;
                    break;
                case UpdateType.EditedChannelPost:
                    from     = update.EditedChannelPost.From.Id;
                    fromName = update.EditedChannelPost.From.Username;
                    contents = update.EditedChannelPost.Text;
                    break;
                case UpdateType.ShippingQuery:
                    from     = update.ShippingQuery.From.Id;
                    fromName = update.ShippingQuery.From.Username;
                    contents = update.ShippingQuery.InvoicePayload;
                    break;
                case UpdateType.PreCheckoutQuery:
                    from     = update.PreCheckoutQuery.From.Id;
                    fromName = update.PreCheckoutQuery.From.Username;
                    contents = "";
                    break;
                default:
                    var ex = new NotImplementedException($"We don't support {update.Type} right now");
                    logger.Error(ex, ex.Message);
                    throw ex;
            }

            using (LogContext.PushProperty("UpdateType", update.Type))
            using (LogContext.PushProperty("MessageType", update.Message?.Type))
            using (LogContext.PushProperty("From", fromName))
            using (LogContext.PushProperty("Contents", contents))
            {
                logger.Debug("{UpdateType} {MessageType} | {From} {Contents}");
            }

            return from;
        }
    }
}