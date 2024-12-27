using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using Telegram.Bot.Polling;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;
using YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Processes;

public class BotHostedService(
    ITelegramBotClient bot,
    MiddlewareManager middleware,
    ILogger<NotificationHostedService> logger) : IHostedService
{
    private async Task SetCommandAsync(CancellationToken cancellationToken = default)
    {
        BotCommand[] commands =
        [
            new BotCommand()
            {
                Command = "start",
                Description = "Розпочати роботу"
            },
            new BotCommand()
            {
                Command = "stop",
                Description = "Видалити всі пов'язані рахунки до цього користувача"
            }
        ];

        await bot.SetMyCommands(commands, cancellationToken: cancellationToken)
            .ConfigureAwait(false);
    }

    private async Task HandlePollingErrorAsync(ITelegramBotClient arg1, Exception arg2, HandleErrorSource arg3,
        CancellationToken arg4)
    {
    }

    private async Task HandleUpdateAsync(ITelegramBotClient arg1, Update update, CancellationToken arg3)
    {
        if (update.Type != UpdateType.Message)
        {
            return;
        }

        var message = update.Message?.Text;

        if (string.IsNullOrWhiteSpace(message))
            return;

        await middleware.InvokeAsync(arg1,
            new UserArgs(update,
                new TelegramHelper(arg1, arg3, update.Message.Chat, update.Message.MessageId)), arg3);
    }

    public async Task StartAsync(CancellationToken cancellationToken)
    {
        ReceiverOptions receiverOptions = new()
        {
            AllowedUpdates = new[] { UpdateType.Message }
        };

        await SetCommandAsync(cancellationToken);

        bot.StartReceiving(
            HandleUpdateAsync,
            HandlePollingErrorAsync,
            receiverOptions,
            cancellationToken
        );

        var me = await bot.GetMeAsync(cancellationToken);
        logger.LogInformation("Hello, World! I am user {} and my name is {}.", me.Username, me.FirstName);
    }

    public Task StopAsync(CancellationToken cancellationToken)
    {
        return Task.CompletedTask;
    }
}