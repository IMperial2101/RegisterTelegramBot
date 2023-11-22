using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot.Types;
using Telegram.Bot.Exceptions;
using System.Net.Http.Headers;
using RegBot2.MainProgram;
using System.Text.RegularExpressions;
using System.Web;

namespace RegBot2
{
    partial class BotOnMessageReceivedClass
    {
        private async void UserState_Start(MyUser user)
        {
            Messages.SendStartMessageAsync(user);

        }

        private async void RegistrationState_Start(MyUser user)
        {
            user.registrationState = Enums.RegistrationState.Login;
        }
        private async void RegistrarionState_Login(MyUser user, Message message)
        {
            if (!CheckInfo.CheckLogin(message.Text, ref dataBase, user.chatId, telegramBot))
                return;

            user.login = message.Text;
            await telegramBot.SendTextMessageAsync(user.chatId, "<b>Введите почту:</b>", ParseMode.Html);
            user.registrationState = Enums.RegistrationState.Email;
        }
        private async void RegistrarionState_Email(MyUser user, Message message)
        {
            if (!CheckInfo.CheckEmail(message.Text, ref dataBase, user.chatId, telegramBot))
                return;

            user.email = message.Text;
            await telegramBot.SendTextMessageAsync(user.chatId, "<b>Введите пароль:</b> \n<i>*Пароль должен быть от 5 до 30 символов.</i>", ParseMode.Html);
            user.registrationState = Enums.RegistrationState.Password;
        }
        private async void RegistrarionState_Password(MyUser user, Message message)
        {
            if (!CheckInfo.CheckPassword(message.Text, user.chatId, telegramBot))
                return;

            user.password = message.Text;
            Messages.SendConfirmRegisterMessageAsync(user);
        }
        private async void RegistrationState_Stop(MyUser user)
        {
            user.userState = Enums.UserState.Start;
        }


        private async void UserCommandStateNoLink_Default(MyUser user)
        {
            Messages.SendCommandsNoLink(user);
        }
        private async void UserCommandStateNoLink_AddLink(MyUser user, Message message)
        {
            if (!CheckInfo.CheckLink(message.Text, user.chatId, telegramBot))
                return;
            user.searchLink = message.Text;
            dataBase.SqlCommand(string.Format("UPDATE users SET search_link = '{0}' WHERE users.chat_id = {1};", user.searchLink, user.chatId));
            dataBase.SqlCommand(string.Format("INSERT INTO Active_search (ID, chat_id, search_link) SELECT ID, chat_id, search_link FROM Users WHERE chat_id = {0};", user.chatId));
            await telegramBot.SendTextMessageAsync(user.chatId, "Ссылка добавлена.");
            Messages.SendCommandsWithLink(user);
            user.userHasLink = Enums.UserHasLink.Yes;
            user.userCommandStateNoLink = Enums.UserCommandStateNoLink.Default;
            user.activeSearch = true;
        }



        private async void UserCommandState_ChangePassword(MyUser user, Message message)
        {
            if (!CheckInfo.CheckPassword(message.Text, user.chatId, telegramBot))
                return;
            user.userCommandState = Enums.UserCommandState.Default;
            user.password = message.Text;
            dataBase.SqlCommand(string.Format("UPDATE users SET _password = '{0}' WHERE users.chat_id = {1}", user.password, user.chatId));
            await telegramBot.SendTextMessageAsync(user.chatId, "Пароль изменен");
            if (user.userHasLink == Enums.UserHasLink.Yes)
                Messages.SendCommandsWithLink(user);
            else
                Messages.SendCommandsNoLink(user);
        }


        private async void UserCommandStateWithLink_Default(MyUser user, Message message)
        {
            Messages.SendCommandsWithLink(user);
        }
        private async void UserCommandStateWithLink_ChangeLink(MyUser user, Message message)
        {
            if (!CheckInfo.CheckLink(message.Text, user.chatId, telegramBot))
                return;
            MyLink myLinkToAdd = new MyLink();
            RegexLink(myLinkToAdd, message.Text);
            user.AutoAddLink(myLinkToAdd,dataBase, true);
            user.searchLink = message.Text;
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.Default;
            ChangeLinkInDataBase(user);



        }
        private async void UserCommandStateWithLink_StopSearch()
        {

        }
        private async void UserCommandStateWithLink_ContinueSearch()
        {

        }
        private async void UserCommandStateWithLink_ChooseLink(MyUser user, Message message, bool history = true)
        {
            

            string count = message.Text;
            if (history)
            {
                if (CheckInfo.CheckLinkCount(count, user.MyLinksHistoryList))
                {
                    user.searchLink = user.MyLinksHistoryList[Convert.ToInt32(count)].url;
                    ChangeLinkInDataBase(user);
                }
            }
            else
            {
                if (CheckInfo.CheckLinkCount(count, user.MyLinksSavedList))
                {
                    user.searchLink = user.MyLinksSavedList[Convert.ToInt32(count)].url;
                    ChangeLinkInDataBase(user);
                }
            }
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.Default;
            Messages.SendCommandsWithLink(user);
        }
        private async void UserCommandStateWithLink_AddLinkHandle(MyUser user, Message message)
        {
            if (!CheckInfo.CheckLink(message.Text, user.chatId, telegramBot))
                return;
            MyLink myLinkToAdd = new MyLink();
            RegexLink(myLinkToAdd, message.Text);
            user.AutoAddLink(myLinkToAdd, dataBase, false);
            user.AutoAddLink(myLinkToAdd, dataBase, true);
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.Default;
            telegramBot.SendTextMessageAsync(user.chatId, "Ссылка добавлена");
            SavedLinksCheckCommand(user);
        }


        private async void UserActionWithLinks_DeleteOne(MyUser user,Message message)
        {
            string count = message.Text;
            if (user.SwitchHistorySaved == Enums.SwitchHistorySaved.History)
            {
                if(CheckInfo.CheckLinkCount(count,user.MyLinksHistoryList))
                {
                    user.constructorHistoryJson = MyLinkJSONController.RemoveLinkFromJson(user.constructorHistoryJson, user.MyLinksHistoryList[Convert.ToInt32(count)]);
                    user.MyLinksHistoryList.RemoveAt(Convert.ToInt32(count));
                    await telegramBot.SendTextMessageAsync(user.chatId, "Ссылка удалена");
                    dataBase.UpdateJsonFileHistory(user, user.constructorHistoryJson);
                    SendHistoryLinksList(user);
                }
            }
            else if(user.SwitchHistorySaved == Enums.SwitchHistorySaved.Saved)
            {
                if (CheckInfo.CheckLinkCount(count, user.MyLinksSavedList))
                {
                    user.constructorSavedJson = MyLinkJSONController.RemoveLinkFromJson(user.constructorSavedJson, user.MyLinksSavedList[Convert.ToInt32(count)]);
                    user.MyLinksSavedList.RemoveAt(Convert.ToInt32(count));
                    await telegramBot.SendTextMessageAsync(user.chatId, "Ссылка удалена");
                    dataBase.UpdateJsonFileSaved(user, user.constructorSavedJson);
                    SendSavedLinksList(user);
                }
            }
            user.userCommandStateWithLink = Enums.UserCommandStateWithLink.Default;
            user.UserActionsWithLinks = Enums.UserActionsWithLinks.Default;
        }
        private async void UserActionWithLinks_DeleteAll(MyUser user)
        {
            if (user.SwitchHistorySaved == Enums.SwitchHistorySaved.History)
            {

            }
            else if (user.SwitchHistorySaved == Enums.SwitchHistorySaved.Saved)
            {

            }
        }


        private async void UserMakeLinkState_City(MyUser user, Message message)
        {
            string userCityName = SearchUserCity(message.Text);
            if(userCityName == "Err")
            {
                telegramBot.SendTextMessageAsync(user.chatId, "К сожалению мы не нашли ваш город, попробуйте написать еще раз");
                return;
            }
            user.makingMylink.city = message.Text;
            user.makingMylink.url += userCityName+"?q=";
            telegramBot.SendTextMessageAsync(user.chatId, "Отправьте название товара");
            user.userMakeLinkState = Enums.UserMakeLinkState.Name;
        }
        private string SearchUserCity(string city)
        {
            string cityNameEn = "";
            int index = citiesFromDbRu.IndexOf(city.Trim().ToLower());
            if(index == -1)
            {
                cityNameEn = "Err";
                //город не найден

            }
            else
            {
                cityNameEn = citiesFromDbEn[index];
                //город найден
            }
            return cityNameEn;

        }
        private async void UserMakeLinkState_Name(MyUser user, Message message)
        {
            string name = message.Text;
            name = name.Trim();
            string[] words = name.Split(' ');
            for(int i = 0; i< words.Length-1; i++)
            {
                user.makingMylink.url += words[i] + '+';
            }
            user.makingMylink.name =name;
            user.makingMylink.url += words[words.Length-1];
            user.makingMylink.url += "&s=104";
            user.makingMylink.url += "&f=ASgCAgECAUXGmgwZeyJmcm9tIjoxMDAwMCwidG8iOjEwMDAwfQ%3D%3D";
            telegramBot.SendTextMessageAsync(user.chatId, "Цена от?");
            user.userMakeLinkState = Enums.UserMakeLinkState.PriceFrom;
        }
        private async void UserMakeLinkState_PriceFrom(MyUser user, Message message)
        {
            
            string priceFrom = message.Text.Trim();
            if (!CheckInfo.CheckPrice(priceFrom))
            {
                telegramBot.SendTextMessageAsync(user.chatId,"Некорректное число");
                return;
            }
            user.makingMylink.priceFrom = priceFrom;
            user.makingMylink.url += "&pmin=" + priceFrom;

            telegramBot.SendTextMessageAsync(user.chatId, "Цена до?");
            user.userMakeLinkState = Enums.UserMakeLinkState.PriceTo;

        }
        private async void UserMakeLinkState_PriceTo(MyUser user, Message message)
        {
            
            string priceTo = message.Text.Trim();
            if (!CheckInfo.CheckPrice(priceTo))
            {
                telegramBot.SendTextMessageAsync(user.chatId, "Некорректное число");
                return;
            }
            user.makingMylink.priceTo = priceTo;
            user.makingMylink.url += "&pmax=" + priceTo;
            telegramBot.SendTextMessageAsync(user.chatId, $"Успешно!\nВаша новая ссылка ={user.makingMylink.url} ");


            
            user.MyLinksHistoryList.Add(new MyLink(user.makingMylink));
            user.constructorHistoryJson = MyLinkJSONController.AddLinkToJson(user.constructorHistoryJson, user.makingMylink);

            dataBase.UpdateJsonFileHistory(user, user.constructorHistoryJson);
            user.userState = Enums.UserState.ConfirmRegistaration;
            Messages.SendCommandsAfterMakeLink(user);
        }

        







        private async void ChangeLinkInDataBase(MyUser user)
        {
            if (user.activeSearch)
                dataBase.SqlCommand(string.Format("UPDATE Active_search SET search_link = '{0}' WHERE Active_search.chat_id = {1};", user.searchLink, user.chatId));
            dataBase.SqlCommand(string.Format("UPDATE Users SET search_link = '{0}' WHERE Users.chat_id = {1};", user.searchLink, user.chatId));
            await telegramBot.SendTextMessageAsync(user.chatId, "Ссылка изменена");
        }
        private async void RegexLink(MyLink myLink, string url)
        {

            url = HttpUtility.UrlDecode(url);
            myLink.url = url;
            myLink.makeByConstructor = false;
            myLink.city = Regex.Match(url, @"https://www.avito.ru/(?<city>[^/?]+)").Groups["city"].Value;
            myLink.name = Regex.Match(url, @"q=(?<product>[^&]+)").Groups["product"].Value;
            myLink.priceFrom = string.IsNullOrEmpty(Regex.Match(url, @"&pmin=(?<priceMin>[^&]+)").Groups["priceMin"].Value) ? "" : Regex.Match(url, @"&pmin=(?<priceMin>[^&]+)").Groups["priceMin"].Value;
            myLink.priceTo = string.IsNullOrEmpty(Regex.Match(url, @"&pmax=(?<priceMax>[^&]+|$)").Groups["priceMax"].Value) ? "" : Regex.Match(url, @"&pmax=(?<priceMax>[^&]+|$)").Groups["priceMax"].Value;

        }
    }
}
