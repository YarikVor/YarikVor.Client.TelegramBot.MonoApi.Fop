using Telegram.Bot;
using YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;

public class MiddlewareManager
{
    private readonly IMiddleware[] _middlewares;

    public MiddlewareManager(IMiddleware[] middlewares)
    {
        _middlewares = middlewares;
    }

    public async ValueTask InvokeAsync(ITelegramBotClient client, UserArgs args, CancellationToken ct)
    {
        using var enumerator = ((IEnumerable<IMiddleware>)_middlewares).GetEnumerator();

        await Next();
        return;

        async Task Next()
        {
            if (enumerator.MoveNext())
                await enumerator.Current.InvokeAsync(client, args, Next, ct);
        }
    }
}