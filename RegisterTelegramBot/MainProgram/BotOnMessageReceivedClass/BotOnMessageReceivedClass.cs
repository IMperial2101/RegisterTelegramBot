using Telegram.Bot.Args;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using Org.BouncyCastle.Crypto.Prng;

namespace RegBot2
{
    partial class BotOnMessageReceivedClass
    {
        public BotOnMessageReceivedClass(DataBase _dataBase, TelegramBotClient _telegramBot, Dictionary<long, MyUser> _Users, List<string> _citiesFromDbRu, List<string> _citiesFromDbEn,List<string> _allCommands, Dictionary<string, int> _callbackQueryToLinkNumber)
        {
            dataBase = _dataBase;
            telegramBot = _telegramBot;
            Users = _Users;
            citiesFromDbRu = _citiesFromDbRu;  
            citiesFromDbEn = _citiesFromDbEn;
            allCommands = _allCommands;
            callbackQueryToLinkNumber = _callbackQueryToLinkNumber;
        }
        Dictionary<string, int> callbackQueryToLinkNumber;
        public static DataBase dataBase;
        public static TelegramBotClient telegramBot;
        public static Dictionary<long, MyUser> Users = new Dictionary<long, MyUser>();
        static List<string> citiesFromDbRu;
        static List<string> citiesFromDbEn;
        static List<string> allCommands;

        private bool SwitchUserstate;
        public async void _BotOnMessageReceived(object sender,
                                               MessageEventArgs messageEventArgs)
        {

            Console.WriteLine("@{0} send {1}\n", messageEventArgs.Message.Chat.Username, messageEventArgs.Message.Text);

            var message = messageEventArgs.Message;
            var chatId = message.Chat.Id;

            if (message == null || message.Type != MessageType.Text)
                return;

            

            MyUser user = CheckUserAndAdd(chatId, message.Chat.Username);
            SwitchUserstate = true;
            if (allCommands.Contains(message.Text))
                if (!user.availableCommands.Contains(message.Text))
                {
                    telegramBot.SendTextMessageAsync(user.chatId, "Эта команда пока недоступна, в клавиатуре есть все доступные команды");
                    return;
                }    
                else
                    SwitchUserstate = false;


            if(SwitchUserstate)
                SwitchUserState(user, message);
            else
                SwitchUserCommands(user, message);



        }
        private async void SwitchUserCommands(MyUser user, Message message)
        {
            switch (message.Text)
            {
                case "⛔️Отменить процесс регистрации":
                    CanceProcessRegistrationCommand(user);
                    break;
                case "📄Инструкция":
                    InstructionCommand(user);
                    break;
                case "🆕Регистрация":
                    RegistrationCommand(user);
                    break;
                case "✅Да":
                    ConfirmRegistrationCommand(user);
                    break;
                case "🔄Пройти заного":
                    AgainRegistrationCommand(user);
                    break;
                case "➕Добавить ссылку":
                    AddlinkCommand(user);
                    break;
                case "🔁Изменить пароль":
                    ChangePasswordCommand(user);
                    break;
                case "⛔️Отменить изменение пароля":
                    CanceChangePasswordCommand(user);
                    break;
                case "🛠Создать ссылку":
                    MakeLinkCommand(user);
                    break;
                case "⛔️Отменить создание ссылки":
                    CanceMakingLinkCommand(user);
                    break;
                case "⛔️Отменить добавление":
                    CanceAddLinkCommand(user);
                    break;
                case "🔄изменить ссылку":
                    ChangeLinkCommand(user);
                    break;
                case "⛔️Остановить поиск":
                    StopSearchCommand(user);
                    break;
                case "✅Продолжить поиск":
                    ContinueSearchCommand(user);
                    break;
                case "💙Сохранить ссылку":
                    SaveLinkCommand(user);
                    break;
                case "✅Сделать активной":
                    MakeActiveCommand(user);
                    break;
                case "🔁Построить заново":
                    MakeLinkAgainCommand(user);
                    break;
                case "🧾Список команд":
                    CommandsListCommand(user);
                    break;
                case "📋Список ссылок":
                    LinksListCommand(user);
                    break;
                case "💙Избранное":
                    SavedCommand(user);
                    break;
                case "🗃История":
                    HistoryCommand(user);
                    break;
                case "🔗Выбрать ссылку":
                    ChooseLinkCommand(user);
                    break;
                case "✖️Удалить ссылку":
                    DeleteLinkCommand(user);
                    break;
                case "❌Удалить все":
                    DeleteAllCommand(user);
                    break;
                case "💙Избранные ссылки":
                    SavedLinksCheckCommand(user);
                    break;
                case "🗃История ссылок":
                    HistoryLinksCommand(user);
                    break;
                case "➕Добавить вручную":
                    AddHandleCommand(user);
                    break;
            }
        }
        private async void SwitchUserState(MyUser user,
                                   Message message)
        {
            if (!SwitchUserstate)
                return;
            switch (user.userState)
            {
                case Enums.UserState.Start:
                    UserState_Start(user);
                    break;
                case Enums.UserState.InRegistration:
                    switch (user.registrationState)
                    {
                        case Enums.RegistrationState.Start:
                            RegistrationState_Start(user);
                            break;

                        case Enums.RegistrationState.Login:
                            RegistrarionState_Login(user, message);
                            break;

                        case Enums.RegistrationState.Email:
                            RegistrarionState_Email(user, message);
                            break;

                        case Enums.RegistrationState.Password:
                            RegistrarionState_Password(user, message);
                            break;
                        case Enums.RegistrationState.Stop:
                            RegistrationState_Stop(user);
                            break;
                    }
                    break;
                case Enums.UserState.ConfirmRegistaration:
                    switch(user.userCommandState)
                    {
                        case Enums.UserCommandState.Default:
                            switch (user.userHasLink)
                            {
                                case Enums.UserHasLink.No:
                                    switch (user.userCommandStateNoLink)
                                    {
                                        case Enums.UserCommandStateNoLink.Default:
                                            UserCommandStateNoLink_Default(user);
                                            break;
                                        case Enums.UserCommandStateNoLink.AddLink:
                                            UserCommandStateNoLink_AddLink(user, message);
                                            break;       
                            }
                                    break;
                                case Enums.UserHasLink.Yes:
                                    switch (user.userCommandStateWithLink)
                                    {
                                        case Enums.UserCommandStateWithLink.Default:
                                            UserCommandStateWithLink_Default(user, message);
                                            break;
                                        case Enums.UserCommandStateWithLink.ChangeLink:
                                            UserCommandStateWithLink_ChangeLink(user, message);
                                            break;
                                        case Enums.UserCommandStateWithLink.StopSearch:
                                            UserCommandStateWithLink_StopSearch();
                                            break;
                                        case Enums.UserCommandStateWithLink.ContinueSearch:
                                            UserCommandStateWithLink_ContinueSearch();
                                            break;
                                        case Enums.UserCommandStateWithLink.CheckRequestHistory:
                                           switch(user.UserActionsWithLinks)
                                            {
                                                case Enums.UserActionsWithLinks.DeleteOne:
                                                    UserActionWithLinks_DeleteOne(user,message);
                                                    break;
                                                case Enums.UserActionsWithLinks.DeleteAll:
                                                    UserActionWithLinks_DeleteAll(user);
                                                    break;
                                            }
                                            break;
                                        case Enums.UserCommandStateWithLink.ChooseLinkHistory:
                                            UserCommandStateWithLink_ChooseLink(user,message,true);
                                            break;
                                        case Enums.UserCommandStateWithLink.ChooseLinkSaved:
                                            UserCommandStateWithLink_ChooseLink(user,message,false);
                                            break;
                                        case Enums.UserCommandStateWithLink.AddLinkHandle:
                                            UserCommandStateWithLink_AddLinkHandle(user,message);
                                            break;
                                    }
                                    break;
                            }

                            break;
                        case Enums.UserCommandState.ChangePassword:
                            UserCommandState_ChangePassword(user,message);
                            break;
                    }
                    
                    break;
                case Enums.UserState.MakeLink:
                    switch (user.userMakeLinkState)
                    {
                        case Enums.UserMakeLinkState.City:
                            UserMakeLinkState_City(user, message);
                            break;
                        case Enums.UserMakeLinkState.Name:
                            UserMakeLinkState_Name(user, message);
                            break;
                        case Enums.UserMakeLinkState.PriceFrom:
                            UserMakeLinkState_PriceFrom(user, message);
                            break;
                        case Enums.UserMakeLinkState.PriceTo:
                            UserMakeLinkState_PriceTo(user, message);
                            break;
                    }
                    break;

                    //https://www.avito.ru/moskva?q=iphone&s=104&f=ASgCAgECAUXGmgwZeyJmcm9tIjoxMDAwMCwidG8iOjEwMDAwfQ%3D%3D&pmin=10000&pmax=20000
            }
        }
        private MyUser CheckUserAndAdd(long chatId, string username)
        {
            MyUser user;
            if (!Users.TryGetValue(chatId, out user))
            {
                user = new MyUser();
                user.chatId = chatId;
                user.telegramUserName = "@" + username;
                Users.Add(chatId, user);

            }
            return user;
        }
        

    }
}
