using System;
using System.Threading;
using BotFramework.Services;
using FluentCache.Simple;
using Microsoft.Extensions.Options;
using NUnit.Framework;

namespace BotFramework.Tests;

public class UserStorageTests
{
    [SetUp]
    public void Setup() { }

    [Test]
    public void Test()
    {
        var userstorage = new UserScopeStorage(new FluentDictionaryCache(),
            new OptionsWrapper<UserScopeStorageOptions>(new() { ScopeLifetime = TimeSpan.FromSeconds(3) }));

        userstorage.Get(1);
        Thread.Sleep(2500);
        userstorage.Get(1);
        Thread.Sleep(2500);
        userstorage.Get(1);
    }
}