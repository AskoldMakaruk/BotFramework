using Microsoft.Data.Sqlite;
using TelegramBotCore.DB.Controllers;

namespace TelegramBotCore.DB
{
    public static class Controller
    {
        public static SqliteConnection Connection;

        public static void Start(string path)
        {
            Connection = new SqliteConnection($@"Data Source = {path};");
            Connection.Open();
            AccountsController = new AccountsController(Connection);
          
        }
        public static AccountsController AccountsController;
    }
}
