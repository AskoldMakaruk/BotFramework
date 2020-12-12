using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Helpers
{
    public static class UpdateExtensions
    {
        public static long GetFromId(this Update update)
        {
            long from;
            switch (update.Type)
            {
                case UpdateType.Message:
                    var message = update.Message;
                    from = message.From.Id;
                    break;
                case UpdateType.InlineQuery:
                    from = update.InlineQuery.From.Id;
                    break;
                case UpdateType.ChosenInlineResult:
                    from = update.ChosenInlineResult.From.Id;
                    break;
                case UpdateType.CallbackQuery:
                    from = update.CallbackQuery.From.Id;
                    break;
                case UpdateType.EditedMessage:
                    from = update.EditedMessage.From.Id;
                    break;
                case UpdateType.ChannelPost:
                    from = update.ChannelPost.From.Id;
                    break;
                case UpdateType.EditedChannelPost:
                    from = update.EditedChannelPost.From.Id;
                    break;
                case UpdateType.ShippingQuery:
                    from = update.ShippingQuery.From.Id;
                    break;
                case UpdateType.PreCheckoutQuery:
                    from = update.PreCheckoutQuery.From.Id;
                    break;
                case UpdateType.Poll:
                case UpdateType.PollAnswer: 
                case UpdateType.Unknown:
                default:
                    return default;
            }


            return from;
        }
    }
}