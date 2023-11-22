using System.Data;
using MySql.Data.MySqlClient;
using Org.BouncyCastle.Asn1.BC;
using RegBot2.DataBaseClass;
using Telegram.Bot.Types;
using static RegBot2.Enums;


namespace RegBot2
{
    internal class DataBase
    {
        public DataBaseSqlCommands sqlCommands;
        private MySqlDataAdapter adapter = new MySqlDataAdapter();
        private MySqlConnection connection = new MySqlConnection(@"Server = localhost;port=3306;Initial Catalog=login;User id=root;password='';database = avitoparserdatabase");
        public DataBase()
        {
            sqlCommands = new DataBaseSqlCommands(this);
        }

        public void Chek()
        {
            if (connection.State != ConnectionState.Open)
            {
                Console.WriteLine("Ошибка подключения!");
            }
            else
                Console.WriteLine("Подключение установлено!");
        }
        public void SqlCommand(string sqlCommand)
        {
            MySqlCommand command = new MySqlCommand(sqlCommand, GetConnection());
            adapter.SelectCommand = command;
            command.ExecuteNonQuery();
        }
        public void SqlCommandShowInfo(string sqlCommand)
        {
            DataTable table = new DataTable();

            MySqlCommand command = new MySqlCommand(sqlCommand, GetConnection());
            adapter.SelectCommand = command;

            adapter.Fill(table);

            if (table.Rows.Count > 0)
                Console.WriteLine(table.Rows.Count);

            DataRow[] currentRows = table.Select(
    null, null, DataViewRowState.CurrentRows);

            if (currentRows.Length < 1)
                Console.WriteLine("No Current Rows Found");
            else
            {
                foreach (DataColumn column in table.Columns)
                    Console.Write("\t{0}", column.ColumnName);

                Console.WriteLine("\tRowState");

                foreach (DataRow row in currentRows)
                {
                    foreach (DataColumn column in table.Columns)
                        Console.Write("\t{0}", row[column]);

                    Console.WriteLine("\t" + row.RowState);
                }
            }

        }
        public List<string> SqlCommandGetOneColumn(string sqlCommand)
        {
            List<string> info = new List<string>();

            DataTable table = MakeSqlTable(sqlCommand);

            if (table.Rows.Count < 1)
                Console.WriteLine("No Current rows find");
            else if (table.Columns.Count > 1)
                Console.WriteLine("More than 1 column selected");
            else
            {
                foreach (DataRow row in table.Rows)
                    info.Add(row[0].ToString());
            }
            return info;
        }
        public Dictionary<long, MyUser> FillDictionaryFromDB()
        {
            Dictionary<long, MyUser> users = new Dictionary<long, MyUser>();

            string query = "SELECT * FROM users";
            MySqlCommand command = new MySqlCommand(query, GetConnection());

            using (var reader = command.ExecuteReader())
            {
                while (reader.Read())
                {
                    var user = new MyUser()
                    {
                        id = reader.GetInt32(0),
                        telegramUserName = reader.GetString(1),
                        login = reader.GetString(2),
                        email = reader.GetString(3),
                        password = reader.GetString(4),
                        chatId = reader.GetInt32(5),
                        searchLink = reader.GetString(6),
                        constructorSavedJson = reader.GetString(7),
                        constructorHistoryJson = reader.GetString(8),
                        userState = Enums.UserState.ConfirmRegistaration,
                        registrationState = Enums.RegistrationState.EndRegistration,
                        userCommandStateWithLink = Enums.UserCommandStateWithLink.Default,
                        userCommandStateNoLink = Enums.UserCommandStateNoLink.Default,
                        userCommandState = Enums.UserCommandState.Default


                    };
                    if (user.searchLink == "")
                        user.userHasLink = Enums.UserHasLink.No;
                    else
                        user.userHasLink = Enums.UserHasLink.Yes;
                   
                    user.fillAvailableCommands();
                    DecodeJSON(user);
                    users.Add(user.chatId, user);
                }
                              
            }
            SendCommands(users);
            return users;
        }
        private void SendCommands(Dictionary<long,MyUser> users)
        {
            foreach(var user in users)
            {
                if (user.Value.userHasLink == Enums.UserHasLink.Yes)
                    Messages.SendCommandsWithLink(user.Value);
                else
                    Messages.SendCommandsNoLink(user.Value);
                List<string> chatsActiveSearch = SqlCommandGetOneColumn($"SELECT Active_search.chat_id from Active_search WHERE chat_id = '{user.Value.chatId}'");
                if (chatsActiveSearch.Count > 0)
                    user.Value.activeSearch = true;
            }
            
        }
        private void DecodeJSON(MyUser user)
        {
            user.MyLinksHistoryList = MyLinkJSONController.DecryptJsonToList(user.constructorHistoryJson);
            user.MyLinksSavedList = MyLinkJSONController.DecryptJsonToList(user.constructorSavedJson); 
        }
        private DataTable MakeSqlTable(string sqlCommand)
        {
            DataTable table = new DataTable();

            MySqlCommand command = new MySqlCommand(sqlCommand, GetConnection());
            adapter.SelectCommand = command;

            adapter.Fill(table);
            return table;
        }
        public void UpdateJsonFileHistory(MyUser user, string jsonHistory)
        {
            SqlCommand($"UPDATE Users SET constructor_history = '{jsonHistory}' WHERE chat_id = '{user.chatId}';");
        }
        public void UpdateJsonFileSaved(MyUser user, string jsonSaved)
        {
            SqlCommand($"UPDATE Users SET constructor_saved = '{jsonSaved}' WHERE chat_id = '{user.chatId}';");
        }

        public void Close()
        {
            connection.Close();
        }
        public void Open()
        {
            connection.Open();
        }
        public MySqlConnection GetConnection()
        {
            return connection;
        }
    }
}

