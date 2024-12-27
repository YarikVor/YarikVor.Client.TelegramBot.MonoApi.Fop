using System.Collections;
using System.Diagnostics.CodeAnalysis;
using YarikVor.Client.TelegramBot.MonoApi.Fop.Extensions;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Services;

public class UserTable: IEnumerable<KeyValuePair<string, object>>
{
    private readonly Dictionary<string, object> _args = new();

    public void Register(string key, object value)
    {
        _args.Add(key, value);
    }

    public bool TryGetValue(string key, [MaybeNullWhen(false)] out object value)
    {
        return _args.TryGetValue(key, out value);
    }

    public bool Is(string key, object equaler)
    {
        return TryGetValue(key, out object value) && value.Equals(equaler);
    }

    public bool Unregister(string key)
    {
        if (TryGetValue(key, out object value))
        {
            if (value is IDisposable disposable)
            {
                disposable.Dispose();
            }
        }
        return _args.Remove(key);
    }

    public bool Contains(string key)
    {
        return _args.ContainsKey(key);
    }

    public object this[string key]
    {
        get => _args[key];
        set => _args[key] = value;
    }

    public void Clear()
    {
        _args
            .Select(a => a.Value)
            .Where(a => a is IDisposable)
            .Cast<IDisposable>()
            .ForEach(a => a.Dispose());
        _args.Clear();
    }

    public void Set(string key)
    {
        _args[key] = null;
    }

    public IEnumerator<KeyValuePair<string, object>> GetEnumerator()
    {
        return _args.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}