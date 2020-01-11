using System.Threading.Tasks;
using Telegram.Bot;

namespace BotFramework.Responses
{
    public class AnswerCallbackQuery : IResponseMessage
    {
        public AnswerCallbackQuery(string callbackQueryId,
                                   string text      = null,
                                   bool   showAlert = false,
                                   string url       = null,
                                   int    cacheTime = 0)
        {
            CallbackQueryId = callbackQueryId;
            Text            = text;
            ShowAlert       = showAlert;
            Url             = url;
            CacheTime       = cacheTime;
        }

        public string CallbackQueryId { get; }
        public string Text            { get; }
        public bool   ShowAlert       { get; }
        public string Url             { get; }
        public int    CacheTime       { get; }

        public async Task Send(TelegramBotClient botClient)
        {
            await botClient.AnswerCallbackQueryAsync(CallbackQueryId, Text, ShowAlert, Url, CacheTime);
        }
    }
}