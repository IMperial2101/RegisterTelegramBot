
using Telegram.Bot;
using Telegram.Bot.Types.Enums;

namespace RegBot2
{
    internal static class CheckInfo
    {
        public static bool CheckLogin(string login,
                                      ref DataBase dataBase, long chatId, TelegramBotClient bot)
        {
            if (login.Length < 5 || login.Length > 30)
            {
                bot.SendTextMessageAsync(chatId, "<i>*Логин должен быть от 5 до 30 символов.</i>", ParseMode.Html);
                return false;
            }
            foreach (var item in dataBase.SqlCommandGetOneColumn("SELECT login FROM users"))
                if (login == item)
                {
                    bot.SendTextMessageAsync(chatId, "<i>*Такой логин уже зарегистрирован.</i>", ParseMode.Html);
                    return false;
                }
            return true;
        }
        public static bool CheckPassword(string password, long chatId, TelegramBotClient bot)
        {
            if (password.Length > 30 || password.Length < 5)
            {
                bot.SendTextMessageAsync(chatId, "<i>*Пароль должен быть от 5 до 30 символов.</i>", ParseMode.Html);
                return false;
            }
            return true;
        }
        public static bool CheckEmail(string email, ref DataBase dataBase, long chatId, TelegramBotClient bot)
        {
            foreach (var item in dataBase.SqlCommandGetOneColumn("SELECT email FROM users"))
                if (email == item)
                {
                    bot.SendTextMessageAsync(chatId, "<i>*Такая почта уже зарегистрирована.</i>", ParseMode.Html);
                    return false;
                }
            return true;
        }
        public static bool CheckLink(string link, long chatId, TelegramBotClient bot)
        {
            if (!link.Contains("www.avito.ru"))
            {
                bot.SendTextMessageAsync(chatId, "<i>*Это не авито ссылка.</i>", ParseMode.Html);
                return false;
            }
            return true;
        }

        public static bool CheckPrice(string price)
        {
            if (int.TryParse(price, out int number))
            {
                if (number >= 0 && number <= 100000000)
                {
                    return true;
                }
            }
            return false;
        }
        public static bool CheckLinkCount(string count, List<MyLink> listWithLinks)
        {
            if(int.TryParse(count, out int number))
            {
                if(number <= listWithLinks.Count && number >= 0 && number <= 10)
                {
                    return true;
                }
            }
            return false;
        }
    }
    
}
