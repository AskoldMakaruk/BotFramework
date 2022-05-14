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
}