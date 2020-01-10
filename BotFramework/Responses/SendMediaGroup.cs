using System.Collections.Generic;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Types;

namespace BotFramework.Responses
{
    public class SendMediaGroup : ResponseMessage<Message[]>
    {
        public SendMediaGroup(IEnumerable<IAlbumInputMedia> inputMedia,
                              ChatId                        chatId,
                              bool                          disableNotification = false,
                              int                           replyToMessageId    = 0)
        {
            InputMedia          = inputMedia;
            ChatId              = chatId;
            DisableNotification = disableNotification;
            ReplyToMessageId    = replyToMessageId;
        }

        public IEnumerable<IAlbumInputMedia> InputMedia          { get; }
        public ChatId                        ChatId              { get; }
        public bool                          DisableNotification { get; }
        public int                           ReplyToMessageId    { get; }

        protected override Task<Message[]> Send(TelegramBotClient botClient) =>
        botClient.SendMediaGroupAsync(InputMedia, ChatId, DisableNotification, ReplyToMessageId);
    }
}