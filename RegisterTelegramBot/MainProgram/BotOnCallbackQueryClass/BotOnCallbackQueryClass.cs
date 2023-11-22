using Telegram.Bot;
using Telegram.Bot.Args;


namespace RegBot2.MainProgram
{
    partial class BotOnCallbackQueryClass
    {
        public BotOnCallbackQueryClass(DataBase _dataBase, TelegramBotClient _telegramBot, Dictionary<long, MyUser> _Users,Dictionary<string,int> _callbackQueryToLinkNumber)
        {
            dataBase = _dataBase;
            telegramBot = _telegramBot;
            Users = _Users;
            callbackQueryToLinkNumber = _callbackQueryToLinkNumber;
        }

        Dictionary<string, int> callbackQueryToLinkNumber;
        DataBase dataBase;
        TelegramBotClient telegramBot;
        static Dictionary<long, MyUser> Users;
        public async void BotOnCallbackQuery(object sender,
                                             CallbackQueryEventArgs callbackQueryEventArgs)
        {
            Console.WriteLine("Thread id: {0}", Thread.CurrentThread.ManagedThreadId);
            Console.WriteLine("{0} choise {1}\n", callbackQueryEventArgs.CallbackQuery.From, callbackQueryEventArgs.CallbackQuery.Data.ToString());

            var callBackQuery = callbackQueryEventArgs.CallbackQuery.Data;
            var chatId = callbackQueryEventArgs.CallbackQuery.Message.Chat.Id;

            if (callBackQuery == null)
                return;

            MyUser user = CheckUser(chatId);
            SwitchListActions(user, callbackQueryEventArgs);
            
        }
        private MyUser CheckUser(long chatId)
        {
            MyUser user;
            if (!Users.TryGetValue(chatId, out user))
            {
                user = new MyUser();
                Users.Add(chatId, user);
            }
            return user;
        }
        private async void SwitchListActions(MyUser user, CallbackQueryEventArgs callbackQuery)
        {
            string callbackData = callbackQuery.CallbackQuery.Data;
            int number = int.Parse(callbackData[^1].ToString());
            string switchAction = "";
            if (callbackData.Contains("History"))
                if (callbackData.Contains("MakeActive"))
                    switchAction = "HistoryMakeActive";
                else
                    switchAction = "HistoryDelete";
            else if (callbackData.Contains("Saved"))
                if (callbackData.Contains("MakeActive"))
                    switchAction = "SavedMakeActive";
                else
                    switchAction = "SavedDelete";

            switch (switchAction)
            {
                case "HistoryMakeActive":
                    HistoryMakeActive(user, callbackQuery, number);
                    break;
                case "HistoryDelete":
                    HistoryDeleteLink(user, callbackQuery, callbackQueryToLinkNumber.FirstOrDefault(x => x.Value == number).Key);
                    break;
                case "SavedMakeActive":
                    SavedMakeActive(user, callbackQuery, number);
                    break;
                case "SavedDelete":
                    SavedDeleteLink(user, callbackQuery, number);
                    break;
            }

        }


    }
}
