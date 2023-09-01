using Telegram.Bot.Exceptions;
using Telegram.Bot.Polling;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types;
using Telegram.Bot;
using AnalisisPrice;
using Telegram.Bot.Types.ReplyMarkups;
using System.Security.Cryptography.X509Certificates;
using System.Linq;
using System.Security.Cryptography;
using Newtonsoft.Json.Linq;
using System.Text.RegularExpressions;

var botClient = new TelegramBotClient("Bot token");
AnalysisPriceBotDbContext db = new();
var database = db.Analises.ToList();using CancellationTokenSource cts = new();
List<string> memory = new List<string>(2);

// StartReceiving does not block the caller thread. Receiving is done on the ThreadPool.
ReceiverOptions receiverOptions = new()
{
    AllowedUpdates = Array.Empty<UpdateType>() // receive all update types except ChatMember related updates
};

botClient.StartReceiving(
    updateHandler: HandleUpdateAsync,
    pollingErrorHandler: HandlePollingErrorAsync,
    receiverOptions: receiverOptions,
    cancellationToken: cts.Token
);

var me = await botClient.GetMeAsync();

Console.WriteLine($"Start listening for @{me.Username}");
Console.ReadLine();

// Send cancellation request to stop bot
cts.Cancel();

async Task HandleUpdateAsync(ITelegramBotClient botClient, Update update, CancellationToken cancellationToken)
{
    // Only process Message updates: https://core.telegram.org/bots/api#message
    if (update.Message is not { } message)
        return;
    // Only process text messages
    if (message.Text is not { } messageText)
        return;

    var chatId = message.Chat.Id;

    //char[] charsToTrim = { ' ', '\n', 't', '"', '\"' };
    //string? searching = message.Text.Trim();

    //string? trimmed = searching.Trim(charsToTrim);
  
    Console.WriteLine($"Received a '{messageText}' message in chat {chatId}.");
    if (message.Text == "/start")
    {
        Message sent = await botClient.SendTextMessageAsync
        (
        chatId: chatId,
        messageText = "Бот начал работу, введите название анализа и его цену",
        cancellationToken: cancellationToken


        );
     
    }
    NameAndPrice np = new();
    bool flag = false;
    
    if(memory.Count != 2)
    {
        memory.Add(message.Text);
    }
    if(memory.Count == 2)
    {
        flag= true;
    }
    if (flag == true)
    {
        int number;
        bool sucsess = int.TryParse(memory[1], out number);
        if(sucsess)
        {
            bool match = false;
            foreach (var item in database)
            {
                

                if (item.AnalisisName.ToLower() == memory[0].ToLower() & item.AnalisisPrice == Convert.ToInt16(memory[1]))
                {
                   match = true;
                }
            }
            if (match == true)
            {
                Message send = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Верно",
                    cancellationToken: cancellationToken
                    );
            }
            else if (match == false)
            {
                Message send = await botClient.SendTextMessageAsync(
                    chatId: chatId,
                    text: "Не верно",
                    cancellationToken: cancellationToken
                    );
            }
            memory.RemoveRange(0, 2);

        }
        else
        {
            Message sent = await botClient.SendTextMessageAsync(
                chatId:chatId,
                text:"Второе значение не может быть ценой, введите значения заново"
                );
            memory.RemoveRange(0, 2);
        }

    }








    //foreach(var item in database)
    // {
    //     Message sent1 = await botClient.SendTextMessageAsync
    //               (
    //               chatId: chatId,
    //               messageText = item.AnalisisName,
    //               cancellationToken: cancellationToken

    //               );
    // }




}
    



    






Task HandlePollingErrorAsync(ITelegramBotClient botClient, Exception exception, CancellationToken cancellationToken)
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
