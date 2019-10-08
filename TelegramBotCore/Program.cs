using System;
using TelegramBotCore.Telegram;
using TelegramBotCore.Telegram.Bot;

namespace TelegramBotCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            var bot = new Client("{YOUR TOKEN HERE}");
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}