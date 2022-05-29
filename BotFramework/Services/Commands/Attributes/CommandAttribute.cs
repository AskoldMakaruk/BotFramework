using System;
using BotFramework.Abstractions;
using BotFramework.Extensions;
using System.Text.RegularExpressions;
using JetBrains.Annotations;

namespace BotFramework.Services.Commands.Attributes;

[AttributeUsage(AttributeTargets.Method | AttributeTargets.Class, AllowMultiple = true)]
public class CommandAttribute : CommandAttributeBase
{
    private Regex? _regex;

    public CommandAttribute(
        string?                command    = null,
        string?                startsWith = null,
        [RegexPattern] string? regex      = null,
        string?                endsWith   = null,
        string?                helpText   = null,
        string?                callback   = null,
        EndpointPriority       priority   = default)
    {
        StartsWith = startsWith;
        EndsWith   = endsWith;
        Regex      = regex;
        HelpText   = helpText;
        Callback   = callback;
        Priority   = priority;
        Command    = command;
    }

    public string? Command    { get; }
    public string? StartsWith { get; }
    public string? EndsWith   { get; }
    public string? HelpText   { get; }
    public string? Callback   { get; }

    [RegexPattern] public string? Regex { get; }

    public override bool? Suitable(UpdateContext context)
    {
        var update = context.Update;
        if (Command != null)
        {
            return update.GetText()?.Equals(Command) ?? false;
        }

        if (StartsWith != null)
        {
            return update.StartsWith(StartsWith);
        }

        if (EndsWith != null)
        {
            return update.EndsWith(EndsWith);
        }

        if (Regex != null)
        {
            _regex ??= new Regex(Regex);

            return _regex.IsMatch(update.GetText() ?? "");
        }

        if (Callback != null)
        {
            return update.CallbackQuery?.Data?.StartsWith(Callback) ?? false;
        }

        return null;
    }
}

public class PriorityAttribute : CommandAttribute
{
    public PriorityAttribute(EndpointPriority priority) : base(priority: priority) { }
}

public class RegexAttribute : CommandAttribute
{
    public RegexAttribute([RegexPattern] string regex) : base(regex: regex) { }
}

public class StartsWithAttribute : CommandAttribute
{
    public StartsWithAttribute(string startsWith) : base(startsWith: startsWith) { }
}

public class EndsWithAttribute : CommandAttribute
{
    public EndsWithAttribute(string endsWith) : base(endsWith: endsWith) { }
}

public class HelpTextAttribute : CommandAttribute
{
    public HelpTextAttribute(string helpText) : base(helpText: helpText) { }
}

public class CallbackAttribute : CommandAttribute
{
    public CallbackAttribute(string callback) : base(callback: callback) { }
}