namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Extensions;

public static class KotlinMethodsExtensions
{
    public static T Let<T>(this T value, Action<T> action)
    {
        action(value);
        return value;
    }

    public static TResult Let<T, TResult>(this T value, Func<T, TResult> action)
    {
        return action(value);
    }

    public static void Apply<T>(this T value, Action<T> action)
    {
        action(value);
    }
}