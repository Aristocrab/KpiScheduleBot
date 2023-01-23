using KpiSchedule.Api;
using KpiSchedule.Api.Entities.Lessons;
using KpiSchedule.Database;
using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KpiSchedule.TelegramBot;

public class CommandsController
{
    private readonly ScheduleService _scheduleService;
    private readonly KpiScheduleDbContext _dbContext;
    private ChatSettings _currentChat = null!;

    public CommandsController(ScheduleService scheduleService, KpiScheduleDbContext dbContext)
    {
        _scheduleService = scheduleService;
        _dbContext = dbContext;
    }
    
    public async Task HandleCommand(ITelegramBotClient botClient, Update update)
    {
        if (update.Message?.Text is null) return;
        var message = update.Message;
        var commandText = update.Message.Text.Split(' ')[0].ToLower();

        var settings = _dbContext.ChatsSettings.FirstOrDefault(x => x.ChatId == update.Message.Chat.Id);
        if (settings is null && commandText != "/g")
        {
            await Start(botClient, update.Message.Chat.Id);
            return;
        }

        if (settings != null)
        {
            _currentChat = settings;
        }
        
        switch (commandText)
        {
            case "/g":
                await SelectGroup(botClient, message.Chat.Id, update.Message.Text);
                break;
            case "/today":
                await Today(botClient, message.Chat.Id);
                break;
            case "/tomorrow":
                await Tomorrow(botClient, message.Chat.Id);
                break;
            case "/week":
                await Week(botClient, message.Chat.Id);
                break;
            case "/nextweek":
                await NextWeek(botClient, message.Chat.Id);
                break;
            case "/timetable":
                await TimeTable(botClient, message.Chat.Id);
                break;
            case "/who":
                await Who(botClient, message.Chat.Id);
                break;
            case "/left":
                await Left(botClient, message.Chat.Id);
                break;
            case "/exam":
            case "/exams":
                await Exams(botClient, message.Chat.Id);
                break;
            default:
                await Start(botClient, message.Chat.Id);
                break;
        }
    }

    public async Task Start(ITelegramBotClient botClient, long chatId)
    {
        await botClient.SendTextMessageAsync(chatId, 
            "*Вас вітає бот з розкладу КПІ*\n\nОберіть групу командою /g\nПриклад: /g IC-12", 
            ParseMode.Markdown);
    }

    public async Task SelectGroup(ITelegramBotClient botClient, long chatId, string messageText)
    {
        if (messageText.Split(' ').Length < 2)
        {
            await Start(botClient, chatId);
            return;
        }

        var groupCode = messageText.Split(' ')[1];
        var group = await _scheduleService.GetGroupByCode(groupCode);

        if (group is null)
        {
            await botClient.SendTextMessageAsync(chatId, "Групу не знайдено. Приклад: '/g IC-12'");
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
                
                _currentChat = newUserSettings;
            }
            else
            {
                userSettings.GroupCode = groupCode;
                userSettings.GroupId = group.Id;
                _currentChat = userSettings;
            }
            
            await _dbContext.SaveChangesAsync();
            
            await botClient.SendTextMessageAsync(chatId, 
                $"Обрано групу: *{_currentChat.GroupCode}*\n\n" +
                "Приклади команд:\n/today\n/tomorrow\n/week\n/nextweek", 
                ParseMode.Markdown);
        }
    }

    private static string BuildDaySchedule(DaySchedule daySchedule)
    {
        var ret = "*" + daySchedule.DayName + ":*\n";
        var orderedLessons = daySchedule.Lessons
            .OrderBy(x => ScheduleService.GetLessonIdByStartTime(x.Time) + 1)
            .GroupBy(x => ScheduleService.GetLessonIdByStartTime(x.Time) + 1);
        
        foreach (var lessonNumber in orderedLessons)
        {
            foreach (var lesson in lessonNumber)
            {
                ret += lessonNumber.Key + ". ";
                ret += lesson.Name + "\n";
            }
        }

        return ret;
    }
    
    public async Task Today(ITelegramBotClient botClient, long chatId)
    {
        var today = await _scheduleService.GetTodaySchedule(_currentChat.GroupId);
        if (today is null)
        {
            await botClient.SendTextMessageAsync(chatId, "Сьогодні пар немає");
            return;
        }
        
        await botClient.SendTextMessageAsync(chatId, BuildDaySchedule(today), ParseMode.Markdown);
    }
    
    public async Task Tomorrow(ITelegramBotClient botClient, long chatId)
    {
        var tomorrow = await _scheduleService.GetTomorrowSchedule(_currentChat.GroupId);
        if (tomorrow is null)
        {
            await botClient.SendTextMessageAsync(chatId, "Завтра пар немає");
            return;
        }
        
        await botClient.SendTextMessageAsync(chatId, BuildDaySchedule(tomorrow), ParseMode.Markdown);
    }
    
    public async Task Week(ITelegramBotClient botClient, long chatId)
    {
        var currentWeek = await _scheduleService.GetCurrentWeek(_currentChat.GroupId);
        await ScheduleByWeek(botClient, chatId, currentWeek);
    }
    
    public async Task NextWeek(ITelegramBotClient botClient, long chatId)
    {
        var nextWeek = await _scheduleService.GetNextWeek(_currentChat.GroupId);
        await ScheduleByWeek(botClient, chatId, nextWeek);
    }

    private async Task ScheduleByWeek(ITelegramBotClient botClient, long chatId, List<DaySchedule>? week)
    {
        var ret = "";

        if (week is not null)
        {
            foreach (var day in week)
            {
                ret += BuildDaySchedule(day);

                ret += "\n";
            }
        }
        else
        {
            ret = "Групу не знайдено";
        }

        await botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
    }
    
    public async Task TimeTable(ITelegramBotClient botClient, long chatId)
    {
        var ret = """
        *1 пара.* 08:30 - 10:05
        *2 пара.* 10:25 - 12:00
        *3 пара.* 12:20 - 13:55
        *4 пара.* 14:15 - 15:50
        *5 пара.* 16:10 - 17:45
        """;
        
        await botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
    }
    
    public async Task Who(ITelegramBotClient botClient, long chatId)
    {
        var currentLesson = await _scheduleService.GetCurrentLesson(_currentChat.GroupId);

        if (currentLesson is null)
        {
            await botClient.SendTextMessageAsync(chatId, "Зараз пар немає", ParseMode.Markdown);
        }
        else
        {
            await botClient.SendTextMessageAsync(chatId, currentLesson.TeacherName, ParseMode.Markdown);
        }
    }
    
    public async Task Left(ITelegramBotClient botClient, long chatId)
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

        await botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
    }
    
    public async Task Exams(ITelegramBotClient botClient, long chatId)
    {
        var exams = await _scheduleService.GetExams(_currentChat.GroupId);

        var ret = "";
        foreach (var exam in exams)
        {
            ret += $"*{exam.Subject}*\n_{exam.LecturerName}_\n{exam.Date:dd.MM.yyyy HH:mm}. ({exam.Room})\n\n";
        }

        await botClient.SendTextMessageAsync(chatId, ret, ParseMode.Markdown);
    }
}