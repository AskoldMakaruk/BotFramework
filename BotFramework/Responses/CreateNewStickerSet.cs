using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;

namespace BotFramework.Responses
{
    public class CreateNewStickerSet : IResponseMessage
    {
        public CreateNewStickerSet(int             userId,
                                   string          name,
                                   string          title,
                                   InputOnlineFile pngSticker,
                                   string          emojis,
                                   bool            isMasks      = false,
                                   MaskPosition    maskPosition = null)
        {
            UserId       = userId;
            Name         = name;
            Title        = title;
            PngSticker   = pngSticker;
            Emojis       = emojis;
            IsMasks      = isMasks;
            MaskPosition = maskPosition;
        }

        public int             UserId       { get; }
        public string          Name         { get; }
        public string          Title        { get; }
        public InputOnlineFile PngSticker   { get; }
        public string          Emojis       { get; }
        public bool            IsMasks      { get; }
        public MaskPosition    MaskPosition { get; }

        public Task Send(TelegramBotClient botClient)
        {
            return botClient.CreateNewStickerSetAsync(UserId, Name, Title, PngSticker, Emojis, IsMasks, MaskPosition);
        }
    }
}