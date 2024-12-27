using Microsoft.Extensions.DependencyInjection;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;

public class MiddlewareManagerBuilder
{
    private static readonly Type MiddlewareType = typeof(IMiddleware);
    private readonly List<Type> _types = new();

    public MiddlewareManagerBuilder Add<T>()
        where T : IMiddleware
    {
        _types.Add(typeof(T));
        return this;
    }

    public MiddlewareManagerBuilder Add(Type type)
    {
        if (type.IsAssignableTo(MiddlewareType))
            throw new TypeAccessException($"{type} must be {MiddlewareType}");

        _types.Add(type);
        return this;
    }

    public MiddlewareManager Create(IServiceProvider service)
    {
        var middlewares = _types
            .Select(service.GetRequiredService)
            .Where(t => t is not null)
            .Cast<IMiddleware>()
            .ToArray();

        var middlewareManager = new MiddlewareManager(middlewares);
        return middlewareManager;
    }
}