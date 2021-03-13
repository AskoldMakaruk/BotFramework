using System;
using Microsoft.Extensions.DependencyInjection;

namespace BotFramework
{
    public class WrappedService<T>
    {
        public WrappedService(T service)
        {
            Service = service;
        }

        public T Service { get; set; }
    }

    public static class WrappedService
    {
        public static void AddWrappedScoped<T>(this IServiceCollection provider, Func<IServiceProvider, T> factory)
        where T : class
        {
            provider.AddScoped(t => new WrappedService<T>(factory(t)));
            provider.AddTransient(t => t.GetService<WrappedService<T>>()!.Service);
        }

        public static T GetWrappedService<T>(this IServiceProvider provider) where T : class
        {
            var service = provider.GetService<WrappedService<T>>();
            if (service is null)
            //todo normal exceptions
                throw new Exception("Not found");
            return service.Service;
        }

        public static void SetWrappedService<T>(this IServiceProvider provider, T newService) where T : class
        {
            var service = provider.GetService<WrappedService<T>>();
            if (service is null)
            //todo normal exceptions
                throw new Exception("Not found");
            service.Service = newService;
        }

        public static void SetWrappedService(this IServiceProvider provider, Type serviceType, object newService)
        {
            var service = provider.GetService(typeof(WrappedService<>).MakeGenericType(serviceType));
            if (service is null)
            //todo normal exceptions
                throw new Exception("Not found");
            dynamic dService = service;
            ref dynamic d    = ref dService.Service;
            d                = newService;
        }
    }
}