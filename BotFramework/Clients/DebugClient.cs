using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot.Requests.Abstractions;
using Telegram.Bot.Types;

namespace BotFramework.Clients
{
    public class DebugClient : IClient
    {
        private int      inputOffset;
        private int      outputOffset;
        public  object[] Resposes;
        public  Update[] UserInputs;

        public DebugClient(Update[] userInputs, object[] resposes)
        {
            Resposes   = resposes;
            UserInputs = userInputs;
        }

        public Task<TResponse> MakeRequest<TResponse>(IRequest<TResponse> request,
                                                      CancellationToken   cancellationToken = default)
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