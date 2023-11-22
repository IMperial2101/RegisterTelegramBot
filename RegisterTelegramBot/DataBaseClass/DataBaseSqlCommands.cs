using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace RegBot2.DataBaseClass
{
    internal class DataBaseSqlCommands
    {
        private DataBase dataBase;

        public DataBaseSqlCommands(DataBase dataBase)
        {
            this.dataBase = dataBase;
        }

        public void UpdateUsersSearchLinkDB(MyUser user)
        {
            dataBase.SqlCommand(string.Format("UPDATE Users SET search_link = '{0}' WHERE Users.chat_id = {1};", user.searchLink, user.chatId));
        }

        public void UpdateActiveSearchSearchLinkDB(MyUser user)
        {
            dataBase.SqlCommand(string.Format("UPDATE Active_search SET search_link = '{0}' WHERE Active_search.chat_id = {1};", user.searchLink, user.chatId));
        }

        public void InsertIntoActiveSearchLinkDB(MyUser user)
        {
            dataBase.SqlCommand(string.Format("INSERT INTO Active_search (ID, chat_id, search_link) SELECT ID, chat_id, search_link FROM Users WHERE chat_id = {0};", user.chatId));
        }

        public void AutoUpdateNewSearchLinkInDB(MyUser user, string newLink)
        {
            user.searchLink = newLink;
            UpdateUsersSearchLinkDB(user);
            if (user.activeSearch)
                UpdateActiveSearchSearchLinkDB(user);
            else
            {
                InsertIntoActiveSearchLinkDB(user);
                user.activeSearch = true;
            }
        }
    }
}
