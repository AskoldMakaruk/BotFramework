using System.Collections.Generic;
using BotFramework.Commands;

namespace BotFramework.Responses
{
    public class Response
    {
        public bool UsePreviousCommands;
        public bool UseStaticCommands = true;
        
        public readonly List<IResponseMessage> Responses;

        public readonly HashSet<ICommand> NextPossible;

        public Response(params ICommand[] nextPossible)
        {
            NextPossible = new HashSet<ICommand>(nextPossible);
            Responses    = new List<IResponseMessage>();
        }
        
        public Response UsePrevious(bool usePrevious)
        {
            UsePreviousCommands = usePrevious;
            return this;
        }

        public Response UseStatic(bool useStatic)
        {
            UseStaticCommands = useStatic;
            return this;
        }

        public Response AddMessage(IResponseMessage message)
        {
            Responses.Add(message);
            return this;
        }
    }
}