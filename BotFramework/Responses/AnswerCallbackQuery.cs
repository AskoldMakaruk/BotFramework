namespace BotFramework.Responses
{
    public class AnswerCallbackQuery : IResponseMessage
    {
        public ResponseType Type => ResponseType.AnswerCallbackQuery;

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

        public readonly string CallbackQueryId;
        public readonly string Text;
        public readonly bool   ShowAlert;
        public readonly string Url;
        public readonly int    CacheTime;
    }
}