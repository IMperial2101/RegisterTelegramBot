using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using Telegram.Bot.Args;
using Telegram.Bot.Types;

namespace RegBot2.MainProgram
{
    partial class BotOnCallbackQueryClass
    {
        private async void HistoryDeleteLink(MyUser user, CallbackQueryEventArgs callbackQuery,string callbackQueryID)
        {

            if (callbackQueryToLinkNumber.TryGetValue(callbackQueryID, out int number))
            {
                user.AutoDeleteLink(dataBase, number, true);
                await telegramBot.SendTextMessageAsync(user.chatId, "Ссылка удалена");
                await telegramBot.DeleteMessageAsync(user.chatId, callbackQuery.CallbackQuery.Message.MessageId);
            }

        }
        private async void SavedDeleteLink(MyUser user, CallbackQueryEventArgs callbackQuery,int number)
        {

           user.AutoDeleteLink(dataBase, number, false);
           await telegramBot.SendTextMessageAsync(user.chatId, "Ссылка удалена");
            
           await telegramBot.DeleteMessageAsync(user.chatId, callbackQuery.CallbackQuery.Message.MessageId);

        }
        private async void HistoryMakeActive(MyUser user, CallbackQueryEventArgs callbackQuery, int number)
        {
            dataBase.sqlCommands.AutoUpdateNewSearchLinkInDB(user, user.MyLinksHistoryList[number].url);
        }
        private async void SavedMakeActive(MyUser user, CallbackQueryEventArgs callbackQuery, int number)
        {
            dataBase.sqlCommands.AutoUpdateNewSearchLinkInDB(user, user.MyLinksSavedList[number].url);
        }
    }
}
