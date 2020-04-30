using BotFramework.Bot;
using Optional;
using Optional.Linq;
using Telegram.Bot.Types;

namespace BotFramework.Commands.Validators
{
    public interface Validator
    {
       Option<object> Validate(Update update, IGetOnlyClient client);
    }
    public interface Validator<T> : Validator
    {
        new Option<T> Validate(Update update, IGetOnlyClient client);
        Option<object> Validator.Validate(Update update, IGetOnlyClient client)
        {
            return Validate(update, client).Select(t => (object) t);
        }
    }
}