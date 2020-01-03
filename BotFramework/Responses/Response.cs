using System.Collections.Generic;
using System.Collections.Immutable;
using BotFramework.Commands;

namespace BotFramework.Responses
{
    public class Response
    {
        internal readonly bool UsePreviousCommands;
        internal readonly bool UseStaticCommands = true;

        public Response UsePrevious(bool usePrevious) =>
            new Response(NextPossible, Responses, usePrevious, UseStaticCommands);

        public Response UseStatic(bool useStatic) =>
            new Response(NextPossible, Responses, UsePreviousCommands, useStatic);

        public Response(params ICommand[] nextPossible)
        {
            NextPossible = nextPossible;
            Responses    = ImmutableList<IResponseMessage>.Empty;
        }

        public Response(IEnumerable<ICommand> nextPossible)
        {
            NextPossible = nextPossible;
            Responses    = ImmutableList<IResponseMessage>.Empty;
        }

        public Response AddMessage(IResponseMessage message) => new Response(NextPossible, Responses.Add(message), UsePreviousCommands, UseStaticCommands);
        public Response SetMessages(ImmutableList<IResponseMessage> messages) => new Response(NextPossible, messages, UsePreviousCommands, UseStaticCommands);
            
        private Response(IEnumerable<ICommand> nextPossible, ImmutableList<IResponseMessage> responses, bool usePrevious,
                         bool                  useStaticCommands)
        {
            NextPossible        = nextPossible;
            Responses           = responses;
            UsePreviousCommands = usePrevious;
            UseStaticCommands   = useStaticCommands;
        }

        public readonly ImmutableList<IResponseMessage> Responses;

        public readonly IEnumerable<ICommand> NextPossible;
    }
}