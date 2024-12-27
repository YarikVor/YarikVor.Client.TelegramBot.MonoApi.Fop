using Telegram.Bot;
using Telegram.Bot.Types;
using Telegram.Bot.Types.Enums;
using Telegram.Bot.Types.ReplyMarkups;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.ServicesLocal;

public class TelegramHelper(ITelegramBotClient bot, CancellationToken ct, ChatId chatId, int messageId)
{
    public async Task DeleteMessageAsync()
    {
        await bot.DeleteMessage(chatId, messageId, ct);
    }

    public async Task<Message> WriteAsync(
        string text,
        int? messageThreadId = default,
        ParseMode parseMode = default,
        IEnumerable<MessageEntity>? entities = default,
        LinkPreviewOptions? linkPreviewOptions = default,
        bool disableNotification = default,
        bool protectContent = default,
        bool allowPaidBroadcast = default,
        string? messageEffectId = default,
        ReplyParameters? replyParameters = default,
        IReplyMarkup? replyMarkup = default,
        string? businessConnectionId = default,
        CancellationToken cancellationToken = default)
    {
        return await bot.SendMessage(
            chatId,
            text,
            parseMode, replyParameters, replyMarkup, linkPreviewOptions, messageThreadId, entities,
            disableNotification, protectContent, messageEffectId, businessConnectionId, allowPaidBroadcast,
            cancellationToken);
    }
}