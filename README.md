# BotFramework

![Nuget](https://img.shields.io/nuget/v/BotFramework )
![Nuget](https://img.shields.io/nuget/dt/BotFramework)

## Echobot

Initializing bot:
```csharp
class Program
    {
        static void Main(string[] args)
        {
            new BotBuilder()
                .UseAssembly(typeof(Program).Assembly)
                .WithName("EchoBot")
                .WithToken(Token)
                .UseLogger((c, m) => Console.WriteLine(m))
                .Build()
                .Run();
        }
    }
```
Create command to handle messages:
```csharp
public class EchoCommand : StaticMessageCommand
    {
        public override Response Execute(Message message, Client client)
        {
            return new Response().SendTextMessage(message.Chat.Id, message.Text);
        }

        public override bool Suitable(Message message) => true; //accept any message
    }
```
