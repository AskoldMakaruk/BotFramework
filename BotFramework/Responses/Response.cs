using System.Collections.Generic;
using System.Linq;
using BotFramework.Commands;

namespace BotFramework.Responses
{
    public class Response
    {
        internal bool UsePreviousCommands { get; private set; }
        internal bool UseStaticCommands   { get; private set; } = true;

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

        public Response(params ICommand[] nextPossible)
        {
            NextPossible = nextPossible;
        }

        public Response(IEnumerable<ICommand> nextPossible) : this(nextPossible.ToArray()) { }

        public Response(IEnumerable<ICommand> nextPossible, params IResponseMessage[] messages) : this(nextPossible)
        {
            responseMessages.AddRange(messages);
        }

        public Response(params IResponseMessage[] messages) : this(null, messages) { }

        public Response AddMessage(params IResponseMessage[] message)
        {
            responseMessages.AddRange(message);
            return this;
        }

        public Response SetMessages(List<IResponseMessage> messages)
        {
            var array = new IResponseMessage[messages.Count];
            messages.CopyTo(array);
            responseMessages = array.ToList();
            return this;
        }

        public          List<IResponseMessage> responseMessages { get; set; } = new List<IResponseMessage>();
        public readonly IEnumerable<ICommand>  NextPossible;
    }
}