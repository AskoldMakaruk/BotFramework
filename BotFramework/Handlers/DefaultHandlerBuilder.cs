using System.Threading;

namespace BotFramework.Handlers
{
    public static class DefaultHandlerCreator
    {
        public static DictionaryInMemoryHandler BuildDictionaryInMemoryHandler(this HandlerConfiguration configuration)
        {
            return new DictionaryInMemoryHandler(configuration);
        }

        public static void RunInMemoryHandler(this HandlerConfiguration configuration)
        {
            var handler = new DictionaryInMemoryHandler(configuration);
            configuration.BotClient.OnUpdate += (_, args) => handler.Handle(args.Update);
            configuration.BotClient.StartReceiving();
            new ManualResetEvent(false).WaitOne();
        }
    }
}