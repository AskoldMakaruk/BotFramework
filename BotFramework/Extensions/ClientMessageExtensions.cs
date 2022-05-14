using System.Threading.Tasks;
using BotFramework.Abstractions;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace BotFramework.Extensions;

public static class ClientMessageExtensions
{
    public static async Task<Message?> Send(this IClient client, Reply reply)
    {
        var chatId = reply.ChatId == null ? null : new ChatId(reply.ChatId.Value);
        if (reply.EditMessageId != null)
        {
            return await client.EditMessageText(reply.EditMessageId.Value, reply.Text, chatId,
                       replyMarkup: reply.ReplyMarkup as InlineKeyboardMarkup, parseMode: ParseMode.Html);
        }

        if (reply.AnswerCallbackQueryId != null)
        {
            await client.AnswerCallbackQuery(reply.AnswerCallbackQueryId, reply.Text, reply.ShowAlert);
        }
        else
        {
            return await client.SendTextMessage(reply.Text, chatId, replyMarkup: reply.ReplyMarkup, parseMode: ParseMode.Html);
        }

        return null;
    }

    public static async Task<Message?> Send(this ITelegramBotClient client, Reply reply)
    {
        var chatId = reply.ChatId!;
        if (reply.EditMessageId != null)
        {
            return await client.EditMessageTextAsync(chatId, reply.EditMessageId.Value, reply.Text,
                       replyMarkup: reply.ReplyMarkup as InlineKeyboardMarkup, parseMode: ParseMode.Html);
        }

        if (reply.AnswerCallbackQueryId != null)
        {
            await client.AnswerCallbackQueryAsync(reply.AnswerCallbackQueryId, reply.Text, reply.ShowAlert);
        }
        else
        {
            return await client.SendTextMessageAsync(chatId, reply.Text, replyMarkup: reply.ReplyMarkup,
                       parseMode: ParseMode.Html);
        }

        return null;
    }
}

public record Reply(string  Text, IReplyMarkup? ReplyMarkup = null, long? ChatId = null, int? EditMessageId = null,
                    string? AnswerCallbackQueryId = null, bool ShowAlert = false)
{
    public static Reply Message(string text, IReplyMarkup? markup = null, long? chatId = null)
    {
        return new Reply(text, markup, chatId);
    }

    public static Reply Edit(string text, CallbackQuery query, IReplyMarkup? replyMarkup = null)
    {
        return new Reply(text, replyMarkup, query.Message?.Chat.Id, query.Message?.MessageId);
    }

    public static Reply Edit(string text, int messageId, long chatId, IReplyMarkup? replyMarkup = null)
    {
        return new Reply(text, replyMarkup, chatId, messageId);
    }
}