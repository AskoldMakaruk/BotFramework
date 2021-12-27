using System.Threading.Tasks;

namespace BotFramework.Abstractions
{
    public interface IEndpoint
    {
        public EndpointPriority           Priority                   { get; }
        public Task                       Action                     { get; }
    }
}