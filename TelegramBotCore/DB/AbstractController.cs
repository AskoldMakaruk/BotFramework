using Microsoft.Data.Sqlite;

namespace TelegramBotCore.DB
{
    public abstract class AbstractController
    {
        protected SqliteConnection Connection;
        protected SqliteType Integer = SqliteType.Integer;
        protected SqliteType Text = SqliteType.Text;
        protected SqliteType Real = SqliteType.Real;
        protected SqliteType Blob = SqliteType.Blob;

        protected abstract string Name { get; }
        protected abstract Column[] Columns { get; }

        protected AbstractController(SqliteConnection connection)
        {
            Connection = connection;
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{Name}';";
            var reader = cmd.ExecuteReader();
            while (reader.Read()) return;
            cmd = connection.CreateCommand();

            string text = $"CREATE TABLE {Name}(";
            foreach (var column in Columns)
                text += $" {column.Name} {column.Params}, ";
            text = text.Substring(0, text.Length - 2);
            text += ");";

            cmd.CommandText = text;
            cmd.ExecuteNonQuery();
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

    public struct Column
    {
        public string Name;
        public string Params;

        public Column(string name, string @params)
        {
            Name = name;
            Params = @params;
        }
    }
}
