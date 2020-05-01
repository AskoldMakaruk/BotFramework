using BotFramework.Bot;
using Optional;
using Optional.Linq;
using Telegram.Bot.Types;

namespace BotFramework.Commands.Validators
{
    public interface Validator
    {
       Option<object> Validate();
    }
    public interface Validator<T> : Validator
    {
        new Option<T> Validate();
        Option<object> Validator.Validate()
        {
            return Validate().Map(t => (object) t);
        }
    }
}