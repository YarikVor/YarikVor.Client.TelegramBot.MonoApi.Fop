using System.Collections;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Services;

public class UserRepositories: IEnumerable<UserTable>
{
    private Dictionary<(long, long), UserTable> _users = new Dictionary<(long, long), UserTable>();

    public UserTable Get(long chatId, long userId)
    {
        if (_users.TryGetValue((chatId, userId), out var userContext))
        {
            userContext.Unregister("new_user");
            return userContext;
        }

        userContext = new UserTable();
        _users[(chatId, userId)] = userContext;
        userContext["new_user"] = null;
        userContext["user_id"] = userId;
        userContext["chat_id"] = chatId;
        return userContext;
    }

    public IEnumerator<UserTable> GetEnumerator()
    {
        return _users.Values.GetEnumerator();
    }

    IEnumerator IEnumerable.GetEnumerator()
    {
        return GetEnumerator();
    }
}