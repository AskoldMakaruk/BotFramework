using System.Collections.Generic;
using BotFramework.Commands;

namespace BotFramework.Responses
{
    public class Response
    {
        public bool UsePreviousCommands;
        public readonly List<IResponseMessage> Responses;

        public readonly ICommand[] NextPossible;

        public Response(params ICommand[] nextPossible)
        {
            NextPossible = nextPossible;
            Responses    = new List<IResponseMessage>();
        }
        
        public Response UsePrevious(bool usePrevious)
        {
            UsePreviousCommands = usePrevious;
            return this;
        }

        public Response AddMessage(params IResponseMessage[] messages)
        {
            Responses.AddRange(messages);
            return this;
        }
    }
}