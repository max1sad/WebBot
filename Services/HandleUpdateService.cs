using Telegram.Bot.Exceptions;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.InlineQueryResults;
using Telegram.Bot.Types.InputFiles;
using Telegram.Bot.Types.ReplyMarkups;
using Telegram.Bot;
using FootballTelegramBot.Model;


namespace FootballTelegramBot.Services
{


public class HandleUpdateService
{
    private readonly ITelegramBotClient _botClient;
    private readonly ILogger<HandleUpdateService> _logger;
    public static DBProvide dBProvide = new DBProvide();
        //public static DBConnection connectsql = new DBConnection();
        //результат запроса к БД
        public static string resultSql = "";
        //строка запроса пишется
        public static string sqlStr = "";
        //метка, которая устнавливает числовое знгачение , определяет этапы диалогов для разных веток общения.
        //от 1-3 это этапы создания заявки на участие в турнире
        //4-5  это запрос на вывод турнирной таблицы.
        public static int levelAplly = 0;
        public static int levelAdminClick = 0;
        public static string adminGameTimes = "";
        //метка на проверку была ли нажата кнопка в начале действий
        static bool labelClick = false;
        //название кнопки на которую совершили нажатие
        public static string actionChoice = "";       
        //фомируются исодные данные для принятия заявки на турнир
        public static string applicationTournament = "";
        public static string applicationChek = "";


    public HandleUpdateService(ITelegramBotClient botClient, ILogger<HandleUpdateService> logger)
    {
        _botClient = botClient;
        _logger = logger;       
    }

        public async Task EchoAsync(Update update)
    {
         StringParcers stringParcers = new StringParcers();
            var message = update.Message;
            //результат ответа от телеграмма выводится
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(update));
            //бот случает чат телеграмма на новые сообщения
            if (update.Type == Telegram.Bot.Types.Enums.UpdateType.Message)
            {               
                if (message.Text == "/start")
                {
                    //очищаем все переменные, бот работает всегда, следовательно если новые действия будут, что бы не вело в заблуждение.
                    labelClick = true;
                    actionChoice = "";
                    sqlStr = "";
                    applicationChek = "";
                    levelAplly = 0;
                    applicationTournament = "";
                    await _botClient.SendTextMessageAsync(message.Chat.Id,"Привет человек");
                    //вывод кнопок для совершения действий
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Выбири действия", replyMarkup: buttonTest());
                    return;
                }
                if (message.Text == "/admin")
                {
                    levelAdminClick = 0;
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Выбири действия", replyMarkup: adminMenu());
                    return;
                }
                if (message.Text == "Заявки")
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Инструкция: после ознакомления со списком игроков, укажите номер заявки и ее статус \"yes или no\"");
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Пример: 1:yes;");
                    levelAdminClick = 1;
                    return;
                }
                if (message.Text == "Добавить расписание")
                {
                    await _botClient.SendTextMessageAsync(message.Chat.Id, "Выбери лигу, указав номер");
                    //вызом метода для вывода из данных из БД
                    resultSql = string.Join("\n", dBProvide.GetAllLeague().Select(l => $"{l.nameLeague} -Лига"));
                    await _botClient.SendTextMessageAsync(message.Chat.Id, resultSql);
                    levelAdminClick = 10;
                    return;
                }
                
                //реализация меню администратора
                switch (levelAdminClick)
                {
                    case 1:
                        if (stringParcers.checkAdminText(message.Text,levelAdminClick) == true)
                        {
                            List<string> stringArray = new List<string>();
                            stringArray = stringParcers.sqlParcerApply(message.Text);
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не верно, повторите ввод");
                            levelAdminClick = 1;
                        }
                        return;
                    case 10:
                        
                        if (stringParcers.inputNumberLeague(message.Text,1) == true) 
                        {
                            adminGameTimes = message.Text + ";";
                            resultSql = string.Join("\n", dBProvide.GetAllTourney().Select(l => l.idNameTourney + " - " + l.nameTourney));
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Выбери номер турнира, для которого составить расписание");
                            //вывод пользователю список турниров с номерами
                            await _botClient.SendTextMessageAsync(message.Chat.Id, resultSql);
                            levelAdminClick = 11;
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAdminClick = 10;
                        }
                        return;
                    case 11:
                        if (stringParcers.inputNumberLeague(message.Text, 1) == true)
                        {
                            adminGameTimes += message.Text + ";";
                            List<string> stringArray = new List<string>();
                            stringArray = stringParcers.sqlParcerApply(adminGameTimes);
                            Console.WriteLine(stringArray[0] + "    " + stringArray[1]);
                            resultSql = string.Join(";", dBProvide.GetTourneyTeams().Where(l => l.idLeague == int.Parse(stringArray[0]) && l.idNameTourney == int.Parse(stringArray[1])).Select(
                                l => l.idTeams));
                            resultSql += ";";
                            stringArray.Clear();
                            stringArray = stringParcers.sqlParcerApply(resultSql);
                            resultSql = "";
                            //получаем список команд учавствующих в определнной лиге и тернире.
                            for (int i=0; i < stringArray.Count; i++)
                            {
                                resultSql += string.Join("", dBProvide.GetAllTeams().Where(l => l.idTeams == int.Parse(stringArray[i])).Select(l => l.idTeams + " - " + l.nameTeams))+"\n";
                            }                            
                            await _botClient.SendTextMessageAsync(message.Chat.Id, resultSql);
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Введите в формате(номер команды1:номер команды2;20-05-22 15:25");
                            levelAdminClick = 12;
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAdminClick = 11;
                        }
                            return;
                    case 12:
                        if (stringParcers.checkAdminText(message.Text,levelAdminClick) == true)
                        {
                            //добавление расписания игры 
                            List<string> stringArray = new List<string>();
                            stringArray = stringParcers.sqlParcerApply(message.Text+";");
                            Console.WriteLine(DateTime.Parse(stringArray[2]+":"+stringArray[3]));
                            string dat =  DateTime.Parse(stringArray[2] + ":" + stringArray[3]).ToString("dd-MM-yy HH:mm");
                           // Model.Match sad = new Model.Match();
                            dBProvide.CreateMatchTimeGame(new Model.Match { idTeams1 = int.Parse(stringArray[0]), idTeams2 = int.Parse(stringArray[1]),
                            dateGames = DateTime.Parse(dat)
                            });
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не верный формат, повторите попытку");
                            levelAdminClick = 12;
                        }
                        return;
                        
                }
                //проверка что после нажатия кнопки было совершон ответ
                switch (levelAplly)
                {                                           
                    case 1:
                        Console.WriteLine(actionChoice);
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, levelAplly) == true)
                        {
                            applicationTournament = message.Text + ";";
                            //запрос на список турниров для участия.
                            resultSql = string.Join("\n", dBProvide.GetAllTourney().Select(l => l.idNameTourney +" - "+ l.nameTourney));                           
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Выбери номер турнира, в котором планируешь учавствовать");
                            //вывод пользователю список турниров с номерами
                            await _botClient.SendTextMessageAsync(message.Chat.Id, resultSql);
                            //Console.WriteLine(applicationTournament);
                            actionChoice = "";
                            levelAplly = 2;
                            applicationChek = "";
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAplly = 1;
                        }                        
                        return;
                    case 2:
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, levelAplly) == true)
                        {
                            //строка в котрой имеется номер лиги и номер турнира
                            applicationTournament += message.Text + ";";
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Введити данные о команде в формате \"Название команды:Игрок1;Игрок2;\" и т.д. Заканчивается знаком \";\"");
                            Console.WriteLine(applicationTournament);
                            levelAplly = 3;
                            applicationChek = "";
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAplly = 2;
                        }                       
                        return;
                    case 3:
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, levelAplly) == true)
                        {
                            string idPlayers = "";
                            List<string> stringArray = new List<string>();
                            applicationTournament += message.Text;
                            Console.WriteLine(applicationTournament);
                            //вызываем метод для обработки строки в нужный формат.
                            stringArray = stringParcers.sqlParcerApply(applicationTournament);
                            Console.WriteLine(message.Chat.Id);                                                        
                            //добавляем команду в таблицу
                            dBProvide.CreateTeams(new Model.Team(){nameTeams = stringArray[2]});
                            string idTeams = string.Join("\n", dBProvide.GetAllTeams().Where(l => l.nameTeams == stringArray[2]).Select(l => l.idTeams));
                            //запрос с номером 100- это вывод 1-го поля с типом числовым.
                            // добавляем команду в турнир
                            dBProvide.CreateTourneyTeams(new Model.TourneyTeams() 
                            { idLeague = int.Parse(stringArray[0]),idNameTourney = int.Parse(stringArray[1]),idTeams = int.Parse(idTeams), statusRequest = "Waiting for confirmation" });
                            //добавялем всех игроков в таблицу игроки и первым запросом считаекм что капитан подал заявку и его номер пишем тоже.
                            dBProvide.CreatePlayerCapitan(new Model.Player() { FIO = stringArray[3], idPhone = message.Chat.Id.ToString() });
                            idPlayers = string.Join("\n", dBProvide.GetAllPlayer().Where(l => l.FIO == stringArray[3]).Select(l => l.idPlayer));
                            Console.WriteLine(idPlayers);
                            dBProvide.CreatePlayerTeamsCapitan(new Model.PlayerTeams() { idTeams = int.Parse(idTeams), teamCaptain = "yes", idPlayer = int.Parse(idPlayers) });
                            for (int i = 4; i < stringArray.Count; i++)
                            {
                                dBProvide.CreatePlayer(new Model.Player() { FIO = stringArray[i] });
                                idPlayers = string.Join("\n", dBProvide.GetAllPlayer().Where(l => l.FIO == stringArray[i]).Select(l => l.idPlayer));
                                dBProvide.CreatePlayerTeams(new Model.PlayerTeams() { idTeams = int.Parse(idTeams), idPlayer = int.Parse(idPlayers) });
                            }

                            //написать запрос добавления игрока в команду , id команды уже получал в переменную str;
                            Console.WriteLine(sqlStr);

                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Спасибо, заявка будет рассмотренна в ближайшее время");
                            
                            resultSql = string.Join("\n", dBProvide.GetTourneyTeams().Where(l => l.idTeams == int.Parse(idTeams)).Select(l => l.idTourney));
                            await _botClient.SendTextMessageAsync(772376797, "Формат заявки: "+applicationTournament+ "  Номер заявки "+ resultSql);
                            
                            levelAplly = 0;
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Не соответствует введенному формату, повторите ввод(в именах только русские буквы)");
                            levelAplly = 3;
                        }                        
                        return;
                    case 4:
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, 1) == true)
                        {
                            applicationTournament = message.Text + ";";
                            //sqlStr = "select idNameTourney,nameTourney from nameTourney where statusToutney is null";
                            resultSql = string.Join("\n", dBProvide.GetAllTourney().Select(l => l.idNameTourney + "-" + l.nameTourney));
                            //actionChoice = "apply1";
                            //resultSql = connectsql.SqlRead(sqlStr, 1);
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Выбери номер турнира, для просмотра турнирной таблицы");
                            //вывод пользователю список турниров с номерами
                            await _botClient.SendTextMessageAsync(message.Chat.Id, resultSql);
                            //Console.WriteLine(applicationTournament);
                            actionChoice = "";
                            levelAplly = 5;
                            applicationChek = "";
                            //levelAplly1 = true;
                            sqlStr = "";
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAplly = 4;
                        }
                        return;
                    case 5:
                        applicationChek = message.Text;
                        if (stringParcers.inputNumberLeague(applicationChek, 1) == true)
                        {
                            applicationTournament += message.Text + ";";
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Запрос к базе на вывод турнирной таблицы");
                            Console.WriteLine(applicationTournament);
                            levelAplly = 0;
                        }
                        else
                        {
                            await _botClient.SendTextMessageAsync(message.Chat.Id, "Ввели не числи. Напишите цифру соответствущую");
                            levelAplly = 5;
                        }
                        return;
                    default:
                        Console.WriteLine("Не указали параметр");
                        await _botClient.SendTextMessageAsync(message.Chat.Id, "Для начала работы напиши \"/start\"");
                        return;
                }              
                //await _botClient.SendTextMessageAsync(message.Chat.Id, "Для начала работы напиши \"/start\"");                
            }
            Console.WriteLine(labelClick);
            //проверка на какую кнопку нажали, происходит формирвоание наименования кнопки, а так же в переменную levlApply указывается номер по которому проверка в какой диалог входить.
            if (labelClick == true)
            {
                //Console.WriteLine("Не заходит в условие");
                labelClick = false;
                CallbackQuery callback = update.CallbackQuery;
                //проверка было ли какое то действие связнное с CallbackQuery, проверка на какую кнопку нажато- 
                var updateCallbackQuery = update.CallbackQuery;
                switch (callback.Data)
                {
                    case "tourneyTable":
                        Console.WriteLine("кнопка нажата");
                        actionChoice = "tourneyTable";
                        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery & actionChoice == "tourneyTable")
                        {
                            await _botClient.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, "Выбери лигу, указав номер");
                            //вывод списка лиг из бд
                            //sqlStr = "SELECT nameLeague FROM league";
                            //вызом метода для вывода из данных из БД. передача двух параметров. строка запроса и название кнопки на которую нажали
                            resultSql = string.Join("\n", dBProvide.GetAllLeague().Select(l => $"{l.nameLeague} -Лига"));
                            await _botClient.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, resultSql);
                            levelAplly = 4;
                            //Console.WriteLine(s);
                        }
                        break;
                    case "gameSchedule":
                        Console.WriteLine("кнопка нажата1");
                        actionChoice = "gameSchedule";
                        break;
                    case "apply":
                        Console.WriteLine("кнопка нажата2");
                        actionChoice = "apply";
                        if (update.Type == Telegram.Bot.Types.Enums.UpdateType.CallbackQuery & actionChoice == "apply")
                        {
                            await _botClient.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, "Выбери лигу, указав номер");
                            //вызом метода для вывода из данных из БД
                            resultSql = string.Join("\n", dBProvide.GetAllLeague().Select(l => $"{l.nameLeague} -Лига")) ;
                            await _botClient.SendTextMessageAsync(updateCallbackQuery.Message.Chat.Id, resultSql);
                            levelAplly = 1;
                            //Console.WriteLine(s);
                        }
                        break;
                }               
            }

       
    }
     
    //функция выводит кнопки администратора
       private static ReplyKeyboardMarkup adminMenu()
        {
            ReplyKeyboardMarkup replyKeyboardMarkup = new ReplyKeyboardMarkup(
                new[]
                {
                    new KeyboardButton[]{"Добавить расписание","Заявки"}
                }
                )
            {
                ResizeKeyboard = true,
                OneTimeKeyboard = true
                
            };
            return replyKeyboardMarkup;
        }
        //кнопки для стартового выбора действий обычного пользователя
        private static IReplyMarkup buttonTest()
        {
            var ikm = new InlineKeyboardMarkup(new[]
            {
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Турнирная таблица", "tourneyTable"),
                    InlineKeyboardButton.WithCallbackData("Расписание игр", "gameSchedule"),
                },
                new[]
                {
                    InlineKeyboardButton.WithCallbackData("Подать заявку", "apply"),
                },
            });
            return ikm;
        }
        
        public static async Task HandleErrorAsync(ITelegramBotClient _botClient, Exception exception, CancellationToken cancellationToken)
        {
            Console.WriteLine(Newtonsoft.Json.JsonConvert.SerializeObject(exception));
        }
}
}