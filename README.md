# BotFramework

[![package](https://img.shields.io/nuget/v/BotFramework)](https://www.nuget.org/packages/BotFramework) [![Nuget (with prereleases)](https://img.shields.io/nuget/vpre/BotFramework)](https://www.nuget.org/packages/BotFramework)

[![Nuget](https://img.shields.io/nuget/dt/BotFramework)](https://www.nuget.org/packages/BotFramework)

## Echobot

### Starting boilerplate:
```csharp
static void Main(string[] args)
    {
        using var host = Host.CreateDefaultBuilder(args)
                             .UseConfigurationWithEnvironment()                             
                             .ConfigureApp((app, context) =>
                             {
                                 app.Services.AddSingleton<ITelegramBotClient>(_ => 
                                    new TelegramBotClient(context.Configuration["BotToken"]));
                                 app.Services.AddTransient<IUpdateConsumer, Client>();                              
                                 app.UseHandlers();
                                 app.UseStaticCommands();
                             })
                             .Build()
                             .RunAsync();
        Console.ReadLine();
    }
```
## Commands
Command is a class that implements ```ICommand```. They are state-machines that can receive and send messages to telegram user using ```IClient```. Commands are user-scoped which means that injected services save their state between user's inputs.

### Lifecycle
BF keeps an instance of each command to call ```Suitable``` without creating new intance, **so you cannot use non-singleton injected services in your ```Suitable``` methods.** We plan to migrate this to static interface methods once .NET 6 is rolled out.

When a message from user comes in bf it will:
* Scan through all commands with ```SuitableFirst```;
* Search for already created command;
* Scan through all commands with ```SuitableLast```;
* Execute command or return incoming message as result of awaited ```GetUpdate```.

### IStaticCommand

Most commands you will write will implement ```IStaticCommand``` interface. They will be executed based on ```SuitableFirst/SuitableLast``` response. 
```csharp
public class EchoCommand : IStaticCommand 
{
    // It's last to prevent re-creation of a same command.
    public bool SuitableLast(Update context) => true;

    public async Task Execute(IClient client)
    {        
        // gets last text message. if none are available it will wait untill one arrives.
        var message = await client.GetTextMessage(); 

        // sending response back to user. user id is optional because command are user-scoped and bf already knows to whom he is supposed to respond.
        await client.SendTextMessage($"Hello, here your last message {message.Text}, type something again");  

        message = await client.GetTextMessage();

        await client.SendTextMessage($"And this is your new message {message.Text}, and now type only message with hello");
        
        await client.SendTextMessage("Well done!");
    }    
}   
```

### DI
All commands can be injected with services like this: 

```csharp
public class EchoCommand : IStaticCommand 
{
    private readonly ILogger logger;

    public EchoCommand(ILogger logger)
    {
        this.logger = logger;
    }    

    public async Task Execute(IClient client)
    {        
        // do your logger.Information("It works")
    }    
}   
```


