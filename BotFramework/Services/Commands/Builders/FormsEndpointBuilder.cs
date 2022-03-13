using System;
using System.Collections.Generic;
using System.ComponentModel.DataAnnotations;
using System.Linq;
using System.Reflection;
using System.Threading.Tasks;
using BotFramework.Abstractions;
using BotFramework.Services.Commands.Attributes;

namespace BotFramework.Middleware;

public record FormTypes(IReadOnlyList<Type> Types);

public class FormsEndpointBuilder : IEndpoitBuilder
{
    private List<FormEndpointCommand> _formEndpointCommands;

    public FormsEndpointBuilder(FormTypes types)
    {
        _formEndpointCommands = types.Types.SelectMany(GetControllerCommands).ToList();
    }

    public IEnumerable<Endpoint> Get()
    {
        return null;
    }

    private IEnumerable<FormEndpointCommand> GetControllerCommands(Type controllerType)
    {
        var methods = controllerType.GetMethods(BindingFlags.Public | BindingFlags.Instance);

        foreach (var method in methods)
        {
            var attrs = this.GetCommandAttributes(method).ToList();
            if (attrs.Count == 0)
            {
                continue;
            }

            var predicate = this.GetPredicate(attrs);

            yield return new FormEndpointCommand(predicate, controllerType);
        }
    }
}

public abstract class FormCommandBase<T>
{
    public abstract bool? IsSuitable(UpdateContext context);

    public abstract Task Execute(T formResult);
}

public interface IFormItemRepository
{
    public Task<List<FormItem>> GetUserItems(long userId);
}

public class FormField
{
    public Type                      ValueType  { get; set; }
    public object?                   Value      { get; set; }
    public string                    TextValue  { get; set; }
    public string                    Name       { get; set; }
    public List<ValidationAttribute> Attributes { get; set; }
}

public class Form
{
    public object Value;
    public Type   ValueType { get; }

    public List<FormField> Fields { get; }

    public Form()
    {
        ValueType = Value.GetType();

        var props = ValueType.GetProperties();
        Fields = props.Select(a => new FormField()
                      {
                          Attributes = GetAttributes(a),
                          Value      = a.GetValue(Value),
                          ValueType  = a.PropertyType,
                          Name       = a.Name
                      })
                      .ToList();
    }

    private static List<ValidationAttribute> GetAttributes(PropertyInfo a)
    {
        return a.GetCustomAttributes(true).OfType<ValidationAttribute>().ToList();
    }

    public void Assign(string name, string value)
    {
        var item = Fields.FirstOrDefault(a => a.Name == name);
        item.TextValue = value;
    }
}

public record FormItem
{
    public int    Id     { get; init; }
    public long   UserId { get; init; }
    public string Name   { get; set; }
    public string Value  { get; set; }
}

[IgnoreReflection]
internal class FormEndpointCommand : ICommand
{
    public string Name => ControllerType.Name;

    private readonly CommandPredicate _predicate;
    public readonly  Type             ControllerType;
    private readonly Func<bool>       ValidateForm;

    public FormEndpointCommand(CommandPredicate predicate, Type controllerType)
    {
        _predicate     = predicate;
        ControllerType = controllerType;
    }

    public Task Execute(UpdateContext context)
    {
        return Task.CompletedTask;
    }

    public bool? Suitable(UpdateContext context)
    {
        return _predicate.Invoke(context) ?? false;
    }
}