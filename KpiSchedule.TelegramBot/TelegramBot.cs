using KpiSchedule.Api;
using KpiSchedule.Database;
using Refit;
using Serilog;
using Telegram.Bot;
using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;

namespace KpiSchedule.TelegramBot;

public class TelegramBot
{
    private readonly TelegramBotClient _botClient;
    private readonly CancellationTokenSource _cancellationTokenSource;
    private readonly CommandsController _commandsController;
    private readonly ILogger _logger;
    
    public TelegramBot(string token, ILogger logger)
    {
        _botClient = new TelegramBotClient(token);
        _cancellationTokenSource = new CancellationTokenSource();
        _logger = logger;
        
        var dbContext = new KpiScheduleDbContext();
        var scheduleService = new ScheduleService(RestService.For<IScheduleApi>("https://schedule.kpi.ua"));
        _commandsController = new CommandsController(_botClient, scheduleService, dbContext, _logger);
    }

    public void StartReceiving()
    {
        _logger.Information("Telegram bot started");
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
        await _commandsController.HandleCommand(update);
    }

    private Task HandleErrors(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
    {
        var errorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.Error(errorMessage);
        return Task.CompletedTask;
    }
}