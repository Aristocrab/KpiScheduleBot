using KpiSchedule.Api;
using KpiSchedule.Api.Entities.Lessons;
using KpiSchedule.Database;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KpiSchedule.TelegramBot;

public class CommandsController
{
    private readonly ITelegramBotClient _botClient;
    private readonly ScheduleService _scheduleService;
    private readonly KpiScheduleDbContext _dbContext;
    private readonly ILogger _logger;
    private ChatSettings _currentChatSettings = null!;

    public CommandsController(ITelegramBotClient botClient, ScheduleService scheduleService, KpiScheduleDbContext dbContext, ILogger logger)
    {
        _botClient = botClient;
        _scheduleService = scheduleService;
        _dbContext = dbContext;
        _logger = logger;
    }
    
    public async Task HandleCommand(Update update)
    {
        if (update.Message?.Text is null) return;
        
        var message = update.Message;
        var commandText = message.Text.Split(' ')[0].ToLower();
        
        _logger.Information("ChatId: {ChatId}, command '{Command}'", message.Chat.Id, commandText);

        var settings = _dbContext.ChatsSettings.FirstOrDefault(x => x.ChatId == message.Chat.Id);
        if (settings is null && commandText != "/g")
        {
            await Start(message.Chat.Id);
            return;
        }
        if (settings != null)
        {
            _currentChatSettings = settings;
        }
        
        switch (commandText)
        {
            case "/g":
                await SelectGroup(message.Chat.Id, message.Text);
                break;
            case "/today":
                await Today(message.Chat.Id);
                break;
            case "/tomorrow":
                await Tomorrow(message.Chat.Id);
                break;
            case "/week":
                await Week(message.Chat.Id);
                break;
            case "/nextweek":
                await NextWeek(message.Chat.Id);
                break;
            case "/full":
                await Full(message.Chat.Id);
                break;
            case "/timetable":
                await TimeTable(message.Chat.Id);
                break;
            case "/who":
                await Who(message.Chat.Id);
                break;
            case "/left":
                await Left(message.Chat.Id);
                break;
            case "/exam":
            case "/exams":
                await Exams(message.Chat.Id);
                break;
            case "/start":
                await Start(message.Chat.Id);
                break;
            case "/help":
                await Help(message.Chat.Id);
                break;
            default:
                _logger.Warning("ChatId: {ChatId}, command '{Command}' was not found", message.Chat.Id, message.Text);
                await Start(message.Chat.Id);
                break;
        }
    }

    public async Task Start(long chatId)
    {
        await _botClient.SendTextMessageAsync(chatId, 
            "*Вас вітає бот з розкладу КПІ*\n\nОберіть групу командою /g\nПриклад: /g IC-12", 
            ParseMode.Markdown);
    }

    public async Task Help(long chatId)
    {
        await _botClient.SendTextMessageAsync(chatId, 
            """
            *Вас вітає бот з розкладу КПІ*

            Список команд:
            /g - Змінити групу. Приклад: `/g ІС-12`

            /today - Розклад на сьогодні
            /tomorrow - Розклад на завтра

            /week - Розклад на цей тиждень
            /nextweek - Розклад на наступний тиждень
            /full - Повний розклад за 2 тижні

            /exams - Розклад екзаменів
            /who - Ім'я викладача поточної пари
            /timetable - Таблиця тривалості пар
            /left - Час до кінця пари

            /help - Список команд
            """, 
            ParseMode.Markdown);
    }

    public async Task SelectGroup(long chatId, string messageText)
    {
        if (messageText.Split(' ').Length < 2)
        {
            await Start(chatId);
            return;
        }

        var groupCode = messageText.Split(' ')[1];
        var group = await _scheduleService.GetGroupByCode(groupCode);

        if (group is null)
        {
            await _botClient.SendTextMessageAsync(chatId, "Групу не знайдено. Приклад: '/g IC-12'");
        }
        else
        {
            var userSettings = _dbContext.ChatsSettings.FirstOrDefault(x => x.ChatId == chatId);
            if (userSettings is null)
            {
                var newUserSettings = new ChatSettings
                {
                    ChatId = chatId,
                    GroupCode = groupCode,
                    GroupId = group.Id
                };
                _dbContext.ChatsSettings.Add(newUserSettings);
                
                _currentChatSettings = newUserSettings;
            }
            else
            {
                userSettings.GroupCode = groupCode;
                userSettings.GroupId = group.Id;
                _currentChatSettings = userSettings;
            }
            
            await _dbContext.SaveChangesAsync();
            
            await _botClient.SendTextMessageAsync(chatId, 
                $"""
                Обрано групу: *{_currentChatSettings.GroupCode}*

                Приклади команд:
                /today - Розклад на сьогодні
                /tomorrow - Розклад на завтра

                /week - Розклад на цей тиждень
                /nextweek - Розклад на наступний тиждень
                /full - Повний розклад за 2 тижні

                /help - Повний список команд
                """, 
                ParseMode.Markdown);
        }
    }

    private static string BuildDaySchedule(DaySchedule daySchedule)
    {
        if (daySchedule.Lessons.Count == 0 && daySchedule.DayName == "Сб")
        {
            return string.Empty;
        }
        
        var ret = "*" + daySchedule.DayName + ":*\n";
        var orderedLessons = daySchedule.Lessons
            .OrderBy(x => ScheduleService.GetLessonIdByStartTime(x.Time) + 1)
            .GroupBy(x => ScheduleService.GetLessonIdByStartTime(x.Time) + 1);
        
        foreach (var lessonNumber in orderedLessons)
        {
            ret += lessonNumber.Key + ". ";
            ret += string.Join("\n    ", lessonNumber.Select(x => x.Name));
            ret += "\n";
        }

        return ret;
    }
    
    public async Task Today(long chatId)
    {
        var today = await _scheduleService.GetTodaySchedule(_currentChatSettings.GroupId);
        if (today is null)
        {
            await _botClient.SendTextMessageAsync(chatId, "Сьогодні пар немає");
            return;
        }
        
        await _botClient.SendTextMessageAsync(chatId, BuildDaySchedule(today), ParseMode.Markdown);
    }
    
    public async Task Tomorrow(long chatId)
    {
        var tomorrow = await _scheduleService.GetTomorrowSchedule(_currentChatSettings.GroupId);
        if (tomorrow is null)
        {
            await _botClient.SendTextMessageAsync(chatId, "Завтра пар немає");
            return;
        }
        
        await _botClient.SendTextMessageAsync(chatId, BuildDaySchedule(tomorrow), ParseMode.Markdown);
    }
    
    public async Task Week(long chatId)
    {
        var currentWeek = (await _scheduleService.GetCurrentTime()).CurrentWeek;
        await ScheduleByWeek(chatId, currentWeek);
    }
    
    public async Task NextWeek(long chatId)
    {
        var nextWeek = (await _scheduleService.GetCurrentTime()).CurrentWeek;
        await ScheduleByWeek(chatId, (nextWeek + 1) % 2);
    }
    
    public async Task Full(long chatId)
    {
        await ScheduleByWeek(chatId, 0);
        await ScheduleByWeek(chatId, 1);
    }

    private async Task ScheduleByWeek(long chatId, int week)
    {
        var weekSchedule = await _scheduleService.GetWeekSchedule(_currentChatSettings.GroupId, week);
        var ret = $"*{week+1}-ий тиждень*\n";

        if (weekSchedule is not null)
        {
            foreach (var day in weekSchedule)
            {
                ret += BuildDaySchedule(day);

                ret += "\n";
            }
        }
        else
        {
            ret = "Групу не знайдено";
        }

        await _botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
    }
    
    public async Task TimeTable(long chatId)
    {
        var ret = """
        *1 пара.* 08:30 - 10:05
        *2 пара.* 10:25 - 12:00
        *3 пара.* 12:20 - 13:55
        *4 пара.* 14:15 - 15:50
        *5 пара.* 16:10 - 17:45
        """;
        
        await _botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
    }
    
    public async Task Who(long chatId)
    {
        var currentLesson = await _scheduleService.GetCurrentLesson(_currentChatSettings.GroupId);

        if (currentLesson is null)
        {
            await _botClient.SendTextMessageAsync(chatId, "Зараз пар немає", ParseMode.Markdown);
        }
        else
        {
            var ret = $"""
            {currentLesson.TeacherName}
            _{currentLesson.Name}_
            """;
            await _botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
        }
    }
    
    public async Task Left(long chatId)
    {
        var time = await _scheduleService.GetCurrentTime();

        string ret;
        if (time.CurrentLesson > 4)
        {
            ret = "Зараз пар немає";
        }
        else
        {
            var currentLessonEndTime = ScheduleService.GetTimeTable().ElementAt(time.CurrentLesson-1).Value;
            var now = TimeOnly.FromDateTime(DateTime.Now);
            var diff = currentLessonEndTime - now;

            if (diff.Hours > 0 && diff.Minutes > 0)
            {
                ret = $"Залишилось: {diff.Hours} годин {diff.Minutes} хвилин {diff.Seconds} секунд";
            }
            else
            {
                ret = "Зараз пар немає";
            }
        }

        await _botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
    }
    
    public async Task Exams(long chatId)
    {
        var exams = await _scheduleService.GetExams(_currentChatSettings.GroupId);

        if (exams is null)
        {
            await _botClient.SendTextMessageAsync(chatId, "Екзамени не знайдено", ParseMode.Markdown);
            return;
        }
        
        var ret = "";
        foreach (var exam in exams)
        {
            ret += $"*{exam.Subject}*\n_{exam.LecturerName}_\n{exam.Date:dd.MM.yyyy HH:mm}. ({exam.Room})\n\n";
        }

        await _botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
    }
}