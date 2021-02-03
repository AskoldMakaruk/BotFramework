using System.Linq;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Clients.ClientExtensions
{
    public static class ClientGetExtensions
    {
        public static async ValueTask<Message> GetTextMessage(this IClient client)
        {
            var update = await client.GetUpdate(u => !string.IsNullOrEmpty(u.Message?.Text));
            return update.Message;
        }

        public static async ValueTask<Message> GetOnlyButtonResult(this IClient client, ReplyKeyboardMarkup replyMarkup)
        {
            var update = await client.GetUpdate(u =>
                         replyMarkup.Keyboard.SelectMany(t => t).Any(t => t.Text == u.Message?.Text));
            return update.Message;
        }
    }
}