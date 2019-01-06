using Microsoft.Data.Sqlite;

namespace TelegramBotCore.DB.Controllers
{
    public class MessageController : AbstractController
    {
        public MessageController(SqliteConnection connection) : base(connection)
        {
        }

        protected override string Name => "MessageTexts";

        protected override Column[] Columns => new[]
        {
            new Column("Key", Text),
            new Column("Value", Text),
        };
        public string GetTextByKey(string key)
        {
            var comm = Connection.CreateCommand();
            comm.CommandText = @"
            SELECT *
            FROM MessageTexts
            WHERE Key = @id;";

            comm.Parameters.Add("@id", SqliteType.Integer).Value = key;
            var reader = comm.ExecuteReader();
            if (reader.Read()) return reader["Value"].ToString();

            SetText(key, key);
            return key;
        }
        public void SetText(string key, string value)
        {
            var comm = Connection.CreateCommand();
            comm.CommandText = @"
            SELECT *
            FROM MessageTexts 
            WHERE Key = @id;";

            comm.Parameters.Add("@id", Text).Value = key;
            var reader = comm.ExecuteReader();
            if (reader.Read())
            {
                comm = Connection.CreateCommand();
                comm.CommandText = @"
                UPDATE MessageTexts SET
	            [Value] = @value,              
		        WHERE @key = key;";

            }
            else
            {
                comm = Connection.CreateCommand();
                comm.CommandText = @"
                INSERT INTO [MessageTexts]
                (Key, Value)
                VALUES (@key,@value);";
            }

            comm.Parameters.Add("@value", Text).Value = value;
            comm.Parameters.Add("@key", Text).Value = key;

            comm.ExecuteNonQuery();
        }
    }
}
