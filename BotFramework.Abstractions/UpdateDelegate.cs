using System.Threading.Tasks;
using Telegram.Bot.Types;

namespace BotFramework.Abstractions;

public delegate Task UpdateDelegate(UpdateContext update);