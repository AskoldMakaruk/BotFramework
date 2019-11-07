using BotFramework.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Commands
{
    public abstract class Command : ICommand
    {
        public Response Answer { get; set; }

        protected ChatId  chat    { get; set; }
        protected Message message { get; set; }

        public Response Execute(Message mes, Client client, long accId)
        {
            message = mes;
            chat    = message.From.Id;
            Run();
            return Answer;
        }

        public void TextMessage(string text, IReplyMarkup replyMarkup = null,
                                int    replyToMessageId = 0)
        {
            //todo this shit
            Answer = new Response();
            Answer.
            .Add(new ResponseMessage(ResponseType.TextMessage)
            {
                ChatId           = chat,
                Text             = text,
                ReplyMarkup      = replyMarkup,
                ReplyToMessageId = replyToMessageId
            });
        }

        protected abstract void Run();
    }

    public class StartCommand : Command
    {
        protected override void Run()
        {
            if (message.Text == "Some text") TextMessage("Some message");
        }
    }
}