using Telegram.Bot;
using Telegram.Bot.Types.ReplyMarkups;
using YarikVor.Api.Monobank.PersonalClient;
using YarikVor.Api.Monobank.PersonalClient.Entities.Options;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Extensions;
using YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;

public class AnswerMiddleware : IMiddleware
{
    public async Task InvokeAsync(ITelegramBotClient client, UserArgs args, Func<Task> next, CancellationToken ct)
    {
        if (!args.Table.Contains("state"))
        {
            await next();
            return;
        }

        if (args.Table.Is("state", 1))
        {
            var message = args.Table["message"].Cast<string>().Trim();
            await args.Helper.DeleteMessageAsync();
            var options = new MonobankClientOptions()
            {
                Token = message,
            };
            var monoclient = new MonobankClient(options);
            var result = await monoclient.GetPersonalInfoAsync(ct);

            if (result.IsSuccess)
            {
                await client.SendMessage(args.Update.Message!.Chat, "Все ок, я слив ваші дані)");
                args.Table["monoclient"] = monoclient;
                args.Table["mono_token"] = message;

                var p = result.Data!;

                var array = p.Accounts
                    .Select(a => (a.Id, a.MaskedPan.FirstOrDefault() ?? a.Id))
                    .Concat(
                        p.Jars.Select(j => (j.Id, j.Title))
                    )
                    .ToArray();

                args.Table["list_of_cards"] = array;

                var keys = array
                    .Select(a => new KeyboardButton(a.Item2))
                    .Chunk(1)
                    .ToArray();

                await client.SendMessage(args.Update.Message.Chat,
                    "Оберіть карту чи банку",
                    replyMarkup: new ReplyKeyboardMarkup(keys), cancellationToken: ct);

                args.Table["state"] = 2;
            }
            else
            {
                monoclient.Dispose();
                await client.SendMessage(args.Update.Message!.Chat, "Не валідно!");
            }
        }
        else if (args.Table.Is("state", 2))
        {
            var list = ((string, string)[])args.Table["list_of_cards"];
            var message = args.Table["message"].ToString();
            var infos = list.FirstOrDefault(t => t.Item2 == message);
            if (infos.Item1 == null)
                return;
            args.Table.Unregister("list_of_cards");
            args.Table["card_id"] = infos.Item1;
            args.Table.Unregister("state");
            await client.SendMessage(args.Update.Message!.Chat, "Дані успішно підтягнуто!", cancellationToken: ct);
        }
    }
}