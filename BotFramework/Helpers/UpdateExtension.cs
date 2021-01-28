using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Helpers
{
    public static class UpdateExtensions
    {
        public static User? GetUser(this Update update)
        {
            User from;
            switch (update.Type)
            {
                case UpdateType.Message:
                    var message = update.Message;
                    from = message.From;
                    break;
                case UpdateType.InlineQuery:
                    from = update.InlineQuery.From;
                    break;
                case UpdateType.ChosenInlineResult:
                    from = update.ChosenInlineResult.From;
                    break;
                case UpdateType.CallbackQuery:
                    from = update.CallbackQuery.From;
                    break;
                case UpdateType.EditedMessage:
                    from = update.EditedMessage.From;
                    break;
                case UpdateType.ChannelPost:
                    from = update.ChannelPost.From;
                    break;
                case UpdateType.EditedChannelPost:
                    from = update.EditedChannelPost.From;
                    break;
                case UpdateType.ShippingQuery:
                    from = update.ShippingQuery.From;
                    break;
                case UpdateType.PreCheckoutQuery:
                    from = update.PreCheckoutQuery.From;
                    break;
                default:
                    return null;
            }


            return from;
        }
    }
}