using System.Threading.Tasks;
using BotFramework.HostServices;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class AppUpdateProducer
    {
        public Task FromUser(Update  update)  => _debugDelegateWrapper.App(update);
        public Task FromUser(Message message) => FromUser(new Update { Message = message });

        private readonly AppRunnerServiceExtensions.DebugDelegateWrapper _debugDelegateWrapper;

        public AppUpdateProducer(AppRunnerServiceExtensions.DebugDelegateWrapper debugDelegateWrapper)
        {
            _debugDelegateWrapper = debugDelegateWrapper;
        }
    }
}