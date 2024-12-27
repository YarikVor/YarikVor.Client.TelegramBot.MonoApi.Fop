using Microsoft.Extensions.Options;
using Telegram.Bot;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop;

public interface ITelegramBotClientOptionsBuilder
{
    TelegramBotClientOptions Create();
}

public class TelegramBotClientOptionsBuilder(IOptions<TelegramSetting> options) : ITelegramBotClientOptionsBuilder
{
    private readonly TelegramSetting _setting = options.Value;

    public TelegramBotClientOptions Create()
    {
        return new TelegramBotClientOptions(_setting.Token);
    }
}