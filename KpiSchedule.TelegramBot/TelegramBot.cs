using KpiSchedule.Api;
using KpiSchedule.Database;
using Serilog;
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
    private ILogger _logger { get; set; }
    
    public TelegramBot(string token, ILogger logger)
    {
        _botClient = new TelegramBotClient(token);
        _cancellationTokenSource = new CancellationTokenSource();
        _dbContext = new KpiScheduleDbContext();

        _logger = logger;
        var scheduleService = new ScheduleService();
        _commandsController = new CommandsController(_botClient, scheduleService, _dbContext, _logger);
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
        var ErrorMessage = exception switch
        {
            ApiRequestException apiRequestException
                => $"Telegram API Error:\n[{apiRequestException.ErrorCode}]\n{apiRequestException.Message}",
            _ => exception.ToString()
        };

        _logger.Error(ErrorMessage);
        return Task.CompletedTask;
    }
}