using System.Threading.Tasks;

namespace BotFramework.Abstractions
{
    public interface ICommand
    {
        Task Execute(IClient client);
    }
}