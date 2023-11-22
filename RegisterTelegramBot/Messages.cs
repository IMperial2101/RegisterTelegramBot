using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace RegBot2
{
    static class Messages
    {
        public static TelegramBotClient botClient = new TelegramBotClient("6188385004:AAHsTOrCYA9H4x04plLGCIwM7JxKystTE2Q");


        public static async Task SendStartMessageAsync(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("🆕Регистрация"),
                    new KeyboardButton("📄Инструкция")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "<b>Добро пожаловать! </b>Перед вами Бот для регистрации\nВам доступно 2 команды, - регистрация и интструкция, рекомендуем ознокомиться с ней перед работой с ботом😊",
                replyMarkup: keyboard,
                parseMode: ParseMode.Html
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }

        }
        public static async Task SendConfirmRegisterMessageAsync(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("✅Да"),
                    new KeyboardButton("🔄Пройти заного"),
                    new KeyboardButton("⛔️Отменить процесс регистрации")
                }
            }, true);
            await botClient.SendTextMessageAsync(user.chatId,
                                                $"<b>Подтверждаете регистрацию?</b>\n\n" +
                                                $"Логин: {user.login}\n" +
                                                $"Email: {user.email}\n" +
                                                $"Пароль: {user.password}",
                                                ParseMode.Html,
                                                replyMarkup: keyboard);
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async Task SendInstructionMessageAsync(ChatId chatId)
        {
            string message = $"<b>Добро пожаловать!</b>\n" +
                $"Перед вами инструкция по работе с ботом для регистрации. \n" +
                $"Для начала вам нужно зарегистрироваться, ввести: Логин, Почту и Пароль. Если вы ввели какие-то данные неверно, то, в конце регистрации, можно будет пройти её заново. После успешной регистрации вам следует добавить ссылку для поиска, после этого вам станет доступен полный списко команд, <b>чтобы его раскрыть</b> нужно начать на квадратик с 4 точечками слева от микрофона.\n\n" +
                $"<b>Команды, которые станут доступны:</b>\n" +
                $"1️⃣ /info - посмотреть инструкцию(отправит это сообщение еще раз)\n" +
                $"2️⃣ /commands - отправить список команд в другой форме(в виде сообщений)\n" +
                $"3️⃣ <b>Список поисков</b> - покажет список ваших предыдущих поисков\n" +
                $"4️⃣ <b>Составить ссылку</b> - конструктор ссылки для поиска(можно составить новый поиск не выходя из чата с ботом) \n";

            await botClient.SendTextMessageAsync(chatId, text: message, ParseMode.Html);
        }
        public static async void SendCommandsNoLink(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("➕Добавить ссылку"),
                    new KeyboardButton("🔁Изменить пароль")
                },
                new[]
                {
                    new KeyboardButton("📄Инструкция"),
                    new KeyboardButton("🛠Создать ссылку")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "Список команд обновлен",
                replyMarkup: keyboard
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async void SendCommandsWithLink(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("⛔️Остановить поиск"),
                    new KeyboardButton("✅Продолжить поиск")


                },
                new[]
                {
                    new KeyboardButton("🔁Изменить пароль"),
                    new KeyboardButton("📄Инструкция")
                },
                new[]
                {
                    new KeyboardButton("🔄изменить ссылку"),
                    new KeyboardButton("📋Список ссылок"),

                },
                new[]
                {
                  new KeyboardButton("🛠Создать ссылку")
                },
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "Вам доступен следующий набор команд",
                replyMarkup: keyboard
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async void SendCommandsAfterMakeLink(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("💙Сохранить ссылку"),
                    new KeyboardButton("✅Сделать активной")


                },
                new[]
                {
                    new KeyboardButton("🔁Построить заново"),
                    new KeyboardButton("📄Инструкция")
                },
                new[]
                {
                    new KeyboardButton("📋Список ссылок"),
                    new KeyboardButton("🧾Список команд"),

                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "Вам доступен следующий набор команд",
                replyMarkup: keyboard
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async void SendKeyboardAddLink(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("🛠Создать ссылку"),
                    new KeyboardButton("⛔️Отменить добавление")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "С помощью команды /🛠Создать ссылку вы можете создать ссылку не выходя из чата.",
                replyMarkup: keyboard
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async void SendChangePasswordKeyboard(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("⛔️Отменить изменение пароля")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "Вы можете отменить изменение пароля",
                replyMarkup: keyboard
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }

        }
        public static async void SendMakeLinkKeyboard(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
              {
                new[]
                {
                    new KeyboardButton("⛔️Отменить создание ссылки")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "Вы можете отменить создание ссылки",
                replyMarkup: keyboard
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async void SendCommandsCheckLinks(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("💙Избранное"),
                    new KeyboardButton("🗃История")
                },
                new[]
                {
                    new KeyboardButton("🧾Список команд"),
                    new KeyboardButton("🛠Создать ссылку")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "Также вы можете выбрать ссылку из <b>избранных</b> или <b>истории </b> ",
                replyMarkup: keyboard, parseMode: ParseMode.Html
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async Task SendCommandsChooseLink(MyUser user, bool history = true)
        {
            string messageText;
            if (history)
                messageText = "История создания ссыллок";
            else
                messageText = "Избранные ссылки";
            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("💙Избранное"),
                    new KeyboardButton("🗃История")
                },
                new[]
                {
                    new KeyboardButton("🧾Список команд"),
                    new KeyboardButton("🔗Выбрать ссылку")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: messageText,
                replyMarkup: keyboard, parseMode: ParseMode.Html
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async Task SendCommandsLinksAction(MyUser user)
        {
            string messageText = "Выберите ссылки";

            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("💙Избранные ссылки"),
                    new KeyboardButton("🗃История ссылок")
                },
                new[]
                {
                    new KeyboardButton("🧾Список команд"),
                    new KeyboardButton("🛠Создать ссылку")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: messageText,
                replyMarkup: keyboard, parseMode: ParseMode.Html
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }

        }
        public static async Task SendCommandsLinksActionHistory(MyUser user)
        {
            string messageText = "Вы можете изменить список истории";

            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("💙Избранные ссылки"),
                },
                new[]
                {
                    new KeyboardButton("🧾Список команд"),
                    new KeyboardButton("❌Удалить все")
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: messageText,
                replyMarkup: keyboard, parseMode: ParseMode.Html
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
        public static async Task SendCommandsLinksActionSaved(MyUser user)
        {
            string messageText = "Вы можете изменить список избранных ссылок";

            var keyboard = new ReplyKeyboardMarkup(new[]
               {
                new[]
                {
                    new KeyboardButton("🗃История ссылок"),
                },
                new[]
                {
                    new KeyboardButton("🧾Список команд"),
                    new KeyboardButton("🛠Создать ссылку")
                },
                new[]
                {
                    new KeyboardButton("✖️Удалить ссылку"),
                    new KeyboardButton("➕Добавить вручную")
                },
                new[]
                {
                    new KeyboardButton("❌Удалить все")
                    
                }
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: messageText,
                replyMarkup: keyboard, parseMode: ParseMode.Html
            );
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }


        public static async void SendMessageCancelRegistration(MyUser user)
        {
            var keyboard = new ReplyKeyboardMarkup(new[]{
                    new KeyboardButton("⛔️Отменить процесс регистрации"),
            }, true);

            await botClient.SendTextMessageAsync(
                chatId: user.chatId,
                text: "Вы можете отменить процесс регистрации с помощью кнопки",
                replyMarkup: keyboard);
            user.availableCommands.Clear();
            foreach (var commandArr in keyboard.Keyboard.ToList())
            {
                foreach (var command in commandArr)
                {
                    user.availableCommands.Add(command.Text);
                }
            }
        }
    }



}
