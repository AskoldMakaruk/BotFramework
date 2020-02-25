using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace BotFramework.Responses
{
    public class AddStickerToSet : IResponseMessage
    {
        public AddStickerToSet(int             userId,
                               string          name,
                               InputOnlineFile pngSticker,
                               string          emojis,
                               MaskPosition    maskPosition = null
        )
        {
            UserId       = userId;
            Name         = name;
            PngSticker   = pngSticker;
            Emojis       = emojis;
            MaskPosition = maskPosition;
        }

        public int             UserId       { get; }
        public string          Name         { get; }
        public InputOnlineFile PngSticker   { get; }
        public string          Emojis       { get; }
        public MaskPosition    MaskPosition { get; }

        public Task Send(TelegramBotClient botClient)
        {
            return botClient.AddStickerToSetAsync(UserId, Name, PngSticker, Emojis, MaskPosition);
        }
    }
}