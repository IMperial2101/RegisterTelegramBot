
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using System.Security.Policy;
using System.Text.RegularExpressions;


namespace RegBot2
{
    partial class BotOnMessageReceivedClass
    {
        //⛔️Отменить процесс регистрации
        private async void CanceProcessRegistrationCommand(MyUser user )
        {
            user.userState = Enums.UserState.Start;

            var keyboard = new ReplyKeyboardRemove();

            await telegramBot.SendTextMessageAsync(
            chatId: user.chatId,
            text: "Процесс регистрации отменен!",
            replyMarkup: keyboard);

            Messages.SendStartMessageAsync(user);

        }
        //📄Инструкция
        private async void InstructionCommand(MyUser user)
        {

            if (user.userState == Enums.UserState.InRegistration || user.userState == Enums.UserState.MakeLink)
                return;
            SwitchUserstate = false;
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.Default;
            await Messages.SendInstructionMessageAsync(user.chatId);
            
        }
        //🆕Регистрация
        private async void RegistrationCommand(MyUser user)
        {

            user.userState = Enums.UserState.InRegistration;
            user.registrationState = Enums.RegistrationState.Login;

            Messages.SendMessageCancelRegistration(user);

            await telegramBot.SendTextMessageAsync(user.chatId, "<b>Введите логин:</b> \n<i>*Логин должен быть от 5 до 30 символов.</i>", ParseMode.Html);
        }
        //✅Да
        private async void ConfirmRegistrationCommand(MyUser user)
        {
            user.registrationState = Enums.RegistrationState.EndRegistration;
            user.userState = Enums.UserState.ConfirmRegistaration;
            dataBase.SqlCommand(string.Format("INSERT INTO users (telegram_user_name, login, email, _password, chat_id , search_link,register_date) VALUES ('{0}','{1}','{2}','{3}',{4},'','{5}');", user.telegramUserName, user.login, user.email, user.password, user.chatId, DateTime.Now.ToString("yyyy-MM-dd")));
            await telegramBot.SendTextMessageAsync(user.chatId, "Вы успешно зарегистрировали аккаунт");
            Messages.SendCommandsNoLink(user);
        }
        //🔄Пройти заного
        private async void AgainRegistrationCommand(MyUser user)
        {
            RegistrationCommand(user);
        }
        //➕Добавить ссылку
        private async void AddlinkCommand(MyUser user)
        {
            await telegramBot.SendTextMessageAsync(user.chatId, "Отправьте ссылку для поиска");
            Messages.SendKeyboardAddLink(user);
            user.userCommandStateNoLink = Enums.UserCommandStateNoLink.AddLink;

        }
        //🔁Изменить пароль
        private async void ChangePasswordCommand(MyUser user)
        {
            await telegramBot.SendTextMessageAsync(user.chatId, "Отправьте новый пароль");
            Messages.SendChangePasswordKeyboard(user);
            user.userCommandState = Enums.UserCommandState.ChangePassword;
        }
        //⛔️Отменить изменение пароля
        private async void CanceChangePasswordCommand(MyUser user)
        {
            user.userCommandState = Enums.UserCommandState.Default;
            CommandsListCommand(user);
        }
        //🛠Создать ссылку
        private async void MakeLinkCommand(MyUser user)
        {
            user.makingMylink = new MyLink();
            user.userState = Enums.UserState.MakeLink;
            user.userMakeLinkState = Enums.UserMakeLinkState.City;
            user.userCommandStateNoLink = Enums.UserCommandStateNoLink.Default;

            user.makingMylink.url = "https://www.avito.ru/";
            await telegramBot.SendTextMessageAsync(user.chatId, "Отправьте город в котором нужно искать");
            Messages.SendMakeLinkKeyboard(user);
        }
        //⛔️Отменить создание ссылки
        private async void CanceMakingLinkCommand(MyUser user)
        {
            switch(user.userHasLink)
            {
                case Enums.UserHasLink.Yes:
                    Messages.SendCommandsWithLink(user);
                    break;
                case Enums.UserHasLink.No:
                    Messages.SendCommandsNoLink(user);
                    break;
            }
            user.userState = Enums.UserState.ConfirmRegistaration;
        }
        //⛔️Отменить добавление
        private async void CanceAddLinkCommand(MyUser user)
        {
            user.userState = Enums.UserState.ConfirmRegistaration;
            user.userCommandStateNoLink = Enums.UserCommandStateNoLink.Default;
            Messages.SendCommandsNoLink(user);
        }
        //🔄изменить ссылку
        private async void ChangeLinkCommand(MyUser user)
        {
            await telegramBot.SendTextMessageAsync(user.chatId, "Отправьте новую ссылку для поиска");
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.ChangeLink;
            Messages.SendCommandsCheckLinks(user);

        }
        //⛔️Остановить поиск
        private async void StopSearchCommand(MyUser user)
        {
            if (!user.activeSearch)
                return;
            SwitchUserstate = false;
            user.activeSearch = false;
            dataBase.SqlCommand($"DELETE FROM Active_search WHERE chat_id = {user.chatId};");
            telegramBot.SendTextMessageAsync(user.chatId, "Поиск остановлен");

        }
        //✅Продолжить поиск
        private async void ContinueSearchCommand(MyUser user)
        {
            if (user.activeSearch)
                return;
            user.activeSearch = true;           
            dataBase.SqlCommand(string.Format("INSERT INTO Active_search (ID, chat_id, search_link) SELECT ID, chat_id, search_link FROM Users WHERE chat_id = {0};", user.chatId));
            telegramBot.SendTextMessageAsync(user.chatId, "Поиск возобновлен");
        }
        //💙Сохранить ссылку
        private async void SaveLinkCommand(MyUser user)
        {
            if (user.userHasLink == Enums.UserHasLink.No)
            {
                telegramBot.SendTextMessageAsync(user.chatId, "Для доступа к сохраненным ссылкам, нужно сначала сделать одну активную ссылку");
                return;
            }
               
            if (user.MyLinksSavedList.Count > 9)
                user.MyLinksSavedList.RemoveAt(0);
            user.MyLinksSavedList.Add(new MyLink(user.makingMylink));
            user.constructorSavedJson = MyLinkJSONController.AddLinkToJson(user.constructorSavedJson, user.makingMylink);

            dataBase.SqlCommand($"UPDATE Users SET constructor_saved = '{user.constructorSavedJson}' WHERE chat_id = '{user.chatId}';");
            telegramBot.SendTextMessageAsync(user.chatId, "Ссылка доблена в избранное");
            CommandsListCommand(user);
        }
        //✅Сделать активной
        private async void MakeActiveCommand(MyUser user)
        {
            user.searchLink = user.makingMylink.url;
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.Default;
            user.userHasLink = Enums.UserHasLink.Yes;
            dataBase.SqlCommand(string.Format("UPDATE Users SET search_link = '{0}' WHERE Users.chat_id = {1};", user.searchLink, user.chatId));
            if (user.activeSearch)
                dataBase.SqlCommand(string.Format("UPDATE Active_search SET search_link = '{0}' WHERE Active_search.chat_id = {1};", user.searchLink, user.chatId));
            else
            {
                dataBase.SqlCommand(string.Format("INSERT INTO Active_search (ID, chat_id, search_link) SELECT ID, chat_id, search_link FROM Users WHERE chat_id = {0};", user.chatId));
                user.activeSearch = true;
            }
            
            await telegramBot.SendTextMessageAsync(user.chatId, "Ссылка изменена");
            CommandsListCommand(user);
        }
        //🔁Построить заново
        private async void MakeLinkAgainCommand(MyUser user)
        {
            MakeLinkCommand(user);
        }
        //🧾Список команд
        private async void CommandsListCommand(MyUser user)
        {
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.Default;
            if(user.userHasLink == Enums.UserHasLink.Yes)
                Messages.SendCommandsWithLink(user);
            else
                Messages.SendCommandsNoLink(user);
        }
        //📋Список ссылок
        private async void LinksListCommand(MyUser user)
        {
            await Messages.SendCommandsLinksAction(user);
        }
        //💙Избранное
        private async void SavedCommand(MyUser user)
        {
            await Messages.SendCommandsChooseLink(user, false);
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.ChooseLinkSaved;
            SendHistoryLinksList(user);
        }
        //🗃История
        private async void HistoryCommand(MyUser user)
        {
            await Messages.SendCommandsChooseLink(user, true);
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.ChooseLinkHistory;
            SendSavedLinksList(user);
        }
        //🔗Выбрать ссылку
        private async void ChooseLinkCommand(MyUser user)
        {
            await telegramBot.SendTextMessageAsync(user.chatId, "Отправьте номер ссылки, которую хотите сделать активной");

        }
        //❌Удалить ссылку
        private async void DeleteLinkCommand(MyUser user)
        {
            await telegramBot.SendTextMessageAsync(user.chatId, "Отправьте номер ссылки, которую хотите удалить");
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.CheckRequestHistory;
            user.UserActionsWithLinks = Enums.UserActionsWithLinks.DeleteOne;
        }
        //❌Удалить все
        private async void DeleteAllCommand(MyUser user)
        {
            
            if(user.SwitchHistorySaved == Enums.SwitchHistorySaved.History)
            {
                user.constructorHistoryJson = MyLinkJSONController.RemoveAll();
                user.MyLinksHistoryList.Clear();
                dataBase.UpdateJsonFileHistory(user, "[]");

            }
            else if(user.SwitchHistorySaved == Enums.SwitchHistorySaved.Saved)
            {
                user.constructorSavedJson = MyLinkJSONController.RemoveAll();
                user.MyLinksSavedList.Clear();
                dataBase.UpdateJsonFileSaved(user, "[]");
            }
            await telegramBot.SendTextMessageAsync(user.chatId, "Все ссылки удалены");
        }
        //💙Избранные ссылки
        private async void SavedLinksCheckCommand(MyUser user)
        {
            user.SwitchHistorySaved = Enums.SwitchHistorySaved.Saved;
            await Messages.SendCommandsLinksActionSaved(user);
            await SendSavedLinksList(user);
            
            
        }
        //🗃История ссылок
        private async void HistoryLinksCommand(MyUser user)
        {
            user.SwitchHistorySaved = Enums.SwitchHistorySaved.History;
            await Messages.SendCommandsLinksActionHistory(user);
            await SendHistoryLinksList(user);
            
            
        }
        //➕Добавить ссылку
        private async void AddHandleCommand(MyUser user)
        {
            await telegramBot.SendTextMessageAsync(user.chatId, "Отправьте ссылку которую хотите сохранить");
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.AddLinkHandle;
        }






        private async Task SendHistoryLinksList(MyUser user)
        {
            int i = 0;
            string messageMakeConstructor;
            string messageAddHandle;
            string callBackStringMakeActive;
            string callBackStringDelete;

            foreach (var myLink in user.MyLinksHistoryList)
            {
                callBackStringMakeActive = $"HistoryMakeActive_{i}";
                callBackStringDelete = $"HistoryDelete_{i}";
                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData("✅Сделать активной", callBackStringMakeActive),
                        InlineKeyboardButton.WithCallbackData("❌Удалить", callBackStringDelete)
                    }
                };
                InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);
                messageMakeConstructor = $"{i} - {myLink.city}" +
                    $"\n     {myLink.name}" +
                    $"\n     {myLink.priceFrom} - {myLink.priceTo}" +
                    $"\n     (🛠Создана в конструкторе)";
                messageAddHandle = $"{i} - {myLink.city}" +
                    $"\n     {myLink.name}" +
                    $"\n     (➕Добавлена вручную)";
                await telegramBot.SendTextMessageAsync(user.chatId, (myLink.makeByConstructor ? messageMakeConstructor : messageAddHandle), replyMarkup: keyboard);


                //dsfsd
                callbackQueryToLinkNumber[callBackStringDelete] = i;
                i++;
            }
            return;
        }

        private async Task SendSavedLinksList(MyUser user)
        {
            int i = 0;
            string message;
            string messageMakeConstructor;
            string messageAddHandle;
            string callBackStringMakeActive;
            string callBackStringDelete;
            user.MyLinksSavedDictionary.Clear();

            foreach (var myLink in user.MyLinksSavedList)
            {
                
                callBackStringMakeActive = $"SavedMakeActive_{i}";
                callBackStringDelete = $"SavedDelete_{i}";
                user.MyLinksSavedDictionary.Add(i, myLink);
                List<List<InlineKeyboardButton>> buttons = new List<List<InlineKeyboardButton>>
                {
                    new List<InlineKeyboardButton>
                    {
                        InlineKeyboardButton.WithCallbackData("✅Сделать активной", callBackStringMakeActive),
                        InlineKeyboardButton.WithCallbackData("❌Удалить", callBackStringDelete)
                    }
                };
                InlineKeyboardMarkup keyboard = new InlineKeyboardMarkup(buttons);
                messageMakeConstructor = $"{i} - {myLink.city}" +
                    $"\n     {myLink.name}" +
                    $"\n     {myLink.priceFrom} - {myLink.priceTo}" +
                    $"\n     (🛠Создана в конструкторе)";
                messageAddHandle = $"{i} - {myLink.city}" +
                    $"\n     {myLink.name}" +
                    $"\n     (➕Добавлена вручную)";
                await telegramBot.SendTextMessageAsync(user.chatId, (myLink.makeByConstructor ? messageMakeConstructor : messageAddHandle), replyMarkup: keyboard);


                callbackQueryToLinkNumber[callBackStringMakeActive] = i;
                callbackQueryToLinkNumber[callBackStringDelete] = i;
                i++;
            }
            return;
        }


    }
}
