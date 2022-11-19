using Telegram.Bot;

namespace BotWebHooksProdject.Services
{
    public class BotSendingNotifications
    {
        public static string botToken = "5444116745:AAEiHTg9bpEQLHA7sEGj_c7SpCTg6AXMVPY";
        
        public async Task CloseZayvkaAsync(string numTel,string nameComand)
        {
            var bot = new TelegramBotClient(botToken);
            await bot.SendTextMessageAsync(numTel, "Для команды( "+nameComand+" ) заявка на участие в турнире отклонена ");
        }
    }
}
