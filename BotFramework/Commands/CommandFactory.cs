using System;
using BotFramework.Bot;
using BotFramework.Responses;
using Telegram.Bot.Types;

namespace BotFramework.Commands
{
    public static class CommandFactory
    {
        private class InlineCommand : ICommand
        {
            private readonly Func<Update, IGetOnlyClient, Response> _execute;
            private readonly Func<Update, bool>                     _suitable;

            public InlineCommand(Func<Update, IGetOnlyClient, Response> Execute, Func<Update, bool> Suitable)
            {
                _execute  = Execute;
                _suitable = Suitable;
            }

            public Response Execute(Update message, IGetOnlyClient client) => _execute(message, client);

            public bool Suitable(Update message) => _suitable(message);
        }

        public static ICommand CreateCommand(Func<Update, IGetOnlyClient, Response> Execute, Func<Update, bool> Suitable)
        {
            return new InlineCommand(Execute, Suitable);
        }
    }
}