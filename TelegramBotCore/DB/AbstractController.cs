using Microsoft.Data.Sqlite;
using System.Text;

namespace TelegramBotCore.DB
{
    public abstract class AbstractController
    {
        protected SqliteConnection Connection;
        protected SqliteType Integer = SqliteType.Integer;
        protected SqliteType Text = SqliteType.Text;
        protected SqliteType Real = SqliteType.Real;
        protected SqliteType Blob = SqliteType.Blob;

        protected SqliteParam Unique = SqliteParam.Unique;
        protected SqliteParam NotNull = SqliteParam.Not_Null;
        protected SqliteParam PrimaryKey = SqliteParam.Primary_Key;

        protected abstract string Name { get; }
        protected abstract Column[] Columns { get; }

        protected AbstractController(SqliteConnection connection)
        {
            Connection = connection;
            var cmd = connection.CreateCommand();
            cmd.CommandText = $"SELECT name FROM sqlite_master WHERE type = 'table' AND name = '{Name}';";
            var reader = cmd.ExecuteReader();
            if (reader.Read()) return;

            cmd = connection.CreateCommand();
            StringBuilder text = new StringBuilder($"CREATE TABLE {Name}(");
            string key = "";
            for (var i = 0; i < Columns.Length; i++)
            {
                var column = Columns[i];
                text.Append($" {column.Name} {column.Type}");
                foreach (var param in column.Params)
                {
                    string p = param.ToString().Replace("_", " ");
                    if (param != SqliteParam.Primary_Key)
                        text.Append(p);
                    else
                    {
                        key = p + $"('{column.Name}')";
                    }
                }
                if (i + 1 < Columns.Length) text.Append(",");
                if (i + 1 == Columns.Length && key != "") text.Append(",");
            }
            text.Append(key);
            text.Append(");");

            cmd.CommandText = text.ToString();
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

    public enum SqliteParam
    {
        Unique, Not_Null, Primary_Key
    }

    public struct Column
    {
        public string Name;
        public SqliteType Type;
        public SqliteParam[] Params;
        public Column(string name, SqliteType type, params SqliteParam[] param)
        {
            Name = name;
            Type = type;
            Params = param;
        }
    }
}
