using Microsoft.Data.Sqlite;
using System.Collections.Generic;
using TelegramBotCore.Telegram;

namespace TelegramBotCore.DB.Controllers
{
    public class AccountsController : AbstractController
    {
        protected override string Name => "Accounts";

        protected override Column[] Columns => new[]
        {
            new Column("Id", Integer, PrimaryKey),
            new Column("ChatId", Integer),
            new Column("Name", Text),
        };
        public AccountsController(SqliteConnection connection) : base(connection)
        {
        }

        public Account GetAccountById(int id)
        {
            var comm = Connection.CreateCommand();
            comm.CommandText = @"
            SELECT *
            FROM Accounts 
            WHERE Accounts.Id = @id;";

            comm.Parameters.Add("@id", SqliteType.Integer).Value = id;
            Account a = null;
            var reader = comm.ExecuteReader();
            while (reader.Read())
            {
                a = new Account();
                a.Id = Parse(reader["Id"]);
                a.ChatId = Parse(reader["ChatId"]);
                a.Name = reader["Name"].ToString();
            }
            return a;
        }
        public Account GetAccountByChatId(long chatId)
        {
            var comm = Connection.CreateCommand();
            comm.CommandText = @"
            SELECT *
            FROM Accounts 
            WHERE Accounts.ChatId = @id;";

            comm.Parameters.Add("@id", SqliteType.Integer).Value = chatId;
            Account a = null;
            var reader = comm.ExecuteReader();
            while (reader.Read())
            {
                a = new Account();
                a.Id = Parse(reader["Id"]);
                a.ChatId = Parse(reader["ChatId"]);
                a.Name = reader["Name"].ToString();
            }
            return a;
        }
        public List<Account> GetList(Account account)
        {
            var comm = Connection.CreateCommand();
            comm.CommandText = @"
            SELECT *
            FROM Accounts 
            WHERE
                CASE WHEN @NamePar LIKE """"
                     THEN 1
                     ELSE @NamePar LIKE Accounts.Name
                END
            AND CASE WHEN  @Id = 0 
                     THEN 1 
                     ELSE @Id = Accounts.Id
                END                
            AND CASE WHEN  @ChatIdPar = 0
                     THEN 1 
                     ELSE @ChatIdPar = Accounts.ChatId
                END";

            comm.Parameters.Add("@NamePar", Text).Value = account.Name ?? "";
            comm.Parameters.Add("@Id", Integer).Value = account.Id;
            comm.Parameters.Add("@ChatIdPar", Integer).Value = account.ChatId;

            var reader = comm.ExecuteReader();
            List<Account> res = new List<Account>();

            while (reader.Read())
            {
                Account a = new Account();
                a.Id = Parse(reader["Id"]);
                a.ChatId = Parse(reader["ChatId"]);
                a.Name = reader["Name"].ToString();
                res.Add(a);
            }
            return res;
        }
        public void Write(Account account)
        {
            var comm = Connection.CreateCommand();

            comm.CommandText = @"INSERT INTO [Accounts]
            ([Name],[ChatId])
            VALUES (@name,@id)";
            comm.Parameters.Add("@name", Text).Value = account.Name;
            comm.Parameters.Add("@id", Integer).Value = account.ChatId;
            comm.ExecuteNonQuery();
        }
        public async void Update(Account account)
        {
            var comm = Connection.CreateCommand();
            comm.CommandText = @"
            UPDATE Accounts SET
	        [Name] = @NamePar,
            [ChatId] = @ChatIdPar,           
		    WHERE @id = id;";
            comm.Parameters.Add("@NamePar", Text).Value = account.Name;
            comm.Parameters.Add("@ChatIdPar", Integer).Value = account.ChatId;
            comm.Parameters.Add("@id", Integer).Value = account.Id;
            await comm.ExecuteNonQueryAsync();
        }
    }
}
