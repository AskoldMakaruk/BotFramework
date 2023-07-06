using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using Telegram.Bot.Requests;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.Payments;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Extensions;

public static class ClientSendExtensions
{
    public static async Task<bool> AnswerCallbackQueryAsync(
        this IClient      client,
        string            callbackQueryId,
        string?           text              = default,
        bool?             showAlert         = default,
        string?           url               = default,
        int?              cacheTime         = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new AnswerCallbackQueryRequest(callbackQueryId)
        {
            Text              = text,
            ShowAlert         = showAlert,
            Url               = url,
            CacheTime         = cacheTime,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeleteMyCommandsAsync(
        this IClient      client,
        BotCommandScope?  scope             = default,
        string?           languageCode      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new DeleteMyCommandsRequest()
        {
            Scope             = scope,
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<BotCommand[]> GetMyCommandsAsync(
        this IClient      client,
        BotCommandScope?  scope             = default,
        string?           languageCode      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetMyCommandsRequest()
        {
            Scope             = scope,
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetMyCommandsAsync(
        this IClient            client,
        IEnumerable<BotCommand> commands,
        BotCommandScope?        scope             = default,
        string?                 languageCode      = default,
        bool                    isWebhookResponse = default,
        CancellationToken       cancellationToken = default) =>
    await client.MakeRequest(new SetMyCommandsRequest(commands)
        {
            Scope             = scope,
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<BotDescription> GetMyDescriptionAsync(
        this IClient      client,
        string?           languageCode      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetMyDescriptionRequest()
        {
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetMyDescriptionAsync(
        this IClient      client,
        string?           description       = default,
        string?           languageCode      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetMyDescriptionRequest()
        {
            Description       = description,
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<File> GetFileAsync(
        this IClient      client,
        string            fileId,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetFileRequest(fileId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<UserProfilePhotos> GetUserProfilePhotosAsync(
        this IClient      client,
        long              userId,
        int?              offset            = default,
        int?              limit             = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetUserProfilePhotosRequest(userId)
        {
            Offset            = offset,
            Limit             = limit,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<MenuButton> GetChatMenuButtonAsync(
        this IClient      client,
        long?             chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetChatMenuButtonRequest()
        {
            ChatId            = chatId,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<User> GetMeAsync(
        this IClient      client,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetMeRequest()
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<ChatAdministratorRights> GetMyDefaultAdministratorRightsAsync(
        this IClient      client,
        bool?             forChannels       = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetMyDefaultAdministratorRightsRequest()
        {
            ForChannels       = forChannels,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<BotName> GetMyNameAsync(
        this IClient      client,
        string?           languageCode      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetMyNameRequest()
        {
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> CloseAsync(
        this IClient      client,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new CloseRequest()
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> LogOutAsync(
        this IClient      client,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new LogOutRequest()
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> BanChatMemberAsync(
        this IClient      client,
        long              userId,
        ChatId?           chatId            = default,
        DateTime?         untilDate         = default,
        bool?             revokeMessages    = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new BanChatMemberRequest(chatId ?? client.UserId, userId)
        {
            UntilDate         = untilDate,
            RevokeMessages    = revokeMessages,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> BanChatSenderChatAsync(
        this IClient      client,
        long              senderChatId,
        ChatId?           chatId            = default,
        DateTime?         untilDate         = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new BanChatSenderChatRequest(chatId ?? client.UserId, senderChatId)
        {
            UntilDate         = untilDate,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> ApproveChatJoinAsync(
        this IClient      client,
        long              userId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new ApproveChatJoinRequest(chatId ?? client.UserId, userId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<ChatInviteLink> CreateChatInviteLinkAsync(
        this IClient      client,
        ChatId?           chatId             = default,
        string?           name               = default,
        DateTime?         expireDate         = default,
        int?              memberLimit        = default,
        bool?             createsJoinRequest = default,
        bool              isWebhookResponse  = default,
        CancellationToken cancellationToken  = default) =>
    await client.MakeRequest(new CreateChatInviteLinkRequest(chatId ?? client.UserId)
        {
            Name               = name,
            ExpireDate         = expireDate,
            MemberLimit        = memberLimit,
            CreatesJoinRequest = createsJoinRequest,
            IsWebhookResponse  = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeclineChatJoinAsync(
        this IClient      client,
        long              userId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new DeclineChatJoinRequest(chatId ?? client.UserId, userId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<ChatInviteLink> EditChatInviteLinkAsync(
        this IClient      client,
        string            inviteLink,
        ChatId?           chatId             = default,
        string?           name               = default,
        DateTime?         expireDate         = default,
        int?              memberLimit        = default,
        bool?             createsJoinRequest = default,
        bool              isWebhookResponse  = default,
        CancellationToken cancellationToken  = default) =>
    await client.MakeRequest(new EditChatInviteLinkRequest(chatId ?? client.UserId, inviteLink)
        {
            Name               = name,
            ExpireDate         = expireDate,
            MemberLimit        = memberLimit,
            CreatesJoinRequest = createsJoinRequest,
            IsWebhookResponse  = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<string> ExportChatInviteLinkAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new ExportChatInviteLinkRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<ChatInviteLink> RevokeChatInviteLinkAsync(
        this IClient      client,
        string            inviteLink,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new RevokeChatInviteLinkRequest(chatId ?? client.UserId, inviteLink)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> CloseForumTopicAsync(
        this IClient      client,
        int               messageThreadId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new CloseForumTopicRequest(chatId ?? client.UserId, messageThreadId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> CloseGeneralForumTopicAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new CloseGeneralForumTopicRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<ForumTopic> CreateForumTopicAsync(
        this IClient      client,
        string            name,
        ChatId?           chatId            = default,
        Color?            iconColor         = default,
        string?           iconCustomEmojiId = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new CreateForumTopicRequest(chatId ?? client.UserId, name)
        {
            IconColor         = iconColor,
            IconCustomEmojiId = iconCustomEmojiId,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeleteChatPhotoAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new DeleteChatPhotoRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeleteChatStickerSetAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new DeleteChatStickerSetRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeleteForumTopicAsync(
        this IClient      client,
        int               messageThreadId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new DeleteForumTopicRequest(chatId ?? client.UserId, messageThreadId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> EditForumTopicAsync(
        this IClient      client,
        int               messageThreadId,
        ChatId?           chatId            = default,
        string?           name              = default,
        string?           iconCustomEmojiId = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new EditForumTopicRequest(chatId ?? client.UserId, messageThreadId)
        {
            Name              = name,
            IconCustomEmojiId = iconCustomEmojiId,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> EditGeneralForumTopicAsync(
        this IClient      client,
        string            name,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new EditGeneralForumTopicRequest(chatId ?? client.UserId, name)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<ChatMember[]> GetChatAdministratorsAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetChatAdministratorsRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<int> GetChatMemberCountAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetChatMemberCountRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<ChatMember> GetChatMemberAsync(
        this IClient      client,
        long              userId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetChatMemberRequest(chatId ?? client.UserId, userId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Chat> GetChatAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetChatRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> HideGeneralForumTopicAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new HideGeneralForumTopicRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> LeaveChatAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new LeaveChatRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> PinChatMessageAsync(
        this IClient      client,
        int               messageId,
        ChatId?           chatId              = default,
        bool?             disableNotification = default,
        bool              isWebhookResponse   = default,
        CancellationToken cancellationToken   = default) =>
    await client.MakeRequest(new PinChatMessageRequest(chatId ?? client.UserId, messageId)
        {
            DisableNotification = disableNotification,
            IsWebhookResponse   = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> PromoteChatMemberAsync(
        this IClient      client,
        long              userId,
        ChatId?           chatId             = default,
        bool?             isAnonymous        = default,
        bool?             canManageChat      = default,
        bool?             canPostMessages    = default,
        bool?             canEditMessages    = default,
        bool?             canDeleteMessages  = default,
        bool?             canManageVideoChat = default,
        bool?             canRestrictMembers = default,
        bool?             canPromoteMembers  = default,
        bool?             canChangeInfo      = default,
        bool?             canInviteUsers     = default,
        bool?             canPinMessages     = default,
        bool?             canManageTopics    = default,
        bool              isWebhookResponse  = default,
        CancellationToken cancellationToken  = default) =>
    await client.MakeRequest(new PromoteChatMemberRequest(chatId ?? client.UserId, userId)
        {
            IsAnonymous        = isAnonymous,
            CanManageChat      = canManageChat,
            CanPostMessages    = canPostMessages,
            CanEditMessages    = canEditMessages,
            CanDeleteMessages  = canDeleteMessages,
            CanManageVideoChat = canManageVideoChat,
            CanRestrictMembers = canRestrictMembers,
            CanPromoteMembers  = canPromoteMembers,
            CanChangeInfo      = canChangeInfo,
            CanInviteUsers     = canInviteUsers,
            CanPinMessages     = canPinMessages,
            CanManageTopics    = canManageTopics,
            IsWebhookResponse  = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> ReopenForumTopicAsync(
        this IClient      client,
        int               messageThreadId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new ReopenForumTopicRequest(chatId ?? client.UserId, messageThreadId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> ReopenGeneralForumTopicAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new ReopenGeneralForumTopicRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> RestrictChatMemberAsync(
        this IClient      client,
        long              userId,
        ChatPermissions   permissions,
        ChatId?           chatId                        = default,
        bool?             useIndependentChatPermissions = default,
        DateTime?         untilDate                     = default,
        bool              isWebhookResponse             = default,
        CancellationToken cancellationToken             = default) =>
    await client.MakeRequest(new RestrictChatMemberRequest(chatId ?? client.UserId, userId, permissions)
        {
            UseIndependentChatPermissions = useIndependentChatPermissions,
            UntilDate                     = untilDate,
            IsWebhookResponse             = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetChatAdministratorCustomTitleAsync(
        this IClient      client,
        long              userId,
        string            customTitle,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetChatAdministratorCustomTitleRequest(chatId ?? client.UserId, userId, customTitle)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetChatDescriptionAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        string?           description       = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetChatDescriptionRequest(chatId ?? client.UserId)
        {
            Description       = description,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetChatPermissionsAsync(
        this IClient      client,
        ChatPermissions   permissions,
        ChatId?           chatId                        = default,
        bool?             useIndependentChatPermissions = default,
        bool              isWebhookResponse             = default,
        CancellationToken cancellationToken             = default) =>
    await client.MakeRequest(new SetChatPermissionsRequest(chatId ?? client.UserId, permissions)
        {
            UseIndependentChatPermissions = useIndependentChatPermissions,
            IsWebhookResponse             = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetChatPhotoAsync(
        this IClient      client,
        InputFileStream   photo,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetChatPhotoRequest(chatId ?? client.UserId, photo)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetChatStickerSetAsync(
        this IClient      client,
        string            stickerSetName,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetChatStickerSetRequest(chatId ?? client.UserId, stickerSetName)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetChatTitleAsync(
        this IClient      client,
        string            title,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetChatTitleRequest(chatId ?? client.UserId, title)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> UnbanChatMemberAsync(
        this IClient      client,
        long              userId,
        ChatId?           chatId            = default,
        bool?             onlyIfBanned      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new UnbanChatMemberRequest(chatId ?? client.UserId, userId)
        {
            OnlyIfBanned      = onlyIfBanned,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> UnbanChatSenderChatAsync(
        this IClient      client,
        long              senderChatId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new UnbanChatSenderChatRequest(chatId ?? client.UserId, senderChatId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> UnhideGeneralForumTopicAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new UnhideGeneralForumTopicRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> UnpinAllChatMessagesAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new UnpinAllChatMessagesRequest(chatId ?? client.UserId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> UnpinAllForumTopicMessagesAsync(
        this IClient      client,
        int               messageThreadId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new UnpinAllForumTopicMessagesRequest(chatId ?? client.UserId, messageThreadId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> UnpinChatMessageAsync(
        this IClient      client,
        ChatId?           chatId            = default,
        int?              messageId         = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new UnpinChatMessageRequest(chatId ?? client.UserId)
        {
            MessageId         = messageId,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<MessageId> CopyMessageAsync(
        this IClient                client,
        ChatId                      fromChatId,
        int                         messageId,
        ChatId?                     chatId                   = default,
        int?                        messageThreadId          = default,
        string?                     caption                  = default,
        ParseMode?                  parseMode                = default,
        IEnumerable<MessageEntity>? captionEntities          = default,
        bool?                       disableNotification      = default,
        bool?                       protectContent           = default,
        int?                        replyToMessageId         = default,
        bool?                       allowSendingWithoutReply = default,
        IReplyMarkup?               replyMarkup              = default,
        bool                        isWebhookResponse        = default,
        CancellationToken           cancellationToken        = default) =>
    await client.MakeRequest(new CopyMessageRequest(chatId ?? client.UserId, fromChatId, messageId)
        {
            MessageThreadId          = messageThreadId,
            Caption                  = caption,
            ParseMode                = parseMode,
            CaptionEntities          = captionEntities,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> ForwardMessageAsync(
        this IClient      client,
        ChatId            fromChatId,
        int               messageId,
        ChatId?           chatId              = default,
        int?              messageThreadId     = default,
        bool?             disableNotification = default,
        bool?             protectContent      = default,
        bool              isWebhookResponse   = default,
        CancellationToken cancellationToken   = default) =>
    await client.MakeRequest(new ForwardMessageRequest(chatId ?? client.UserId, fromChatId, messageId)
        {
            MessageThreadId     = messageThreadId,
            DisableNotification = disableNotification,
            ProtectContent      = protectContent,
            IsWebhookResponse   = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> EditInlineMessageLiveLocationAsync(
        this IClient          client,
        string                inlineMessageId,
        double                latitude,
        double                longitude,
        float?                horizontalAccuracy   = default,
        int?                  heading              = default,
        int?                  proximityAlertRadius = default,
        InlineKeyboardMarkup? replyMarkup          = default,
        bool                  isWebhookResponse    = default,
        CancellationToken     cancellationToken    = default) =>
    await client.MakeRequest(new EditInlineMessageLiveLocationRequest(inlineMessageId, latitude, longitude)
        {
            HorizontalAccuracy   = horizontalAccuracy,
            Heading              = heading,
            ProximityAlertRadius = proximityAlertRadius,
            ReplyMarkup          = replyMarkup,
            IsWebhookResponse    = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> EditMessageLiveLocationAsync(
        this IClient          client,
        int                   messageId,
        double                latitude,
        double                longitude,
        ChatId?               chatId               = default,
        float?                horizontalAccuracy   = default,
        int?                  heading              = default,
        int?                  proximityAlertRadius = default,
        InlineKeyboardMarkup? replyMarkup          = default,
        bool                  isWebhookResponse    = default,
        CancellationToken     cancellationToken    = default) =>
    await client.MakeRequest(new EditMessageLiveLocationRequest(chatId ?? client.UserId, messageId, latitude, longitude)
        {
            HorizontalAccuracy   = horizontalAccuracy,
            Heading              = heading,
            ProximityAlertRadius = proximityAlertRadius,
            ReplyMarkup          = replyMarkup,
            IsWebhookResponse    = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendLocationAsync(
        this IClient      client,
        double            latitude,
        double            longitude,
        ChatId?           chatId                   = default,
        int?              messageThreadId          = default,
        int?              livePeriod               = default,
        int?              heading                  = default,
        int?              proximityAlertRadius     = default,
        bool?             disableNotification      = default,
        bool?             protectContent           = default,
        int?              replyToMessageId         = default,
        bool?             allowSendingWithoutReply = default,
        IReplyMarkup?     replyMarkup              = default,
        bool              isWebhookResponse        = default,
        CancellationToken cancellationToken        = default) =>
    await client.MakeRequest(new SendLocationRequest(chatId ?? client.UserId, latitude, longitude)
        {
            MessageThreadId          = messageThreadId,
            LivePeriod               = livePeriod,
            Heading                  = heading,
            ProximityAlertRadius     = proximityAlertRadius,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendVenueAsync(
        this IClient      client,
        double            latitude,
        double            longitude,
        string            title,
        string            address,
        ChatId?           chatId                   = default,
        int?              messageThreadId          = default,
        string?           foursquareId             = default,
        string?           foursquareType           = default,
        string?           googlePlaceId            = default,
        string?           googlePlaceType          = default,
        bool?             disableNotification      = default,
        bool?             protectContent           = default,
        int?              replyToMessageId         = default,
        bool?             allowSendingWithoutReply = default,
        IReplyMarkup?     replyMarkup              = default,
        bool              isWebhookResponse        = default,
        CancellationToken cancellationToken        = default) =>
    await client.MakeRequest(new SendVenueRequest(chatId ?? client.UserId, latitude, longitude, title, address)
        {
            MessageThreadId          = messageThreadId,
            FoursquareId             = foursquareId,
            FoursquareType           = foursquareType,
            GooglePlaceId            = googlePlaceId,
            GooglePlaceType          = googlePlaceType,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> StopInlineMessageLiveLocationAsync(
        this IClient          client,
        string                inlineMessageId,
        InlineKeyboardMarkup? replyMarkup       = default,
        bool                  isWebhookResponse = default,
        CancellationToken     cancellationToken = default) =>
    await client.MakeRequest(new StopInlineMessageLiveLocationRequest(inlineMessageId)
        {
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> StopMessageLiveLocationAsync(
        this IClient          client,
        int                   messageId,
        ChatId?               chatId            = default,
        InlineKeyboardMarkup? replyMarkup       = default,
        bool                  isWebhookResponse = default,
        CancellationToken     cancellationToken = default) =>
    await client.MakeRequest(new StopMessageLiveLocationRequest(chatId ?? client.UserId, messageId)
        {
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendAnimationAsync(
        this IClient                client,
        InputFile                   animation,
        ChatId?                     chatId                   = default,
        int?                        messageThreadId          = default,
        int?                        duration                 = default,
        int?                        width                    = default,
        int?                        height                   = default,
        InputFile?                  thumbnail                = default,
        string?                     caption                  = default,
        ParseMode?                  parseMode                = default,
        IEnumerable<MessageEntity>? captionEntities          = default,
        bool?                       hasSpoiler               = default,
        bool?                       disableNotification      = default,
        bool?                       protectContent           = default,
        int?                        replyToMessageId         = default,
        bool?                       allowSendingWithoutReply = default,
        IReplyMarkup?               replyMarkup              = default,
        bool                        isWebhookResponse        = default,
        CancellationToken           cancellationToken        = default) =>
    await client.MakeRequest(new SendAnimationRequest(chatId ?? client.UserId, animation)
        {
            MessageThreadId          = messageThreadId,
            Duration                 = duration,
            Width                    = width,
            Height                   = height,
            Thumbnail                = thumbnail,
            Caption                  = caption,
            ParseMode                = parseMode,
            CaptionEntities          = captionEntities,
            HasSpoiler               = hasSpoiler,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendAudioAsync(
        this IClient                client,
        InputFile                   audio,
        ChatId?                     chatId                   = default,
        int?                        messageThreadId          = default,
        string?                     caption                  = default,
        ParseMode?                  parseMode                = default,
        IEnumerable<MessageEntity>? captionEntities          = default,
        int?                        duration                 = default,
        string?                     performer                = default,
        string?                     title                    = default,
        InputFile?                  thumbnail                = default,
        bool?                       disableNotification      = default,
        bool?                       protectContent           = default,
        int?                        replyToMessageId         = default,
        bool?                       allowSendingWithoutReply = default,
        IReplyMarkup?               replyMarkup              = default,
        bool                        isWebhookResponse        = default,
        CancellationToken           cancellationToken        = default) =>
    await client.MakeRequest(new SendAudioRequest(chatId ?? client.UserId, audio)
        {
            MessageThreadId          = messageThreadId,
            Caption                  = caption,
            ParseMode                = parseMode,
            CaptionEntities          = captionEntities,
            Duration                 = duration,
            Performer                = performer,
            Title                    = title,
            Thumbnail                = thumbnail,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SendChatActionAsync(
        this IClient      client,
        ChatAction        action,
        ChatId?           chatId            = default,
        int?              messageThreadId   = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SendChatActionRequest(chatId ?? client.UserId, action)
        {
            MessageThreadId   = messageThreadId,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendContactAsync(
        this IClient      client,
        string            phoneNumber,
        string            firstName,
        ChatId?           chatId                   = default,
        int?              messageThreadId          = default,
        string?           lastName                 = default,
        string?           vcard                    = default,
        bool?             disableNotification      = default,
        bool?             protectContent           = default,
        int?              replyToMessageId         = default,
        bool?             allowSendingWithoutReply = default,
        IReplyMarkup?     replyMarkup              = default,
        bool              isWebhookResponse        = default,
        CancellationToken cancellationToken        = default) =>
    await client.MakeRequest(new SendContactRequest(chatId ?? client.UserId, phoneNumber, firstName)
        {
            MessageThreadId          = messageThreadId,
            LastName                 = lastName,
            Vcard                    = vcard,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendDiceAsync(
        this IClient      client,
        ChatId?           chatId                   = default,
        int?              messageThreadId          = default,
        Emoji?            emoji                    = default,
        bool?             disableNotification      = default,
        bool?             protectContent           = default,
        int?              replyToMessageId         = default,
        bool?             allowSendingWithoutReply = default,
        IReplyMarkup?     replyMarkup              = default,
        bool              isWebhookResponse        = default,
        CancellationToken cancellationToken        = default) =>
    await client.MakeRequest(new SendDiceRequest(chatId ?? client.UserId)
        {
            MessageThreadId          = messageThreadId,
            Emoji                    = emoji,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendDocumentAsync(
        this IClient                client,
        InputFile                   document,
        ChatId?                     chatId                      = default,
        int?                        messageThreadId             = default,
        InputFile?                  thumbnail                   = default,
        string?                     caption                     = default,
        ParseMode?                  parseMode                   = default,
        IEnumerable<MessageEntity>? captionEntities             = default,
        bool?                       disableContentTypeDetection = default,
        bool?                       disableNotification         = default,
        bool?                       protectContent              = default,
        int?                        replyToMessageId            = default,
        bool?                       allowSendingWithoutReply    = default,
        IReplyMarkup?               replyMarkup                 = default,
        bool                        isWebhookResponse           = default,
        CancellationToken           cancellationToken           = default) =>
    await client.MakeRequest(new SendDocumentRequest(chatId ?? client.UserId, document)
        {
            MessageThreadId             = messageThreadId,
            Thumbnail                   = thumbnail,
            Caption                     = caption,
            ParseMode                   = parseMode,
            CaptionEntities             = captionEntities,
            DisableContentTypeDetection = disableContentTypeDetection,
            DisableNotification         = disableNotification,
            ProtectContent              = protectContent,
            ReplyToMessageId            = replyToMessageId,
            AllowSendingWithoutReply    = allowSendingWithoutReply,
            ReplyMarkup                 = replyMarkup,
            IsWebhookResponse           = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message[]> SendMediaGroupAsync(
        this IClient                  client,
        IEnumerable<IAlbumInputMedia> media,
        ChatId?                       chatId                   = default,
        int?                          messageThreadId          = default,
        bool?                         disableNotification      = default,
        bool?                         protectContent           = default,
        int?                          replyToMessageId         = default,
        bool?                         allowSendingWithoutReply = default,
        bool                          isWebhookResponse        = default,
        CancellationToken             cancellationToken        = default) =>
    await client.MakeRequest(new SendMediaGroupRequest(chatId ?? client.UserId, media)
        {
            MessageThreadId          = messageThreadId,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendMessageAsync(
        this IClient                client,
        string                      text,
        ChatId?                     chatId                   = default,
        int?                        messageThreadId          = default,
        ParseMode?                  parseMode                = default,
        IEnumerable<MessageEntity>? entities                 = default,
        bool?                       disableWebPagePreview    = default,
        bool?                       disableNotification      = default,
        bool?                       protectContent           = default,
        int?                        replyToMessageId         = default,
        bool?                       allowSendingWithoutReply = default,
        IReplyMarkup?               replyMarkup              = default,
        bool                        isWebhookResponse        = default,
        CancellationToken           cancellationToken        = default) =>
    await client.MakeRequest(new SendMessageRequest(chatId ?? client.UserId, text)
        {
            MessageThreadId          = messageThreadId,
            ParseMode                = parseMode,
            Entities                 = entities,
            DisableWebPagePreview    = disableWebPagePreview,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendPhotoAsync(
        this IClient                client,
        InputFile                   photo,
        ChatId?                     chatId                   = default,
        int?                        messageThreadId          = default,
        string?                     caption                  = default,
        ParseMode?                  parseMode                = default,
        IEnumerable<MessageEntity>? captionEntities          = default,
        bool?                       hasSpoiler               = default,
        bool?                       disableNotification      = default,
        bool?                       protectContent           = default,
        int?                        replyToMessageId         = default,
        bool?                       allowSendingWithoutReply = default,
        IReplyMarkup?               replyMarkup              = default,
        bool                        isWebhookResponse        = default,
        CancellationToken           cancellationToken        = default) =>
    await client.MakeRequest(new SendPhotoRequest(chatId ?? client.UserId, photo)
        {
            MessageThreadId          = messageThreadId,
            Caption                  = caption,
            ParseMode                = parseMode,
            CaptionEntities          = captionEntities,
            HasSpoiler               = hasSpoiler,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendPollAsync(
        this IClient                client,
        string                      question,
        IEnumerable<string>         options,
        ChatId?                     chatId                   = default,
        int?                        messageThreadId          = default,
        bool?                       isAnonymous              = default,
        PollType?                   type                     = default,
        bool?                       allowsMultipleAnswers    = default,
        int?                        correctOptionId          = default,
        string?                     explanation              = default,
        ParseMode?                  explanationParseMode     = default,
        IEnumerable<MessageEntity>? explanationEntities      = default,
        int?                        openPeriod               = default,
        DateTime?                   closeDate                = default,
        bool?                       isClosed                 = default,
        bool?                       disableNotification      = default,
        bool?                       protectContent           = default,
        int?                        replyToMessageId         = default,
        bool?                       allowSendingWithoutReply = default,
        IReplyMarkup?               replyMarkup              = default,
        bool                        isWebhookResponse        = default,
        CancellationToken           cancellationToken        = default) =>
    await client.MakeRequest(new SendPollRequest(chatId ?? client.UserId, question, options)
        {
            MessageThreadId          = messageThreadId,
            IsAnonymous              = isAnonymous,
            Type                     = type,
            AllowsMultipleAnswers    = allowsMultipleAnswers,
            CorrectOptionId          = correctOptionId,
            Explanation              = explanation,
            ExplanationParseMode     = explanationParseMode,
            ExplanationEntities      = explanationEntities,
            OpenPeriod               = openPeriod,
            CloseDate                = closeDate,
            IsClosed                 = isClosed,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendVideoNoteAsync(
        this IClient      client,
        InputFile         videoNote,
        ChatId?           chatId                   = default,
        int?              messageThreadId          = default,
        int?              duration                 = default,
        int?              length                   = default,
        InputFile?        thumbnail                = default,
        bool?             disableNotification      = default,
        bool?             protectContent           = default,
        int?              replyToMessageId         = default,
        bool?             allowSendingWithoutReply = default,
        IReplyMarkup?     replyMarkup              = default,
        bool              isWebhookResponse        = default,
        CancellationToken cancellationToken        = default) =>
    await client.MakeRequest(new SendVideoNoteRequest(chatId ?? client.UserId, videoNote)
        {
            MessageThreadId          = messageThreadId,
            Duration                 = duration,
            Length                   = length,
            Thumbnail                = thumbnail,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendVideoAsync(
        this IClient                client,
        InputFile                   video,
        ChatId?                     chatId                   = default,
        int?                        messageThreadId          = default,
        int?                        duration                 = default,
        int?                        width                    = default,
        int?                        height                   = default,
        InputFile?                  thumbnail                = default,
        string?                     caption                  = default,
        ParseMode?                  parseMode                = default,
        IEnumerable<MessageEntity>? captionEntities          = default,
        bool?                       hasSpoiler               = default,
        bool?                       supportsStreaming        = default,
        bool?                       disableNotification      = default,
        bool?                       protectContent           = default,
        int?                        replyToMessageId         = default,
        bool?                       allowSendingWithoutReply = default,
        IReplyMarkup?               replyMarkup              = default,
        bool                        isWebhookResponse        = default,
        CancellationToken           cancellationToken        = default) =>
    await client.MakeRequest(new SendVideoRequest(chatId ?? client.UserId, video)
        {
            MessageThreadId          = messageThreadId,
            Duration                 = duration,
            Width                    = width,
            Height                   = height,
            Thumbnail                = thumbnail,
            Caption                  = caption,
            ParseMode                = parseMode,
            CaptionEntities          = captionEntities,
            HasSpoiler               = hasSpoiler,
            SupportsStreaming        = supportsStreaming,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendVoiceAsync(
        this IClient                client,
        InputFile                   voice,
        ChatId?                     chatId                   = default,
        int?                        messageThreadId          = default,
        string?                     caption                  = default,
        ParseMode?                  parseMode                = default,
        IEnumerable<MessageEntity>? captionEntities          = default,
        int?                        duration                 = default,
        bool?                       disableNotification      = default,
        bool?                       protectContent           = default,
        int?                        replyToMessageId         = default,
        bool?                       allowSendingWithoutReply = default,
        IReplyMarkup?               replyMarkup              = default,
        bool                        isWebhookResponse        = default,
        CancellationToken           cancellationToken        = default) =>
    await client.MakeRequest(new SendVoiceRequest(chatId ?? client.UserId, voice)
        {
            MessageThreadId          = messageThreadId,
            Caption                  = caption,
            ParseMode                = parseMode,
            CaptionEntities          = captionEntities,
            Duration                 = duration,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetChatMenuButtonAsync(
        this IClient      client,
        long?             chatId            = default,
        MenuButton?       menuButton        = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetChatMenuButtonRequest()
        {
            ChatId            = chatId,
            MenuButton        = menuButton,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetMyDefaultAdministratorRightsAsync(
        this IClient             client,
        ChatAdministratorRights? rights            = default,
        bool?                    forChannels       = default,
        bool                     isWebhookResponse = default,
        CancellationToken        cancellationToken = default) =>
    await client.MakeRequest(new SetMyDefaultAdministratorRightsRequest()
        {
            Rights            = rights,
            ForChannels       = forChannels,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetMyNameAsync(
        this IClient      client,
        string?           name              = default,
        string?           languageCode      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetMyNameRequest()
        {
            Name              = name,
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<BotShortDescription> GetMyShortDescriptionAsync(
        this IClient      client,
        string?           languageCode      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetMyShortDescriptionRequest()
        {
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetMyShortDescriptionAsync(
        this IClient      client,
        string?           shortDescription  = default,
        string?           languageCode      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetMyShortDescriptionRequest()
        {
            ShortDescription  = shortDescription,
            LanguageCode      = languageCode,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<GameHighScore[]> GetGameHighScoresAsync(
        this IClient      client,
        long              userId,
        int               messageId,
        long?             chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetGameHighScoresRequest(userId, chatId ?? client.UserId, messageId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<GameHighScore[]> GetInlineGameHighScoresAsync(
        this IClient      client,
        long              userId,
        string            inlineMessageId,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetInlineGameHighScoresRequest(userId, inlineMessageId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendGameAsync(
        this IClient          client,
        string                gameShortName,
        long?                 chatId                   = default,
        int?                  messageThreadId          = default,
        bool?                 disableNotification      = default,
        bool?                 protectContent           = default,
        int?                  replyToMessageId         = default,
        bool?                 allowSendingWithoutReply = default,
        InlineKeyboardMarkup? replyMarkup              = default,
        bool                  isWebhookResponse        = default,
        CancellationToken     cancellationToken        = default) =>
    await client.MakeRequest(new SendGameRequest(chatId ?? client.UserId, gameShortName)
        {
            MessageThreadId          = messageThreadId,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SetGameScoreAsync(
        this IClient      client,
        long              userId,
        int               score,
        int               messageId,
        long?             chatId             = default,
        bool?             force              = default,
        bool?             disableEditMessage = default,
        bool              isWebhookResponse  = default,
        CancellationToken cancellationToken  = default) =>
    await client.MakeRequest(new SetGameScoreRequest(userId, score, chatId ?? client.UserId, messageId)
        {
            Force              = force,
            DisableEditMessage = disableEditMessage,
            IsWebhookResponse  = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetInlineGameScoreAsync(
        this IClient      client,
        long              userId,
        int               score,
        string            inlineMessageId,
        bool?             force              = default,
        bool?             disableEditMessage = default,
        bool              isWebhookResponse  = default,
        CancellationToken cancellationToken  = default) =>
    await client.MakeRequest(new SetInlineGameScoreRequest(userId, score, inlineMessageId)
        {
            Force              = force,
            DisableEditMessage = disableEditMessage,
            IsWebhookResponse  = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeleteWebhookAsync(
        this IClient      client,
        bool?             dropPendingUpdates = default,
        bool              isWebhookResponse  = default,
        CancellationToken cancellationToken  = default) =>
    await client.MakeRequest(new DeleteWebhookRequest()
        {
            DropPendingUpdates = dropPendingUpdates,
            IsWebhookResponse  = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Update[]> GetUpdatesAsync(
        this IClient             client,
        int?                     offset            = default,
        int?                     limit             = default,
        int?                     timeout           = default,
        IEnumerable<UpdateType>? allowedUpdates    = default,
        bool                     isWebhookResponse = default,
        CancellationToken        cancellationToken = default) =>
    await client.MakeRequest(new GetUpdatesRequest()
        {
            Offset            = offset,
            Limit             = limit,
            Timeout           = timeout,
            AllowedUpdates    = allowedUpdates,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<WebhookInfo> GetWebhookInfoAsync(
        this IClient      client,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetWebhookInfoRequest()
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetWebhookAsync(
        this IClient             client,
        string                   url,
        InputFileStream?         certificate        = default,
        string?                  ipAddress          = default,
        int?                     maxConnections     = default,
        IEnumerable<UpdateType>? allowedUpdates     = default,
        bool?                    dropPendingUpdates = default,
        string?                  secretToken        = default,
        bool                     isWebhookResponse  = default,
        CancellationToken        cancellationToken  = default) =>
    await client.MakeRequest(new SetWebhookRequest(url)
        {
            Certificate        = certificate,
            IpAddress          = ipAddress,
            MaxConnections     = maxConnections,
            AllowedUpdates     = allowedUpdates,
            DropPendingUpdates = dropPendingUpdates,
            SecretToken        = secretToken,
            IsWebhookResponse  = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> AnswerInlineQueryAsync(
        this IClient                   client,
        string                         inlineQueryId,
        IEnumerable<InlineQueryResult> results,
        int?                           cacheTime         = default,
        bool?                          isPersonal        = default,
        string?                        nextOffset        = default,
        InlineQueryResultsButton?      button            = default,
        bool                           isWebhookResponse = default,
        CancellationToken              cancellationToken = default) =>
    await client.MakeRequest(new AnswerInlineQueryRequest(inlineQueryId, results)
        {
            CacheTime         = cacheTime,
            IsPersonal        = isPersonal,
            NextOffset        = nextOffset,
            Button            = button,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<SentWebAppMessage> AnswerWebAppQueryAsync(
        this IClient      client,
        string            webAppQueryId,
        InlineQueryResult result,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new AnswerWebAppQueryRequest(webAppQueryId, result)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> AnswerPreCheckoutQueryAsync(
        this IClient      client,
        string            preCheckoutQueryId,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new AnswerPreCheckoutQueryRequest(preCheckoutQueryId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> AnswerShippingQueryAsync(
        this IClient      client,
        string            shippingQueryId,
        string            errorMessage,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new AnswerShippingQueryRequest(shippingQueryId, errorMessage)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<string> CreateInvoiceLinkAsync(
        this IClient              client,
        string                    title,
        string                    description,
        string                    payload,
        string                    providerToken,
        string                    currency,
        IEnumerable<LabeledPrice> prices,
        int?                      maxTipAmount              = default,
        IEnumerable<int>?         suggestedTipAmounts       = default,
        string?                   providerData              = default,
        string?                   photoUrl                  = default,
        int?                      photoSize                 = default,
        int?                      photoWidth                = default,
        int?                      photoHeight               = default,
        bool?                     needName                  = default,
        bool?                     needPhoneNumber           = default,
        bool?                     needEmail                 = default,
        bool?                     needShippingAddress       = default,
        bool?                     sendPhoneNumberToProvider = default,
        bool?                     sendEmailToProvider       = default,
        bool?                     isFlexible                = default,
        bool                      isWebhookResponse         = default,
        CancellationToken         cancellationToken         = default) =>
    await client.MakeRequest(new CreateInvoiceLinkRequest(title, description, payload, providerToken, currency, prices)
        {
            MaxTipAmount              = maxTipAmount,
            SuggestedTipAmounts       = suggestedTipAmounts,
            ProviderData              = providerData,
            PhotoUrl                  = photoUrl,
            PhotoSize                 = photoSize,
            PhotoWidth                = photoWidth,
            PhotoHeight               = photoHeight,
            NeedName                  = needName,
            NeedPhoneNumber           = needPhoneNumber,
            NeedEmail                 = needEmail,
            NeedShippingAddress       = needShippingAddress,
            SendPhoneNumberToProvider = sendPhoneNumberToProvider,
            SendEmailToProvider       = sendEmailToProvider,
            IsFlexible                = isFlexible,
            IsWebhookResponse         = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendInvoiceAsync(
        this IClient              client,
        string                    title,
        string                    description,
        string                    payload,
        string                    providerToken,
        string                    currency,
        IEnumerable<LabeledPrice> prices,
        long?                     chatId                    = default,
        int?                      messageThreadId           = default,
        int?                      maxTipAmount              = default,
        IEnumerable<int>?         suggestedTipAmounts       = default,
        string?                   startParameter            = default,
        string?                   providerData              = default,
        string?                   photoUrl                  = default,
        int?                      photoSize                 = default,
        int?                      photoWidth                = default,
        int?                      photoHeight               = default,
        bool?                     needName                  = default,
        bool?                     needPhoneNumber           = default,
        bool?                     needEmail                 = default,
        bool?                     needShippingAddress       = default,
        bool?                     sendPhoneNumberToProvider = default,
        bool?                     sendEmailToProvider       = default,
        bool?                     isFlexible                = default,
        bool?                     disableNotification       = default,
        bool?                     protectContent            = default,
        int?                      replyToMessageId          = default,
        bool?                     allowSendingWithoutReply  = default,
        InlineKeyboardMarkup?     replyMarkup               = default,
        bool                      isWebhookResponse         = default,
        CancellationToken         cancellationToken         = default) =>
    await client.MakeRequest(
        new SendInvoiceRequest(chatId ?? client.UserId, title, description, payload, providerToken, currency, prices)
        {
            MessageThreadId           = messageThreadId,
            MaxTipAmount              = maxTipAmount,
            SuggestedTipAmounts       = suggestedTipAmounts,
            StartParameter            = startParameter,
            ProviderData              = providerData,
            PhotoUrl                  = photoUrl,
            PhotoSize                 = photoSize,
            PhotoWidth                = photoWidth,
            PhotoHeight               = photoHeight,
            NeedName                  = needName,
            NeedPhoneNumber           = needPhoneNumber,
            NeedEmail                 = needEmail,
            NeedShippingAddress       = needShippingAddress,
            SendPhoneNumberToProvider = sendPhoneNumberToProvider,
            SendEmailToProvider       = sendEmailToProvider,
            IsFlexible                = isFlexible,
            DisableNotification       = disableNotification,
            ProtectContent            = protectContent,
            ReplyToMessageId          = replyToMessageId,
            AllowSendingWithoutReply  = allowSendingWithoutReply,
            ReplyMarkup               = replyMarkup,
            IsWebhookResponse         = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> AddStickerToSetAsync(
        this IClient      client,
        long              userId,
        string            name,
        InputSticker      sticker,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new AddStickerToSetRequest(userId, name, sticker)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> CreateNewStickerSetAsync(
        this IClient              client,
        long                      userId,
        string                    name,
        string                    title,
        IEnumerable<InputSticker> stickers,
        StickerFormat             stickerFormat,
        StickerType?              stickerType       = default,
        bool?                     needsRepainting   = default,
        bool                      isWebhookResponse = default,
        CancellationToken         cancellationToken = default) =>
    await client.MakeRequest(new CreateNewStickerSetRequest(userId, name, title, stickers, stickerFormat)
        {
            StickerType       = stickerType,
            NeedsRepainting   = needsRepainting,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeleteStickerFromSetAsync(
        this IClient      client,
        InputFileId       sticker,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new DeleteStickerFromSetRequest(sticker)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeleteStickerSetAsync(
        this IClient      client,
        string            name,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new DeleteStickerSetRequest(name)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Sticker[]> GetCustomEmojiStickersAsync(
        this IClient        client,
        IEnumerable<string> customEmojiIds,
        bool                isWebhookResponse = default,
        CancellationToken   cancellationToken = default) =>
    await client.MakeRequest(new GetCustomEmojiStickersRequest(customEmojiIds)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Sticker[]> GetForumTopicIconStickersAsync(
        this IClient      client,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetForumTopicIconStickersRequest()
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<StickerSet> GetStickerSetAsync(
        this IClient      client,
        string            name,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new GetStickerSetRequest(name)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> SendStickerAsync(
        this IClient      client,
        InputFile         sticker,
        ChatId?           chatId                   = default,
        int?              messageThreadId          = default,
        string?           emoji                    = default,
        bool?             disableNotification      = default,
        bool?             protectContent           = default,
        int?              replyToMessageId         = default,
        bool?             allowSendingWithoutReply = default,
        IReplyMarkup?     replyMarkup              = default,
        bool              isWebhookResponse        = default,
        CancellationToken cancellationToken        = default) =>
    await client.MakeRequest(new SendStickerRequest(chatId ?? client.UserId, sticker)
        {
            MessageThreadId          = messageThreadId,
            Emoji                    = emoji,
            DisableNotification      = disableNotification,
            ProtectContent           = protectContent,
            ReplyToMessageId         = replyToMessageId,
            AllowSendingWithoutReply = allowSendingWithoutReply,
            ReplyMarkup              = replyMarkup,
            IsWebhookResponse        = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetCustomEmojiStickerSetThumbnailAsync(
        this IClient      client,
        string            name,
        string?           customEmojiId     = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetCustomEmojiStickerSetThumbnailRequest(name)
        {
            CustomEmojiId     = customEmojiId,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetStickerEmojiListAsync(
        this IClient        client,
        InputFileId         sticker,
        IEnumerable<string> emojiList,
        bool                isWebhookResponse = default,
        CancellationToken   cancellationToken = default) =>
    await client.MakeRequest(new SetStickerEmojiListRequest(sticker, emojiList)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetStickerKeywordsAsync(
        this IClient         client,
        InputFileId          sticker,
        IEnumerable<string>? keywords          = default,
        bool                 isWebhookResponse = default,
        CancellationToken    cancellationToken = default) =>
    await client.MakeRequest(new SetStickerKeywordsRequest(sticker)
        {
            Keywords          = keywords,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetStickerMaskPositionAsync(
        this IClient      client,
        InputFileId       sticker,
        MaskPosition?     maskPosition      = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetStickerMaskPositionRequest(sticker)
        {
            MaskPosition      = maskPosition,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetStickerPositionInSetAsync(
        this IClient      client,
        InputFileId       sticker,
        int               position,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetStickerPositionInSetRequest(sticker, position)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetStickerSetThumbnailAsync(
        this IClient      client,
        string            name,
        long              userId,
        InputFile?        thumbnail         = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetStickerSetThumbnailRequest(name, userId)
        {
            Thumbnail         = thumbnail,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> SetStickerSetTitleAsync(
        this IClient      client,
        string            name,
        string            title,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new SetStickerSetTitleRequest(name, title)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<File> UploadStickerFileAsync(
        this IClient      client,
        long              userId,
        InputFileStream   sticker,
        StickerFormat     stickerFormat,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new UploadStickerFileRequest(userId, sticker, stickerFormat)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> DeleteMessageAsync(
        this IClient      client,
        int               messageId,
        ChatId?           chatId            = default,
        bool              isWebhookResponse = default,
        CancellationToken cancellationToken = default) =>
    await client.MakeRequest(new DeleteMessageRequest(chatId ?? client.UserId, messageId)
        {
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> EditInlineMessageCaptionAsync(
        this IClient                client,
        string                      inlineMessageId,
        string?                     caption           = default,
        ParseMode?                  parseMode         = default,
        IEnumerable<MessageEntity>? captionEntities   = default,
        InlineKeyboardMarkup?       replyMarkup       = default,
        bool                        isWebhookResponse = default,
        CancellationToken           cancellationToken = default) =>
    await client.MakeRequest(new EditInlineMessageCaptionRequest(inlineMessageId)
        {
            Caption           = caption,
            ParseMode         = parseMode,
            CaptionEntities   = captionEntities,
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> EditInlineMessageMediaAsync(
        this IClient          client,
        string                inlineMessageId,
        InputMedia            media,
        InlineKeyboardMarkup? replyMarkup       = default,
        bool                  isWebhookResponse = default,
        CancellationToken     cancellationToken = default) =>
    await client.MakeRequest(new EditInlineMessageMediaRequest(inlineMessageId, media)
        {
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> EditInlineMessageReplyMarkupAsync(
        this IClient          client,
        string                inlineMessageId,
        InlineKeyboardMarkup? replyMarkup       = default,
        bool                  isWebhookResponse = default,
        CancellationToken     cancellationToken = default) =>
    await client.MakeRequest(new EditInlineMessageReplyMarkupRequest(inlineMessageId)
        {
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<bool> EditInlineMessageTextAsync(
        this IClient                client,
        string                      inlineMessageId,
        string                      text,
        ParseMode?                  parseMode             = default,
        IEnumerable<MessageEntity>? entities              = default,
        bool?                       disableWebPagePreview = default,
        InlineKeyboardMarkup?       replyMarkup           = default,
        bool                        isWebhookResponse     = default,
        CancellationToken           cancellationToken     = default) =>
    await client.MakeRequest(new EditInlineMessageTextRequest(inlineMessageId, text)
        {
            ParseMode             = parseMode,
            Entities              = entities,
            DisableWebPagePreview = disableWebPagePreview,
            ReplyMarkup           = replyMarkup,
            IsWebhookResponse     = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> EditMessageCaptionAsync(
        this IClient                client,
        int                         messageId,
        ChatId?                     chatId            = default,
        string?                     caption           = default,
        ParseMode?                  parseMode         = default,
        IEnumerable<MessageEntity>? captionEntities   = default,
        InlineKeyboardMarkup?       replyMarkup       = default,
        bool                        isWebhookResponse = default,
        CancellationToken           cancellationToken = default) =>
    await client.MakeRequest(new EditMessageCaptionRequest(chatId ?? client.UserId, messageId)
        {
            Caption           = caption,
            ParseMode         = parseMode,
            CaptionEntities   = captionEntities,
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> EditMessageMediaAsync(
        this IClient          client,
        int                   messageId,
        InputMedia            media,
        ChatId?               chatId            = default,
        InlineKeyboardMarkup? replyMarkup       = default,
        bool                  isWebhookResponse = default,
        CancellationToken     cancellationToken = default) =>
    await client.MakeRequest(new EditMessageMediaRequest(chatId ?? client.UserId, messageId, media)
        {
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> EditMessageReplyMarkupAsync(
        this IClient          client,
        int                   messageId,
        ChatId?               chatId            = default,
        InlineKeyboardMarkup? replyMarkup       = default,
        bool                  isWebhookResponse = default,
        CancellationToken     cancellationToken = default) =>
    await client.MakeRequest(new EditMessageReplyMarkupRequest(chatId ?? client.UserId, messageId)
        {
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Message> EditMessageTextAsync(
        this IClient                client,
        int                         messageId,
        string                      text,
        ChatId?                     chatId                = default,
        ParseMode?                  parseMode             = default,
        IEnumerable<MessageEntity>? entities              = default,
        bool?                       disableWebPagePreview = default,
        InlineKeyboardMarkup?       replyMarkup           = default,
        bool                        isWebhookResponse     = default,
        CancellationToken           cancellationToken     = default) =>
    await client.MakeRequest(new EditMessageTextRequest(chatId ?? client.UserId, messageId, text)
        {
            ParseMode             = parseMode,
            Entities              = entities,
            DisableWebPagePreview = disableWebPagePreview,
            ReplyMarkup           = replyMarkup,
            IsWebhookResponse     = isWebhookResponse
        }
      , cancellationToken);

    public static async Task<Poll> StopPollAsync(
        this IClient          client,
        int                   messageId,
        ChatId?               chatId            = default,
        InlineKeyboardMarkup? replyMarkup       = default,
        bool                  isWebhookResponse = default,
        CancellationToken     cancellationToken = default) =>
    await client.MakeRequest(new StopPollRequest(chatId ?? client.UserId, messageId)
        {
            ReplyMarkup       = replyMarkup,
            IsWebhookResponse = isWebhookResponse
        }
      , cancellationToken);
}