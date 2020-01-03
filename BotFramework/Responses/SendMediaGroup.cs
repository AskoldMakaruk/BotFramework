using System.Collections.Generic;
using Telegram.Bot.Types;

namespace BotFramework.Responses
{
    public class SendMediaGroup : IResponseMessage
    {
        public ResponseType Type => ResponseType.Album;

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
    }
}