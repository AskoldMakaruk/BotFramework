using System.Reflection;
using System.Text;
using Telegram.Bot.Requests;
using Telegram.Bot.Requests.Abstractions;

namespace Generator;

public static class ClientSendExtensionsGenerator
{
    public static void GenerateFile()
    {
        var requestClasses = typeof(SendMessageRequest).Assembly.GetTypes()
                                                       .Where(t => t.IsClass
                                                                   && t.IsPublic
                                                                   && !t.IsAbstract
                                                                   && t.IsAssignableTo(typeof(IRequest)))
                                                       .ToList();
        var builder = new StringBuilder();

        builder.AppendLine("using System;");
        builder.AppendLine("using System.Collections.Generic;");
        builder.AppendLine("using System.Threading;");
        builder.AppendLine("using System.Threading.Tasks;");
        builder.AppendLine("using BotFramework.Abstractions;");
        builder.AppendLine("using Telegram.Bot.Requests;");
        builder.AppendLine("using Telegram.Bot.Types;");
        builder.AppendLine("using Telegram.Bot.Types.Enums;");
        builder.AppendLine("using Telegram.Bot.Types.InlineQueryResults;");
        builder.AppendLine("using Telegram.Bot.Types.Payments;");
        builder.AppendLine("using Telegram.Bot.Types.ReplyMarkups;");
        builder.AppendLine();
        builder.AppendLine("namespace BotFramework.Extensions;");
        builder.AppendLine();
        builder.AppendLine("public static class ClientSendExtensions");
        builder.AppendLine("{");


        foreach (var type in requestClasses)
        {
            if (type.Name.Contains("Parameterless"))
            {
                continue;
            }

            var name       = type.Name.Replace("Request", "Async");
            var returnType = type.BaseType.GetGenericArguments()[0].Name;
            var contParams = type.GetConstructors()
                                 .First()
                                 .GetParameters()
                                 .ToList();
            var contArgs = contParams.Select(p => p.Name == "chatId" ? "chatId ?? client.UserId" : p.Name);

            var properties = type.GetProperties().Where(a => a.CanWrite).ToList();

            builder.Append($"public static async Task<{returnType}> {name}(\n");
            var methodInputs = new List<string>() { "this IClient client" };
            methodInputs.AddRange(contParams.Where(a => !IsChatId(a.Name))
                                            .Select(p =>
                                            $"{GetTypeName(p.ParameterType)} {(VariableName(p.Name))}"));
            var chatIdParam = contParams.FirstOrDefault(p => IsChatId(p.Name));
            if (chatIdParam != null)
            {
                methodInputs.Add($"{GetTypeName(chatIdParam.ParameterType)}? chatId = default");
            }

            methodInputs.AddRange(properties.Select(p => $"{GetTypeName(p.PropertyType)} {VariableName(p.Name)} = default"));
            methodInputs.Add("CancellationToken cancellationToken = default");
            builder.AppendJoin(",\n", methodInputs);
            // builder.AppendJoin(",\n", contParams.Select(p => $"{GetTypeName(p.ParameterType)} {p.Name} = default"));
            // builder.Append(",");
            // builder.AppendJoin(",\n", properties.Select(p => $"{GetTypeName(p.PropertyType)} {VariableName(p.Name)} = default"));
            // builder.AppendLine(",\nCancellationToken cancellationToken = default");
            builder.AppendLine(") =>");
            builder.AppendLine($"    await client.MakeRequest(new {type.Name}({string.Join(", ", contArgs)})");
            builder.AppendLine("{");
            builder.AppendJoin(",\n", properties.Select(p => $"{p.Name} = {VariableName(p.Name)}"));
            builder.AppendLine("}");
            builder.AppendLine(", cancellationToken);\n");
        }

        builder.AppendLine("}");
        var path = @"D:\source\telegram bots\BotFramework\BotFramework\Extensions\newss.cs";
        File.WriteAllText(path, builder.ToString());

        string VariableName(string name) => name.ToLower()[0] + name[1..];

        string GetTypeName(Type type)
        {
            if (type.Name == "IEnumerable`1")
            {
                return $"IEnumerable<{type.GenericTypeArguments[0].Name}>";
            }

            if (type.Name == "Nullable`1")
            {
                return $"{type.GenericTypeArguments[0].Name}?";
            }

            return type.Name;
        }

        bool IsChatId(string parameterInfo) => parameterInfo.Equals("chatId", StringComparison.OrdinalIgnoreCase);
    }


    //     public static async Task<Message> SendMessageAsync(this IClient      client,
    //                                                       string            text,
    //                                                       ChatId?           chatId                = default,
    //                                                       ParseMode?        parseMode             = null,
    //                                                       bool              disableWebPagePreview = default,
    //                                                       bool              disableNotification   = default,
    //                                                       int               replyToMessageId      = default,
    //                                                       IReplyMarkup?     replyMarkup           = default,
    //                                                       CancellationToken cancellationToken     = default
    //     ) =>
    //     await client.MakeRequest(new SendMessageRequest(chatId ?? client.UserId, text)
    //     {
    //         ParseMode             = parseMode,
    //         DisableWebPagePreview = disableWebPagePreview,
    //         DisableNotification   = disableNotification,
    //         ReplyToMessageId      = replyToMessageId,
    //         ReplyMarkup           = replyMarkup
    //     }, cancellationToken);
    public static void Main() { }
}