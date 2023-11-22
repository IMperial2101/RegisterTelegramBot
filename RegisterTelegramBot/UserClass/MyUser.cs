

using RegBot2;
using Telegram.Bot.Types;

namespace RegBot2
{
    internal class MyUser:MyUserState
    {      
        public int id { get; set; }
        public string telegramUserName { get; set; }
        public string login { get; set; }
        public string email { get; set; }
        public string password { get; set; }
        public long chatId { get; set; }
        public string searchLink { get; set; }
        public bool activeSearch = false;
        public MyLink makingMylink { get; set; } 
        public List<string> availableCommands = new List<string>();
        public List<MyLink> MyLinksHistoryList { get; set; } = new List<MyLink>();
        public List<MyLink> MyLinksSavedList { get; set; } = new List<MyLink>();
        public Dictionary<int, MyLink> MyLinksHistoryDictionary = new Dictionary<int, MyLink>();
        public Dictionary<int, MyLink> MyLinksSavedDictionary = new Dictionary<int, MyLink>();
        public string constructorSavedJson { get; set; } 
        public string constructorHistoryJson { get; set; }
        public async void AutoAddLink(MyLink myLink, DataBase dataBase, bool history = true)
        {
            if(history)
            {
                MyLinksHistoryList.Add(myLink);
                constructorHistoryJson = MyLinkJSONController.AddLinkToJson(constructorHistoryJson, myLink);
                dataBase.UpdateJsonFileHistory(this, constructorHistoryJson);
            }
            else
            {
                MyLinksSavedList.Add(myLink);
                constructorSavedJson = MyLinkJSONController.AddLinkToJson(constructorSavedJson, myLink);
                dataBase.UpdateJsonFileSaved(this, constructorSavedJson);
            }
            
        }
        public async void AutoDeleteLink(DataBase dataBase, int count, bool history = true)
        {
            if (history)
            {
                constructorHistoryJson = MyLinkJSONController.RemoveLinkFromJson(constructorHistoryJson, MyLinksHistoryList[count]);
                MyLinksHistoryList.RemoveAt(count);
                dataBase.UpdateJsonFileHistory(this, constructorHistoryJson);
            }
            else
            {
                constructorSavedJson = MyLinkJSONController.RemoveLinkFromJson(constructorSavedJson, MyLinksSavedList[count]);
                MyLinksSavedList.RemoveAt(count);
                dataBase.UpdateJsonFileSaved(this,constructorSavedJson);
            }
        }
        public void fillAvailableCommands()
        {
            if (userHasLink == Enums.UserHasLink.Yes)
            {
                availableCommands.AddRange(new string[] { "⛔️Остановить поиск", "✅Продолжить поиск", "🔁Изменить пароль", "📄Инструкция", "🔄изменить ссылку", "📋Список ссылок", "🛠Создать ссылку" });
                
            }        
            else
            {
                availableCommands.AddRange(new string[] { "➕Добавить ссылку", "🔁Изменить пароль", "📄Инструкция", "🛠Создать ссылку" });
            }


        }
        //корректная ссылка с ценой - https://www.avito.ru/moskva?q=iphone&s=104&f=ASgCAgECAUXGmgwZeyJmcm9tIjoxMDAwMCwidG8iOjEwMDAwfQ%3D%3D&pmin=10000&pmax=20000
    }
}
