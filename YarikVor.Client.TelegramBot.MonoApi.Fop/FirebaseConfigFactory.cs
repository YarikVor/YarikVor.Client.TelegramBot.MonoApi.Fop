using FireSharp.Config;
using FireSharp.Interfaces;
using Microsoft.Extensions.Options;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop;

public interface IFirebaseConfigFactory
{
    IFirebaseConfig Create();
}

public class FirebaseConfigFactory(IOptions<FirebaseSetting> options) : IFirebaseConfigFactory
{
    private readonly FirebaseSetting _setting = options.Value;

    public IFirebaseConfig Create()
    {
        return new FirebaseConfig()
        {
            AuthSecret = _setting.AuthSecret,
            BasePath = _setting.BasePath
        };
    }
}