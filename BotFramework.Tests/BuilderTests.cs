using System.Reflection;
using BotFramework.Commands;
using BotFramework.Responses;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class Tests
    {
        private BotBuilder builder;
        private Assembly   assembly;

        [SetUp]
        public void Setup()
        {
            builder  = new BotBuilder().WithToken(new string('0', 9) + ":" + new string('0', 35));
            assembly = Assembly.GetAssembly(GetType());
        }

        [Test]
        public void Build_StaticCommands_1()
        {
            builder
            .WithStaticCommands(new SomeCommand())
            .Build();

            Assert.Pass();
        }

        [Test]
        public void Build_StaticCommands_2()
        {
            builder
            .UseAssembly(assembly)
            .Build();

            Assert.Pass();
        }

        [Test]
        public void Build_StartCommand()
        {
            builder
            .WithStartCommands(new SomeCommand())
            .Build();

            Assert.Pass();
        }

        [Test]
        public void Build_InlineCommand()
        {
            builder.WithStaticCommands(CommandFactory.CreateCommand(
                (u, client) => new Response(),
                u => true));
        }
    }

    [StaticCommand]
    public class SomeCommand : MessageCommand
    {
        public override Response Execute(Message message, IGetOnlyClient client)
        {
            return new Response().AddMessage(new TextMessage(0, ""));
        }

        public override bool Suitable(Message message) => message.Text == "true";
    }
}