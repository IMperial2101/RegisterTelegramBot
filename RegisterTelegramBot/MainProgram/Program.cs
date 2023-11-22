using RegBot2.MainProgram;
using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using System;
using System.Text.RegularExpressions;
using Org.BouncyCastle.Asn1;


/*
 * Обработать все поля Private
 * Сделать добавление в историю ссылок ссылки не созданные через конструктор(через регулярку)
 * Сделеать то же самое с избранным
 */

namespace RegBot2
{
    partial class Program
    {
        public Program()
        {
            for (int i = 0; i < citiesFromDbRu.Count; i++)
            {
                citiesFromDbRu[i] = citiesFromDbRu[i].Trim().ToLower();
            }
        }
        static private Dictionary<string, int> callbackQueryToLinkNumber = new Dictionary<string, int>();
        static readonly TelegramBotClient telegramBot = new TelegramBotClient("6188385004:AAHsTOrCYA9H4x04plLGCIwM7JxKystTE2Q");
        static DataBase dataBase = StartProcedures.StartDataBaseConnection();
        //static readonly Dictionary<long, MyUser> Users = new Dictionary<long, MyUser>();
        static List<string> citiesFromDbRu = dataBase.SqlCommandGetOneColumn("Select city_name_ru FROM cities;");
        static List<string> citiesFromDbEn = dataBase.SqlCommandGetOneColumn("Select city_name_en FROM cities;");
        static List<string> allCommands = new List<string>(new string[] { "🆕Регистрация", "📄Инструкция", "⛔️Отменить процесс регистрации", "✅Да", "🔄Пройти заного", "➕Добавить ссылку", "🔁Изменить пароль", "🛠Создать ссылку", "⛔️Отменить создание ссылки", "⛔️Отменить добавление", "🔄изменить ссылку", "⛔️Остановить поиск", "✅Продолжить поиск", "💙Сохранить ссылку", "✅Сделать активной", "🔁Построить заново", "🧾Список команд", "📋Список ссылок", "💙Избранное", "💙Избранные ссылки", "🗃История", "🗃История ссылок", "🔗Выбрать ссылку", "⛔️Отменить изменение пароля", "✖️Удалить ссылку", "❌Удалить все", "➕Добавить вручную" });

        //Для заполнения словаря из бд 
        static readonly Dictionary<long, MyUser> Users = dataBase.FillDictionaryFromDB();
        static bool exitRequested = false;
        static void Main()
        {

            for (int i = 0; i < citiesFromDbRu.Count; i++)
            {
                citiesFromDbRu[i] = citiesFromDbRu[i].Trim().ToLower();
            }


            telegramBot.OnMessage += new BotOnMessageReceivedClass(dataBase, telegramBot, Users, citiesFromDbRu, citiesFromDbEn, allCommands,callbackQueryToLinkNumber)._BotOnMessageReceived;
            telegramBot.OnCallbackQuery += new BotOnCallbackQueryClass(dataBase, telegramBot, Users,callbackQueryToLinkNumber ).BotOnCallbackQuery;

            telegramBot.StartReceiving(Array.Empty<UpdateType>());

            var me = telegramBot.GetMeAsync();

            Console.WriteLine("{0} start receiving", me.Result);

            Console.ReadLine();


        }

        static void GetCitiesFromTxt()
        {
            string filePath = "D:\\Политех учёба\\2 курс\\2 семестр\\Базы Данных\\Курсовая\\ZennoPoster\\бот для парсинга\\Итоговый список всех городов с сылками.txt";
            List<string[]> citiesList = new List<string[]>();
            List<string> cities = new List<string>();
            string[] lineArr = new string[2];
            try
            {
                using (StreamReader reader = new StreamReader(filePath))
                {
                    int i = 0;
                    string line;
                    while ((line = reader.ReadLine()) != null)
                    {

                        Match match = Regex.Match(line, @"^(.*?)\s-\shttps://www.avito.ru/(.*?)(?:\s-\s.*?)?\?q=.*$");

                        if (match.Success)
                        {
                            string city = match.Groups[1].Value;  // Текст до знака '-'
                            string query = match.Groups[2].Value; // Текст после https://www.avito.ru/ и перед '?q=iphone'


                            if (cities.Contains(line))
                            {
                                Console.WriteLine(line);
                                continue;
                            }
                            cities.Add(line);
                            lineArr[0] = city;
                            lineArr[1] = query;
                            citiesList.Add(lineArr);
                            //Console.WriteLine(i + ": " + city + " - " + query);
                            //dataBase.SqlCommand($"INSERT INTO cities (city_name_ru, city_name_en) VALUES ('{city}','{query}')");

                            i++;
                        }
                    }
                    Console.ReadLine();
                }
            }
            catch (IOException e)
            {
                Console.WriteLine("Ошибка чтения файла: " + e.Message);
            }
        }
    }
}
