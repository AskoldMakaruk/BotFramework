using System;
using TelegramBotCore.DB;
using TelegramBotCore.Telegram;

namespace TelegramBotCore
{
    internal class Program
    {
        private static void Main(string[] args)
        {
            Controller.Start("database.db");
            var bot = new Bot("{YOUR TOKEN HERE}");
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
