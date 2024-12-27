using Telegram.Bot;
using Telegram.Bot.Types.Enums;
using YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;

public class ParseCommandMiddleware : IMiddleware
{
    public async Task InvokeAsync(ITelegramBotClient client, UserArgs args, Func<Task> next, CancellationToken ct)
    {
        var message = args.Update.Message?.Text;

        if (!message.StartsWith('/'))
            return;

        var me = await client.GetMe(cancellationToken: ct).ConfigureAwait(false);

        var nickname = me.Username;
        if (nickname == null)
            return;

        args.Table["bot_name"] = me.Username;

        var commands = message.Split(' ', StringSplitOptions.RemoveEmptyEntries);

        var splitName = commands[0].Split('@', StringSplitOptions.RemoveEmptyEntries);

        if (args.Update?.Message?.Chat.Type != ChatType.Private
            && splitName.Length > 1
            && splitName[1].ToLower() == nickname.ToLower()
            || args.Update?.Message?.Chat.Type == ChatType.Private)
        {
            var commandName = commands[0][1..];

            args.Table["command_name"] = commandName;

            if (commandName == "start")
            {
                args.Table["state"] = 1;
            }
            else if (commandName == "stop")
            {
                var chatId = args.Table["chat_id"];
                var userId = args.Table["user_id"];
                args.Table.Clear();
                args.Table["chat_id"] = chatId;
                args.Table["user_id"] = userId;
            }

            await next();
        }
    }

    private bool TryParseCommandName(string command, out string commandName)
    {
        commandName = string.Empty;

        var commandPart = command.ToLower();
        var splitCommandName = commandPart.Split('@');

        /*if (splitCommandName.Length == 2 &&
            !splitCommandName[1].Equals(_botName, StringComparison.InvariantCultureIgnoreCase)) return false;*/

        commandName = splitCommandName[0][1..];
        return true;
    }
}