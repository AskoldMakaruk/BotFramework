using System.Collections.Generic;
using BotFramework.Bot;
using Optional;
using Telegram.Bot.Types;

namespace BotFramework.Commands.Validators
{
    public class CallBackQueryValidator : Validator<CallbackQuery>
    {
        private readonly Update update;
        public Option<CallbackQuery> Validate() => update.CallbackQuery.SomeNotNull();
        public CallBackQueryValidator(Update update) => this.update = update;
    }
}