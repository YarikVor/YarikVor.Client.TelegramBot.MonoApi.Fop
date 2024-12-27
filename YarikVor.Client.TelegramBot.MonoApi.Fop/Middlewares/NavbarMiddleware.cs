using Telegram.Bot;
using Telegram.Bot.Types;
using YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;

public class NavbarMiddleware : IMiddleware
{
    public async Task InvokeAsync(ITelegramBotClient client, UserArgs args, Func<Task> next, CancellationToken ct)
    {
        if (!args.Table.TryGetValue("state", out var state))
        {
            
            await next();
            return;
        }

        switch (state)
        {
            case 1:
            {
                await client.SendTextMessageAsync(
                    args.Update.Message.Chat.Id,
                    "Введіть token з monobank api:",
                    replyParameters:
                    new ReplyParameters()
                    {
                        MessageId = (int)args.Table["message_id"],
                        ChatId = (long)args.Table["chat_id"],
                    },
                    cancellationToken: ct
                );
                break;
            }
            default:
                await next().ConfigureAwait(false);
                break;
        }
    }
}