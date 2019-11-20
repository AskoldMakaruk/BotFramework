using System.Collections.Generic;
using System.Linq;
using BotFramework.Commands;
using Telegram.Bot.Types;

namespace BotFramework.Queries
{
    public abstract class Query
    {
        public abstract string Alias { get; }

        public Response Execute(CallbackQuery message)
        {
            return Run(message, UnpackParams(message.Data));
        }

        protected abstract Response Run(CallbackQuery message, Dictionary<string, string> values);

        public virtual bool IsSuitable(CallbackQuery message, long account)
        {
            return message.Data.StartsWith(Alias);
        }

        public static Dictionary<string, string> UnpackParams(string input)
        {
            return input.Substring(input.IndexOf(' ') + 1)
                .Split('&')
                .Select(s => s.Split('='))
                .ToDictionary(r => r[0], r => r[1]);
        }

        public static string PackParams(string Alias, string Name, string Value)
        {
            return PackParams(Alias, (Name, Value));
        }

        public static string PackParams(string Alias, params(string Name, string Value) [] input)
        {
            return $"{Alias} {string.Join('&'.ToString(), input.Select(i => $"{i.Name}={i.Value}"))}";
        }
    }
}