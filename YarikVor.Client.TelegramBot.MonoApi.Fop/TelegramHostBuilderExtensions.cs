using Microsoft.Extensions.DependencyInjection;
using Telegram.Bot;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Middlewares;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Processes;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Services;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop;

public static class TelegramHostBuilderExtensions
{
    public static IServiceCollection AddTelegramHost(this IServiceCollection services)
    {
        
        
        services
            .AddSingleton<ITelegramBotClientOptionsBuilder, TelegramBotClientOptionsBuilder>()
            .AddSingleton<IFirebaseConfigFactory, FirebaseConfigFactory>()
            .AddSingleton<ITelegramBotClient, TelegramBotClient>(provider =>
            {
                var factory = provider.GetRequiredService<ITelegramBotClientOptionsBuilder>();
                var options = factory.Create();
                return new TelegramBotClient(options, new HttpClient());
            })
            .AddHostedService<BotHostedService>()
            .AddSingleton<CheckMiddleware>()
            .AddSingleton<ParseCommandMiddleware>()
            .AddSingleton<NavbarMiddleware>()
            .AddSingleton<UserRepositories>()
            .AddSingleton<AnswerMiddleware>()
            .AddSingleton<MonobankCheckMiddleware>()
            .AddHostedService<NotificationHostedService>()
            .AddSingleton<Storage>();

        var middleware = new MiddlewareManagerBuilder()
            .Add<CheckMiddleware>()
            .Add<MonobankCheckMiddleware>()
            .Add<AnswerMiddleware>()
            .Add<ParseCommandMiddleware>()
            .Add<NavbarMiddleware>();
        
        services.AddSingleton(middleware.Create);

        return services;
    }
}