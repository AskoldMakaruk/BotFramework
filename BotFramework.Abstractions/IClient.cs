namespace BotFramework.Abstractions
{
    public interface IClient : IRequestSinc, IUpdateQueue
    {
        /// <summary>
        ///     Identifier of user. Each user has his/her own <see cref="IClient" />
        /// </summary>
        long UserId { get; }
    }
}