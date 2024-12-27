using Telegram.Bot;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Services;
using YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;

public class CheckMiddleware : IMiddleware
{
    private readonly UserRepositories _repositories;

    public CheckMiddleware(UserRepositories repositories)
    {
        _repositories = repositories;
    }

    public async Task InvokeAsync(ITelegramBotClient client, UserArgs args, Func<Task> next, CancellationToken ct)
    {
        args.Table = _repositories.Get(args.Update.Message!.Chat.Id, args.Update.Message!.From.Id);

        args.Table["user_name"] = args.Update.Message.From.FirstName;
        args.Table["message_id"] = args.Update.Message.MessageId;
        args.Table["message"] = args.Update.Message!.Text;

        if (args.Table.Contains("new_user"))
        {
            args.Table.Set("state_wait");
        }

        await next();
    }
}