using BotFramework.Bot;
using Optional;
using Optional.Linq;
using Telegram.Bot.Types;

namespace BotFramework.Commands
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
    public class MessageValidator : Validator<Message>
    {
        public Option<Message> Validate(Update update, IGetOnlyClient client) => update.Message.SomeNotNull();
    }
}