using Telegram.Bot.Types;
using TelegramBotCore.Controllers;
using TelegramBotCore.DB.Model;
using TelegramBotCore.Telegram;

namespace StickerMemeBot.Telegram.Queries
{
    public abstract class Query
    {
        public Query(CallbackQuery message, Bot client, Account account)
        {
            Message = message;
            Client = client;
            Account = account;
        }
        public TelegramController Controller { get; set; }
        public CallbackQuery Message { get; }
        public Bot Client { get; }
        public Account Account { get; }

        public abstract void Execute();
        public abstract bool IsSuitable();
    }
}