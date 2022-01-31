using System.Threading.Tasks;

namespace BotFramework.Abstractions
{
    public interface ICommand
    {
        public Task Execute(IClient client);
    }

    // public interface IStatelessCommand : ICommand
    // {
    //     Task ICommand.Execute(IClient       client) => Execute();
    //     public Task   Execute(UpdateContext context);
    // }
    //
    // public class StatelessCommand : IStatelessCommand
    // {
    //     public Task Execute(IRequestSinc, UpdateContext context)
    //     {
    //         
    //     }
    // }
    //
    // public interface IDialogCommand : ICommand
    // {
    //     public int State { init; }
    //
    //     public Task Execute(IClient client);
    // }
}