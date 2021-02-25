using System;

namespace BotFramework
{
    public class WrappedServiceProvider
    {
        public WrappedServiceProvider(IServiceProvider provider)
        {
            Provider = provider;
        }

        public IServiceProvider Provider { get; set; }
    }
}