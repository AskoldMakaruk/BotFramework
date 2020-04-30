using System.Collections.Generic;
using System.Linq;
using BotFramework.Bot;
using Optional;
using Optional.Linq;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands.Validators
{
    public class CallBackQueryValidator : Validator<ParsedCallBackQuery>
    {
        public Option<ParsedCallBackQuery> Validate(Update update, IGetOnlyClient client)
        {
            return from query in update.CallbackQuery.SomeWhen(t => t != null && update.Type == UpdateType.CallbackQuery)
                   from values in UnpackParams(query.Data)
                   select new ParsedCallBackQuery(query, values);
        }

        public static Option<Dictionary<string, string>> UnpackParams(string input)
        {
            if (!input.Contains(' ')) return Option.None<Dictionary<string, string>>();
            return input.Substring(input.IndexOf(' ') + 1)
                        .Split('&')
                        .Select(s => s.Split('='))
                        .ToDictionary(r => r[0], r => r[1])
                        .Some();
        }

        public static string PackParams(string Alias, string Name, string Value)
        {
            return PackParams(Alias, (Name, Value));
        }

        public static string PackParams(string Alias, params (string Name, string Value)[] input)
        {
            return $"{Alias} {string.Join('&'.ToString(), input.Select(i => $"{i.Name}={i.Value}"))}";
        }
    }

    public class ParsedCallBackQuery
    {
        public CallbackQuery              Query  { get; }
        public Dictionary<string, string> Values { get; }
        public ParsedCallBackQuery(CallbackQuery query, Dictionary<string, string> values) => (Query, Values) = (query, values);
    }
}