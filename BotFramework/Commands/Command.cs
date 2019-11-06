using BotFramework.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Commands
{
    public abstract class Command : ICommand
    {
        public Response Response { get; set; }

        public void TextMessage(string text, IReplyMarkup replyMarkup = null,
                                int    replyToMessageId = 0)
        {
            Response.Responses
                    .Add(new ResponseMessage(ResponseType.TextMessage)
                    {
                        ChatId           = chat,
                        Text             = text,
                        ReplyMarkup      = replyMarkup,
                        ReplyToMessageId = replyToMessageId
                    });
        }

        protected ChatId  chat    { get; set; }
        protected Message message { get; set; }

        public Response Execute(Message mes, Client client, long accId)
        {
            message = mes;
            chat    = message.From.Id;
            Run();
            return Response;
        }

        protected abstract void Run();
    }

    public class StartCommand : Command
    {
        protected override void Run()
        {
            if (message.Text == "Some text")
            {
                TextMessage("Some message");
            }
        }
    }
}