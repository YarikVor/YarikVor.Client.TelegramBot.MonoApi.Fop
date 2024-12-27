using Telegram.Bot.Types;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Services;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

public class UserArgs(Update update, TelegramHelper helper)
{
    public readonly Update Update = update;

    public UserTable Table;

    public readonly TelegramHelper Helper = helper;
}