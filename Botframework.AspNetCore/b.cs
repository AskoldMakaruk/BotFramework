// using Microsoft.Extensions.DependencyInjection;
// using Microsoft.AspNetCore.Hosting;
// using Microsoft.Extensions.Logging;
//
//
// namespace Serilog
// {
//     /// <summary>
//     /// Extends <see cref="IWebHostBuilder"/> with Serilog configuration methods.
//     /// </summary>
//     public static class BotFrameworkWebHostBuilderExtensions
//     {
//         /// <summary>
//         /// Use BotFramework with webhook.
//         /// </summary>
//         /// <param name="builder"></param>
//         /// <param name="logger"></param>
//         /// <param name="dispose"></param>
//         /// <param name="providers"></param>
//         /// <returns></returns>
//         /// <exception cref="ArgumentNullException"></exception>
//         public static IWebHostBuilder UseSerilog(
//             this IWebHostBuilder builder, 
//             ILogger logger = null, 
//             bool dispose = false,
//             LoggerProviderCollection providers = null)
//         {
//             if (builder == null) throw new ArgumentNullException(nameof(builder));
//
//             builder.ConfigureServices(collection =>
//             {
//                 if (providers != null)
//                 {
//                     collection.AddSingleton<ILoggerFactory>(services =>
//                     {
//                         var factory = new SerilogLoggerFactory(logger, dispose, providers);
//
//                         foreach (var provider in services.GetServices<ILoggerProvider>())
//                             factory.AddProvider(provider);
//
//                         return factory;
//                     });
//                 }
//                 else
//                 {
//                     collection.AddSingleton<ILoggerFactory>(services => new SerilogLoggerFactory(logger, dispose));
//                 }
//
//                 ConfigureServices(collection, logger);
//             });
//
//             return builder;
//         }
//
//         /// <summary>Sets Serilog as the logging provider.</summary>
//         /// <remarks>
//         /// A <see cref="WebHostBuilderContext"/> is supplied so that configuration and hosting information can be used.
//         /// The logger will be shut down when application services are disposed.
//         /// </remarks>
//         /// <param name="builder">The web host builder to configure.</param>
//         /// <param name="configureLogger">The delegate for configuring the <see cref="LoggerConfiguration" /> that will be used to construct a <see cref="Serilog.Core.Logger" />.</param>
//         /// <param name="preserveStaticLogger">Indicates whether to preserve the value of <see cref="Serilog.Log.Logger"/>.</param>
//         /// <param name="writeToProviders">By default, Serilog does not write events to <see cref="ILoggerProvider"/>s registered through
//         /// the Microsoft.Extensions.Logging API. Normally, equivalent Serilog sinks are used in place of providers. Specify
//         /// <c>true</c> to write events to all providers.</param>
//         /// <returns>The web host builder.</returns>
//         public static IWebHostBuilder UseSerilog(
//             this IWebHostBuilder builder,
//             Action<WebHostBuilderContext, LoggerConfiguration> configureLogger,
//             bool preserveStaticLogger = false,
//             bool writeToProviders = false)
//         {
//             if (builder == null) throw new ArgumentNullException(nameof(builder));
//             if (configureLogger == null) throw new ArgumentNullException(nameof(configureLogger));
//
//             builder.ConfigureServices((context, collection) =>
//             {
//                 var loggerConfiguration = new LoggerConfiguration();
//
//                 LoggerProviderCollection loggerProviders = null;
//                 if (writeToProviders)
//                 {
//                     loggerProviders = new LoggerProviderCollection();
//                     loggerConfiguration.WriteTo.Providers(loggerProviders);
//                 }
//
//                 configureLogger(context, loggerConfiguration);
//                 var logger = loggerConfiguration.CreateLogger();
//                 
//                 ILogger registeredLogger = null;
//                 if (preserveStaticLogger)
//                 {
//                     registeredLogger = logger;
//                 }
//                 else
//                 {
//                     // Passing a `null` logger to `SerilogLoggerFactory` results in disposal via
//                     // `Log.CloseAndFlush()`, which additionally replaces the static logger with a no-op.
//                     Log.Logger = logger;
//                 }
//
//                 collection.AddSingleton<ILoggerFactory>(services =>
//                 {
//                     var factory = new SerilogLoggerFactory(registeredLogger, true, loggerProviders);
//
//                     if (writeToProviders)
//                     {
//                         foreach (var provider in services.GetServices<ILoggerProvider>())
//                             factory.AddProvider(provider);
//                     }
//
//                     return factory;
//                 });
//
//                 ConfigureServices(collection, logger);
//             });
//             return builder;
//         }
//         
//         static void ConfigureServices(IServiceCollection collection, ILogger logger)
//         {
//             if (collection == null) throw new ArgumentNullException(nameof(collection));
//
//             if (logger != null)
//             {
//                 // This won't (and shouldn't) take ownership of the logger. 
//                 collection.AddSingleton(logger);
//             }
//
//             // Registered to provide two services...
//             var diagnosticContext = new DiagnosticContext(logger);
//
//             // Consumed by e.g. middleware
//             collection.AddSingleton(diagnosticContext);
//
//             // Consumed by user code
//             collection.AddSingleton<IDiagnosticContext>(diagnosticContext);
//         }
//     }
// }