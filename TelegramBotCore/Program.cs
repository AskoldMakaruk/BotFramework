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
            var bot = new Bot("494598173:AAEgnATNyRt7fqecoVGwWmIGswKQyfm2NgY");
            while (true)
            {
                Console.ReadLine();
            }
        }
    }
}
