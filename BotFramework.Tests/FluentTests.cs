using BotFramework.Bot;
using BotFramework.Commands;
using BotFramework.Responses;
using FluentAssertions;
using NUnit.Framework;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class FluentTests
    {
        [Test]
        public void Test()
        {
            AssertionOptions.AssertEquivalencyUsing(options =>
            options.Using<ChatId>(ctx => ctx.Subject.ToString().Should().BeEquivalentTo(ctx.Expectation.ToString()))
                   .WhenTypeIs<ChatId>());
            var resp1 = new Response()
                        .AddMessage(new EditTextMessage(123, 1, "Bar"))
                        .AddMessage(new TextMessage(229, "Foo"));
            var resp2 = new Response()
                        .AddTextMessage(229, "Foo")
                        .AddMessage(new EditTextMessage(123, 1, "Bar"));
            var resp3 = new Response()
                        .AddTextMessage(12345679, "Foo")
                        .AddMessage(new EditTextMessage(123, 1, "Bar"));
            var resp4 = new Response()
                        .AddMessage(new EditTextMessage(123, 1, "Bar"))
                        .AddMessage(new TextMessage(229, "Foo"))
                        .UsePrevious(true);
            var resp5 = new Response(new SomeCommand())
                        .AddMessage(new EditTextMessage(123, 1, "Bar"))
                        .AddMessage(new TextMessage(229, "Foo"))
                        .UsePrevious(true);
            var resp6 = new Response(new GetTaskCommand("Some text"))
                        .AddMessage(new EditTextMessage(123, 1, "Bar"))
                        .AddMessage(new TextMessage(229, "Foo"))
                        .UsePrevious(true);
            resp1.Should().BeEquivalentTo(resp2, options => options.Including(t => t.Responses));
            resp1.Should().NotBeEquivalentTo(resp3, options => options.Including(t => t.Responses));
            resp1.Should()
                 .NotBeEquivalentTo(resp4, options => options.Including(t => t.Responses).Including(t => t.UsePreviousCommands));
            resp1.Should().NotBeEquivalentTo(resp4, options => options.IncludingProperties().IncludingFields());
            resp1.Should()
                 .NotBeEquivalentTo(resp3,
                     options => options.Including(t => t.Responses)
                                       .Including(t => t.NextPossible)
                                       .Including(t => t.UsePreviousCommands));
            resp6.Should()
                 .NotBeEquivalentTo(resp5,
                     options => options.Including(t => t.Responses)
                                       .Including(t => t.NextPossible)
                                       .Including(t => t.UsePreviousCommands));
        }

        [Test]
        public void TestCoomands()
        {
            AssertionOptions.AssertEquivalencyUsing(options =>
            options.Using<ChatId>(ctx => ctx.Subject.ToString().Should().BeEquivalentTo(ctx.Expectation.ToString()))
                   .WhenTypeIs<ChatId>());
            var arr1 = new ICommand[] {new GetTaskCommand("acc"), new StartCommand()};
            var arr2 = new ICommand[] {new StartCommand(), new GetTaskCommand("foo"),};
            var arr3 = new ICommand[] {new StartCommand(), new GetTaskCommand("acc"),};
            arr1.Should().NotBeEquivalentTo(arr2);
            arr1.Should().BeEquivalentTo(arr3);
            ((StartCommand) arr3[0]).account = "Shit";
            arr1.Should().NotBeEquivalentTo(arr3);
            var arr4 = new ICommand[] {new GetTaskCommand("Foo"), new GetTaskCommand("Bar"), };
            var arr5 = new ICommand[] {new StartCommand("Foo"), new StartCommand("Bar"), };
            arr4.Should().NotBeEquivalentTo(arr5);
            new GetTaskCommand("Res").Should().NotBeEquivalentTo(new SomeCommand());
        }
    }

    class StartCommand : MessageCommand
    {
        public string account;

        public override bool Suitable(Message message)
        {
            throw new System.NotImplementedException();
        }

        public override Response Execute(Message message, IGetOnlyClient client)
        {
            throw new System.NotImplementedException();
        }

        public StartCommand()
        {
            
        }

        public StartCommand(string account)
        {
            this.account = this.account;
        }
    }

    class GetTaskCommand : MessageCommand
    {
        public override bool Suitable(Message message)
        {
            throw new System.NotImplementedException();
        }

        public override Response Execute(Message message, IGetOnlyClient client)
        {
            throw new System.NotImplementedException();
        }

        public string account;

        public GetTaskCommand(string acc)
        {
            account = acc;
        }
    }
}