using System.Threading.Tasks;
using Telegram.Bot;

namespace BotFramework.Responses
{
    public class SetStickerPositionInSet : IResponseMessage
    {
        public SetStickerPositionInSet(string sticker,
                                       int    position)
        {
            Sticker  = sticker;
            Position = position;
        }

        public string Sticker  { get; }
        public int    Position { get; }

        public Task Send(TelegramBotClient botClient)
        {
            return botClient.SetStickerPositionInSetAsync(Sticker, Position);
        }
    }
}