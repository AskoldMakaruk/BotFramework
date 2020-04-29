using BotFramework.Bot;
using Optional;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public interface Validator
    {
       Optional<object> Validate(Update update, IGetOnlyClient client);
    }
    public interface Validator<T> : Validator
    {
        new Optional<T> Validate(Update update, IGetOnlyClient client);
        Optional<object> Validator.Validate(Update update, IGetOnlyClient client)
        {
            return Validate(update, client).Select(t => (object) t);
        }
    }
    public class MessageValidator : Validator<Message>
    {
        public Optional<Message> Validate(Update update, IGetOnlyClient client) => update.Message.ToOptional();
    }
}