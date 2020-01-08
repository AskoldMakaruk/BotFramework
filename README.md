# BotFramework

[![package](https://img.shields.io/nuget/v/BotFramework)](https://www.nuget.org/packages/BotFramework)
[![Nuget](https://img.shields.io/nuget/dt/BotFramework)](https://www.nuget.org/packages/BotFramework)

## Echobot

Initializing bot:
```csharp
class Program
    {
        static void Main()
        {
            new BotBuilder()
            .UseAssembly(typeof(Program).Assembly)
            .WithName("EchoBot")
            .WithToken("<YOUR TOKEN>")
            .UseLogger((c, m) => Console.WriteLine(m))
            .Build()
            .Run();
        }
    }
```
Create command to handle messages:
```csharp
public class EchoCommand : MessageCommand
    {
        public override Response Execute(Message message, Client client)
        {
            return new Response().AddMessage(new TextMessage(message.Chat.Id, message.Text));
        }

        public override bool Suitable(Message message) => true; //accept any message
    }
```
