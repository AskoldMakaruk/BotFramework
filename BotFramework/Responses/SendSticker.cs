using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Responses
{
    public class SendSticker : IResponseMessage
    {
        public SendSticker(ChatId          chatId,
                           InputOnlineFile sticker,
                           bool            disableNotification = false,
                           int             replyToMessageId    = 0,
                           IReplyMarkup    replyMarkup         = null)
        {
            ChatId              = chatId;
            Sticker             = sticker;
            DisableNotification = disableNotification;
            ReplyToMessageId    = replyToMessageId;
            ReplyMarkup         = replyMarkup;
        }

        public ChatId          ChatId              { get; }
        public InputOnlineFile Sticker             { get; }
        public bool            DisableNotification { get; }
        public int             ReplyToMessageId    { get; }
        public IReplyMarkup    ReplyMarkup         { get; }

        public async Task Send(TelegramBotClient botClient)
        {
            await botClient.SendStickerAsync(ChatId, Sticker, DisableNotification, ReplyToMessageId, ReplyMarkup);
        }
    }
}