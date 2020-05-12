using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Text;
using BotFramework.Bot;
using BotFramework.Commands;
using BotFramework.Commands.Injectors;
using BotFramework.Commands.Validators;
using BotFramework.Responses;
using NUnit.Framework;
using Optional;
using Telegram.Bot.Types;

namespace BotFramework.Tests
{
    public class Benchmark
    {
        private CompilerInjector compilerInjector;
        private ReflectionInjector reflInjector;

        Update update => new Update
        {
            Message = new Message
            {
                Text = "hello",
                Chat = new Chat
                {
                    Id = 1
                }
            }
        };

        private readonly int                                            benchScale = 1_000_000;
        private          Func<Update, IGetOnlyClient, Option<ICommand>> func;

        [SetUp]
        public void Setup()
        {
            var validators = new Dictionary<Type, Type>
            {
                [typeof(Message)] = typeof(MessageValidator), [typeof(HelloMessage)] = typeof(HelloCommandValidator)
            };
            compilerInjector = new CompilerInjector(validators);
            reflInjector = new ReflectionInjector(validators);

            func = Compiled();
            func(update, null);
        }

        [Test]
        public void TestReflectionInjector()
        {
            for (int i = 0; i < benchScale; i++)
                _ = reflInjector.GetPossible(new[] {typeof(EchoCommand)}, update, null).Select(t => t.Execute()).ToList();
        }

        [Test]
        public void TestStupid()
        {
            for (int i = 0; i < benchScale; i++)
            {
                _ = new[] {new EchoCommandStupid(update)}.Select(t => t.Execute()).ToList();
            }
        }

        [Test]
        public void TestCompilerInjector()
        {
            for (int i = 0; i < benchScale; i++)
                _ = compilerInjector.GetPossible(new[] {typeof(EchoCommand)}, update, null).Select(t => t.Execute()).ToList();
        }

        [Test]
        public void TestFuncPreCompiled()
        {
            Func<Update, IGetOnlyClient, Option<ICommand>> f = (update, client) => new MessageValidator(update)
                                                                                   .Validate()
                                                                                   .FlatMap(mes =>
                                                                                   (new EchoCommand(mes) as ICommand).Some());
            for (int i = 0; i < benchScale; i++)
                _ = f(update, null).Map(t => t.Execute());
        }

        [Test]
        public void TestFunc()
        {
            for (int i = 0; i < benchScale; i++)
                _ = func(update, null).Map(t => t.Execute());
        }


        [Test]
        public void TestCompiling() =>
            Compiled();

        private Func<Update, IGetOnlyClient, Option<ICommand>> Compiled()
        {
            var func = compilerInjector.CompileCommand(typeof(EchoCommand));
            Console.WriteLine(func.ToString());
            Console.WriteLine(func(update, null).HasValue);
            return func;
        }

        public class HelloMessage
        {
            public Message Message;
            public HelloMessage(Message m) => Message = m;
        }


        public class HelloCommandValidator : Validator<HelloMessage>
        {
            private readonly Message message;

            public Option<HelloMessage> Validate() =>
            new HelloMessage(message).SomeWhen(t => message.Text == "hello");

            public HelloCommandValidator(Message message) => this.message = message;
        }

        public class EchoCommand : ICommand
        {
            private readonly Message message;

            public Response Execute()
            {
                return new Response().AddMessage(new TextMessage(message.Chat.Id, message.Text));
            }

            public EchoCommand(Message message) => this.message = message;
        }

        public class EchoCommandStupid : ICommand
        {
            private readonly Message message;

            public Response Execute()
            {
                if (message.Text == "hello")
                    return new Response().AddMessage(new TextMessage(message.Chat.Id, message.Text));
                else return new Response();
            }

            public EchoCommandStupid(Update message) => this.message = message.Message;
        }
    }
}