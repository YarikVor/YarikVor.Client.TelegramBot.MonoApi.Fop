using FireSharp.Interfaces;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.DependencyInjection.Extensions;
using Microsoft.Extensions.Hosting;
using Telegram.Bot.Types;
using YarikVor.Client.TelegramBot.MonoApi.Fop;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Extensions;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Processes;

AppDomain.CurrentDomain.UnhandledException += (sender, args) =>
{
    var exception = args.ExceptionObject as Exception;
    Console.WriteLine($"[Unhandled Exception] {exception?.Message}\n{exception?.StackTrace}");
};

var builder = Host.CreateApplicationBuilder(args);

builder.Configuration.Apply(it =>
{
    it.AddJsonFile("appsettings.json");
});

builder.Services.TryAddEnumerable();

builder.Services
    .Configure<FirebaseSetting>(builder.Configuration.GetRequiredSection("Firebase"))
    .Configure<TelegramSetting>(builder.Configuration.GetRequiredSection("Telegram"));

builder.Services
    .AddTelegramHost();

var engine = builder.Build();
await engine.RunAsync();
