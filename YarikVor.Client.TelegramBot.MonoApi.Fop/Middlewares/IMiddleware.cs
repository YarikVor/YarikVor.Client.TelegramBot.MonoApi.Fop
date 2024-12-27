using Telegram.Bot;
using YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;

public interface IMiddleware
{
    Task InvokeAsync(ITelegramBotClient client, UserArgs args, Func<Task> next, CancellationToken ct);
}