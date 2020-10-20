using System.Threading.Tasks;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public static class ClientExtensions
    {
        public static Task<Message> SendTextMessageAsync(this IClient client, string text, long? userId = null)
        {
            userId ??= client.UserId;
            return client.MakeRequestAsync(new SendMessageRequest(userId.Value, text));
        }

        public static async Task<Message> GetTextMessageAsync(this IClient client)
        {
            var update = await client.GetUpdateAsync(u => !string.IsNullOrEmpty(u.Message?.Text));
            return update.Message;
        }
    }
}