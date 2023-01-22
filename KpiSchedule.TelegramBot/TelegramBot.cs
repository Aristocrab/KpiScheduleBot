using KpiSchedule.Api;
using KpiSchedule.Database;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KpiSchedule.TelegramBot;

public class TelegramBot
{
    private TelegramBotClient _botClient { get; set; }
    private CancellationTokenSource _cancellationTokenSource { get; set; }
    private CommandsController _commandsController { get; set; }
    private KpiScheduleDbContext _dbContext { get; set; }
    
    public TelegramBot(string token)
    {
        _botClient = new TelegramBotClient(token);
        _cancellationTokenSource = new CancellationTokenSource();
        _dbContext = new KpiScheduleDbContext();
        _commandsController = new CommandsController(new ScheduleService(), _dbContext);
    }

    public void StartReceiving()
    {
        _botClient.StartReceiving(
            updateHandler: HandleUpdates,
            pollingErrorHandler: HandleErrors,
            receiverOptions: new ReceiverOptions
            {
                AllowedUpdates = Array.Empty<UpdateType>()
            },
            cancellationToken: _cancellationTokenSource.Token
        );
    }

    public void Stop()
    {
        _cancellationTokenSource.Cancel();
    }

    private async Task HandleUpdates(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
    {
        await _commandsController.HandleCommand(botClient, update);
    }

    private Task HandleErrors(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        Console.WriteLine(ErrorMessage);
        return Task.CompletedTask;
    }
}