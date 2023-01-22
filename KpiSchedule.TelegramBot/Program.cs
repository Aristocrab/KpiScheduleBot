using KpiSchedule.TelegramBot;
using Microsoft.Extensions.Configuration;

var config = new ConfigurationBuilder()
    .AddUserSecrets<Program>()
    .Build();

if (config["TelegramBotApiKey"] is null)
{
    Console.WriteLine("User secret 'TelegramBotApiKey' was not found.");
    return;
}

var bot = new TelegramBot(config["TelegramBotApiKey"]!);

bot.StartReceiving();
Console.WriteLine("Bot started");
Console.ReadLine();

bot.Stop();