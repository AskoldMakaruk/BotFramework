using System.Collections.Generic;
using System.Linq;
using BotFramework.Bot;
using BotFramework.Responses;
using Monads;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace BotFramework.Commands
{
    public abstract class CallbackQueryCommand : IStaticCommand
    {
        public abstract string     Alias      { get; }
        public          UpdateType UpdateType => UpdateType.CallbackQuery;

        public Optional<Response> Run(Update update, Client client) =>
        update.Type == UpdateType && Suitable(update)
        ? Execute(update.CallbackQuery, client, UnpackParams(update.CallbackQuery.Data))
        : new Optional<Response>();

        public abstract Optional<Response> Execute(CallbackQuery message, Client client, Dictionary<string, string> values);

        public bool Suitable(Update message)
        {
            return message.CallbackQuery?.Data?.StartsWith(Alias) ?? false;
        }

        public static Dictionary<string, string> UnpackParams(string input)
        {
            if (!input.Contains(' ')) return null;
            return input.Substring(input.IndexOf(' ') + 1)
                        .Split('&')
                        .Select(s => s.Split('='))
                        .ToDictionary(r => r[0], r => r[1]);
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
}