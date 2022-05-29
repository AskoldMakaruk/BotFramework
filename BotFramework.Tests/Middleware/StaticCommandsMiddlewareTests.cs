using System;
using System.Linq;
using BotFramework.Abstractions;
using BotFramework.Extensions.Hosting;
using BotFramework.Middleware;
using FluentAssertions;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using NUnit.Framework;

namespace BotFramework.Tests.Middleware;

public class StaticCommandsMiddlewareTests
{
    private IHost _host;

    [SetUp]
    public void Setup()
    {
        _host = Host.CreateDefaultBuilder()
                    .ConfigureHostConfiguration(builder => builder.AddEnvironmentVariables())
                    .UseBotFramework((app, _) =>
                    {
                        app.UseStaticCommands();
                        //app.UseHandlers();
                    }, true)
                    .Build();
    }

    private Type[] commandTypes =
    {
        typeof(StateMachineTests.StatefullCommand)
    };

    // [Test]
    // public void GetStaticCommands_WhenExecuted_ShouldReturnStaticCommands()
    // {
    //     var commands = StaticCommandsDIExtensions.GetAssemblies().Types.ToList();
    //     commands.Should().Contain(commandTypes);
    // }

    [Test]
    public void ServicesShouldContainStaticCommands()
    {
        var commands = _host.Services.GetService<IServiceCollection>()!.Where(a => a.ServiceType == typeof(ICommand))
                            .Select(a => a.ImplementationType)
                            .ToList();
        commands.Should().Contain(commandTypes);
    }
}