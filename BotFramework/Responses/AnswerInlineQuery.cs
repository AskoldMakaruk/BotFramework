using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types.InlineQueryResults;

namespace BotFramework.Responses
{
    public class AnswerInlineQuery : IResponseMessage
    {
        public AnswerInlineQuery(string                             inlineQueryId,
                                 IEnumerable<InlineQueryResultBase> results,
                                 int?                               cacheTime         = null,
                                 bool                               isPersonal        = false,
                                 string                             nextOffset        = null,
                                 string                             switchPmText      = null,
                                 string                             switchPmParameter = null)
        {
            InlineQueryId     = inlineQueryId;
            Results           = results;
            CacheTime         = cacheTime;
            IsPersonal        = isPersonal;
            NextOffset        = nextOffset;
            SwitchPmText      = switchPmText;
            SwitchPmParameter = switchPmParameter;
        }

        public string                             InlineQueryId     { get; }
        public IEnumerable<InlineQueryResultBase> Results           { get; }
        public int?                               CacheTime         { get; }
        public bool                               IsPersonal        { get; }
        public string                             NextOffset        { get; }
        public string                             SwitchPmText      { get; }
        public string                             SwitchPmParameter { get; }
        public async Task Send(TelegramBotClient botClient)
        {
            await botClient.AnswerInlineQueryAsync(InlineQueryId, Results,
                CacheTime, IsPersonal, NextOffset,
                SwitchPmText, SwitchPmParameter);
        }
    }
}