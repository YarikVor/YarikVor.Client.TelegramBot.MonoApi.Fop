namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Extensions;

public static class AsIsExtensions
{
    public static T Cast<T>(this object o)
    {
        return (T)o;
    }

    public static T? As<T>(this object o) where T : class
    {
        return o as T;
    }

    public static T? AsNull<T>(this object? o) where T : struct
    {
        return o is T value ? (T?)value : null;
    }

}