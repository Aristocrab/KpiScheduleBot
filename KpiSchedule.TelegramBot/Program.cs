using KpiSchedule.TelegramBot;
using Microsoft.Extensions.Configuration;
using Serilog;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();
var logger = new LoggerConfiguration()
    .WriteTo.Console()
    .CreateLogger();

if (config["TelegramBotApiKey"] is null)
{
    Console.WriteLine("User secret 'TelegramBotApiKey' was not found.");
    return;
}

var bot = new TelegramBot(config["TelegramBotApiKey"]!, logger);

bot.StartReceiving();
Console.ReadLine();

bot.Stop();