using System.Threading.Tasks;

namespace BotFramework.Abstractions;

public interface ICommand
{
    public Task Execute(UpdateContext context);

    /// <summary>
    ///     If this returns <c>true</c>, instance of this command will be created and executed. Any other <see cref="Suitable" /> command will be discarded.
    /// </summary>
    /// <remarks>
    ///     It must be thread-save and have access only to static members.
    /// </remarks>
    public bool? Suitable(UpdateContext context);
}