using System;
using System.Collections.Generic;

namespace BotFramework.Middleware;

public record StaticCommandsList(IReadOnlyList<Type> Types);