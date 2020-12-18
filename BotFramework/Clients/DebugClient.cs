using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class DebugClient : IClient
    {
        public Update[] UserInputs;
        public object[] Resposes;

        public DebugClient(Update[] userInputs, object[] resposes)
        {
            Resposes   = resposes;
            UserInputs = userInputs;
        }

        private int inputOffset  = 0;
        private int outputOffset = 0;

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
                                                                 CancellationToken cancellationToken = default(CancellationToken))
        {
            var res = (TResponse) Resposes[outputOffset++];
            return Task.FromResult(res);
        }

        public ValueTask<Update> GetUpdate(Func<Update, bool>? filter = null, Action<Update>? onFilterFail = null)
        {
            return ValueTask.FromResult(UserInputs[inputOffset++]);
        }

        public long UserId => 0;
    }
}