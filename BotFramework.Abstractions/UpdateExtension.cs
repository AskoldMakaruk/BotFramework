using System;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

#nullable enable
namespace BotFramework.Abstractions;

public static class UpdateExtensions
{
    /// <summary>
    /// Returns ChatId if it isn't null, if it is, returns FromId.
    /// </summary>
    /// <param name="update">Message received from user.</param>
    /// <returns>Unique id of update's sender.</returns>
    public static long? GetId(this Update? update)
    {
        var info = GetInfoFromUpdate(update);
        return info?.Chat?.Id ?? info?.From?.Id;
    }

    public static User? GetUser(this Update? update) => GetInfoFromUpdate(update)?.From;

    public static string ToLogString(this Update? update) => GetInfoFromUpdate(update).ToString();

    public static ParsedUpdate GetInfoFromUpdate(this Update? update)
    {
        User? from = default;
        Chat? chat = default;
        string? fromName;
        var contents = string.Empty;

        switch (update?.Type)
        {
            case UpdateType.Message:
                var message = update.Message;
                from = message?.From;
                fromName = message?.From?.Username;
                chat = message?.Chat;
                switch (update.Message.Type)
                {
                    case MessageType.Unknown:
                        contents = "Unknown";
                        break;
                    case MessageType.Text:
                        contents = message?.Text;
                        break;
                    case MessageType.Photo:
                    case MessageType.Audio:
                    case MessageType.Video:
                    case MessageType.Voice:
                    case MessageType.Document:
                        contents = message?.Caption;
                        break;
                    case MessageType.Sticker:
                        contents = message?.Sticker?.SetName;
                        break;
                    case MessageType.Location:
                        contents = $"{message?.Location.Latitude} | {message?.Location.Longitude}";
                        break;
                    case MessageType.Contact:
                        contents =
                  $"{message?.Contact?.FirstName} {message?.Contact?.LastName} {message?.Contact?.PhoneNumber}";
                        break;
                    case MessageType.Venue:
                        contents = message?.Venue.Title;
                        break;
                    case MessageType.Game:
                        contents = message?.Game.Title;
                        break;
                    case MessageType.VideoNote:
                        contents = message?.VideoNote.Duration.ToString();
                        break;
                    case MessageType.Invoice:
                        contents = $"{message?.Invoice.Title}: {message?.Invoice.TotalAmount} {message?.Invoice.Currency}";
                        break;
                    case MessageType.SuccessfulPayment:
                        //todo enhance 
                        contents = $"{message?.SuccessfulPayment.OrderInfo.Name} {message?.SuccessfulPayment.OrderInfo.Email}";
                        break;
                    case MessageType.WebsiteConnected:
                        contents = message?.ConnectedWebsite;
                        break;
                    case MessageType.ChatMembersAdded:
                    
                        break;
                    case MessageType.ChatMemberLeft:
                        break;
                    case MessageType.ChatTitleChanged:
                        contents = message?.Chat?.Title;
                        break;
                    case MessageType.ChatPhotoChanged:
                        break;
                    case MessageType.MessagePinned:
                        break;
                    case MessageType.ChatPhotoDeleted:
                        break;
                    case MessageType.GroupCreated:
                        break;
                    case MessageType.SupergroupCreated:
                        break;
                    case MessageType.ChannelCreated:
                        break;
                    case MessageType.MigratedToSupergroup:
                        break;
                    case MessageType.MigratedFromGroup:
                        break;
                    case MessageType.Poll:
                        contents = message?.Poll?.Question;
                        break;
                    case MessageType.Dice:
                        break;
                    case MessageType.MessageAutoDeleteTimerChanged:
                        break;
                    case MessageType.ProximityAlertTriggered:
                        break;
                    case MessageType.VoiceChatScheduled:
                        break;
                    case MessageType.VoiceChatStarted:
                        break;
                    case MessageType.VoiceChatEnded:
                        break;
                    case MessageType.VoiceChatParticipantsInvited:
                        break;
                }
                break;
            case UpdateType.InlineQuery:
                from     = update.InlineQuery?.From;
                fromName = update.InlineQuery?.From?.Username;
                contents = update.InlineQuery?.Query;
                break;
            case UpdateType.ChosenInlineResult:
                from     = update?.ChosenInlineResult?.From;
                fromName = update?.ChosenInlineResult?.From?.Username;
                contents = update?.ChosenInlineResult?.Query;
                break;
            case UpdateType.CallbackQuery:
                from     = update?.CallbackQuery?.From;
                fromName = update?.CallbackQuery?.From?.Username;
                contents = update?.CallbackQuery?.Data;
                chat     = update?.CallbackQuery?.Message?.Chat;
                break;
            case UpdateType.EditedMessage:
                from     = update?.EditedMessage?.From;
                fromName = update?.EditedMessage?.From?.Username;
                contents = update?.EditedMessage?.Text;
                chat     = update?.EditedMessage?.Chat;
                break;
            case UpdateType.ChannelPost:
                chat     = update?.ChannelPost?.Chat;
                fromName = update?.ChannelPost?.Chat?.Title;
                contents = update?.ChannelPost?.Text;
                break;
            case UpdateType.EditedChannelPost:
                from     = update?.EditedChannelPost?.From;
                fromName = update?.EditedChannelPost?.From?.Username;
                contents = update?.EditedChannelPost?.Text;
                chat     = update?.EditedChannelPost?.Chat;
                break;
            case UpdateType.ShippingQuery:
                from     = update.ShippingQuery?.From;
                fromName = update.ShippingQuery?.From?.Username;
                contents = update.ShippingQuery?.InvoicePayload;
                break;
            case UpdateType.PreCheckoutQuery:
                from     = update.PreCheckoutQuery?.From;
                fromName = update.PreCheckoutQuery?.From?.Username;
                break;
            case UpdateType.MyChatMember:
                from     = update.MyChatMember?.From;
                fromName = update.MyChatMember?.From?.Username;
                break;
            case UpdateType.ChatMember:
                from     = update.ChatMember?.From;
                fromName = update.ChatMember?.From?.Username;
                break;
            case UpdateType.Poll:
            case UpdateType.PollAnswer:
            case UpdateType.Unknown:
            case null:
            default:
                from     = null;
                fromName = null;
                break;
        }

        return new ParsedUpdate(from, chat, update?.Type, update?.Message?.Type, fromName, contents);
    }

    public record ParsedUpdate(User? From, Chat? Chat, UpdateType? UpdateType, MessageType? MessageType, string? FromName,
                               string? Contents)
    {
        public override string ToString()
        {
            return $"{UpdateType} {MessageType} | {FromName} {Contents}";
        }
    }
}
