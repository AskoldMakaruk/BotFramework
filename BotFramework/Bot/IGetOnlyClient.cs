using System.IO;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Types;
using File = Telegram.Bot.Types.File;

namespace BotFramework.Bot
{
    public interface IGetOnlyClient
    {
        public Task<File> GetFileAsync(
            string            fileId,
            CancellationToken cancellationToken = default);

        Task<Stream> DownloadFileAsync(
            string            filePath,
            CancellationToken cancellationToken = default);

        public Task<File> GetInfoAndDownloadFileAsync(
            string            fileId,
            Stream            destination,
            CancellationToken cancellationToken = default);

        public Task<Chat> GetChatAsync(ChatId chatId, CancellationToken cancellationToken = default);

        public Task<ChatMember[]> GetChatAdministratorsAsync(
            ChatId            chatId,
            CancellationToken cancellationToken = default);

        public Task<int> GetChatMembersCountAsync(
            ChatId            chatId,
            CancellationToken cancellationToken = default);

        public Task<ChatMember> GetChatMemberAsync(
            ChatId            chatId,
            int               userId,
            CancellationToken cancellationToken = default);

        public Task<GameHighScore[]> GetGameHighScoresAsync(
            int               userId,
            long              chatId,
            int               messageId,
            CancellationToken cancellationToken = default);

        public Task<GameHighScore[]> GetGameHighScoresAsync(
            int               userId,
            string            inlineMessageId,
            CancellationToken cancellationToken = default);

        public Task<string> ExportChatInviteLinkAsync(
            ChatId            chatId,
            CancellationToken cancellationToken = default);

        public Task<StickerSet> GetStickerSetAsync(
            string            name,
            CancellationToken cancellationToken = default);

        public Task<UserProfilePhotos> GetUserProfilePhotosAsync(
            int               userId,
            int               offset            = 0,
            int               limit             = 0,
            CancellationToken cancellationToken = default);
    }
}