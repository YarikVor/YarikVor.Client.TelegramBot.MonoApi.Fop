using Microsoft.Extensions.Hosting;
using Microsoft.Extensions.Logging;
using Telegram.Bot;
using YarikVor.Api.Monobank.PersonalClient;
using YarikVor.Api.Monobank.PersonalClient.Entities.Dto;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Extensions;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Services;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Processes;

public class NotificationHostedService(
    UserRepositories userRepositories,
    ITelegramBotClient telegramBotClient,
    Storage storage) : IHostedService
{
    private Task? _task = null;

    public Task StartAsync(CancellationToken cancellationToken = default)
    {
        _task ??= Task.Run(async () =>
        {
            await storage.LoadAsync();

            while (true)
            {
                await Task.Delay(60000, cancellationToken);

                foreach (var user in userRepositories.ToArray())
                {
                    if (!user.TryGetValue("monoclient", out var o)) continue;
                    if (!user.TryGetValue("card_id", out var c)) continue;
                    if (o is not MonobankClient monoClient)
                    {
                        continue;
                    }

                    var cardIds = user.TryGetValue("statements", out var s)
                        ? (HashSet<(string, DateTime)>)s
                        : new HashSet<(string, DateTime)>();

                    if (cardIds != s)
                    {
                        user["statements"] = cardIds;
                    }

                    var request = new TransactionRequest()
                    {
                        Account = c.ToString(),
                        From = DateTimeOffset.UtcNow.AddMinutes(-7),
                    };

                    var result = await monoClient.GetTransactionsAsync(request, cancellationToken);

                    if (result.IsSuccess)
                    {
                        var currentTime = DateTime.UtcNow;
                        var offsetTime = currentTime.AddMinutes(-10);
                        cardIds.RemoveWhere(p => p.Item2 <= offsetTime);

                        foreach (var transaction in result.Data.Where(t => cardIds.All(tuple => tuple.Item1 != t.Id)))
                        {
                            cardIds.Add((transaction.Id, currentTime));

                            await telegramBotClient.SendMessage(user["chat_id"].Cast<long>(),
                                $"Прийшло щось: {transaction.Description} {(transaction.Amount / 100M):F} грн");
                        }
                    }
                }

                await storage.SaveAsync();
            }
        }, cancellationToken);
        return Task.CompletedTask;
    }

    public async Task StopAsync(CancellationToken cancellationToken)
    {
        _task?.Dispose();
        _task = null;
        await storage.SaveAsync();
    }
}