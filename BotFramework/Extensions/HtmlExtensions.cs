using System.Text;

namespace BotFramework.Extensions;

public static class HtmlExtensions
{
    public static string? Bold(this string? text)
    {
        return text == null ? null : $"<b>{text}</b>";
    }

    public static string? Italic(this string? text)
    {
        return text == null ? null : $"<i>{text}</i>";
    }

    public static string? Code(this string? text)
    {
        return text == null ? null : $"<code>{text}</code>";
    }

    public static string? Underline(this string? text)
    {
        return text == null ? null : $"<u>{text}</u>";
    }

    public static string? Strike(this string? text)
    {
        return text == null ? null : $"<s>{text}</s>";
    }

    public static string? Pre(this string? text)
    {
        return text == null ? null : $"<pre>{text}</pre>";
    }

    public static string? Pre(this string? text, string language)
    {
        return text == null ? null : $"<pre language=\"{language}\">{text}</pre>";
    }

    public static StringBuilder AppendBold(this StringBuilder builder, string text)
    {
        return builder.Append("<b>").Append(text).Append("</b>");
    }

    public static StringBuilder? AppendItalic(this StringBuilder builder, string text)
    {
        return builder.Append("<i>").Append(text).Append("</i>");
    }

    public static StringBuilder? AppendCode(this StringBuilder builder, string text)
    {
        return builder.Append("<code>").Append(text).Append("</code>");
    }

    public static StringBuilder? AppendUnderline(this StringBuilder builder, string text)
    {
        return builder.Append("<u>").Append(text).Append("</u>");
    }

    public static StringBuilder? AppendStrike(this StringBuilder builder, string text)
    {
        return builder.Append("<s>").Append(text).Append("</s>");
    }

    public static StringBuilder? AppendPre(this StringBuilder builder, string text)
    {
        return builder.Append("<pre>").Append(text).Append("</pre>");
    }

    public static StringBuilder? AppendPre(this StringBuilder builder, string text, string language)
    {
        return builder.Append("<pre language=\"").Append(language).Append("\">").Append(text).Append("</pre>");
    }
}