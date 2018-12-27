using Microsoft.Data.Sqlite;

namespace TelegramBotCore.DB
{
    public abstract class AbstractController
    {
        protected SqliteConnection Connection;
        protected SqliteType Integer = SqliteType.Integer;
        protected SqliteType Text = SqliteType.Text;
        protected SqliteType Real = SqliteType.Real;
      
        protected AbstractController(SqliteConnection connection)
        {
            Connection = connection;
        }

        protected int Parse(object stringToParse)
        {
            if (string.IsNullOrEmpty(stringToParse.ToString())) return 0;
            return int.Parse(stringToParse.ToString());
        }
        protected double ParseDouble(object stringToParse)
        {
            if (string.IsNullOrEmpty(stringToParse.ToString())) return 0;
            return double.Parse(stringToParse.ToString());
        }
    }
}
