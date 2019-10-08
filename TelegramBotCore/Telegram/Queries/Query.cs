using Telegram.Bot.Types;
using TelegramBotCore.Controllers;
using TelegramBotCore.DB.Model;

namespace TelegramBotCore.Telegram.Queries
{
    public abstract class Query
    {
        public TelegramController Controller { get; set; }

        public abstract Response Execute(CallbackQuery    message, Account account);
        public abstract bool     IsSuitable(CallbackQuery message, Account account);
    }
}