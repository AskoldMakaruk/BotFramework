using System.Threading.Tasks;
using Telegram.Bot;

namespace BotFramework.Responses
{
    public class DeleteStickerFromSet : IResponseMessage
    {
        public DeleteStickerFromSet(string sticker)
        {
            Sticker = sticker;
        }

        public string Sticker { get; }

        public Task Send(TelegramBotClient botClient)
        {
            return botClient.DeleteStickerFromSetAsync(Sticker);
        }
    }
}