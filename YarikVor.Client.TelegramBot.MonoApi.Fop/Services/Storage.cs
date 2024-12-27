using System.Text.Json;
using FireSharp;
using FireSharp.Config;
using Microsoft.Extensions.Options;
using YarikVor.Api.Monobank.PersonalClient;
using YarikVor.Api.Monobank.PersonalClient.Entities.Dto;
using YarikVor.Api.Monobank.PersonalClient.Entities.Options;

namespace YarikVor.Client.TelegramBot.MonoApi.Fop.Services;

public class Storage
{
    private readonly UserRepositories _repositories;
    private readonly FirebaseClient _firebaseClient;
    
    public Storage(UserRepositories repositories, IFirebaseConfigFactory factory)
    {
        _repositories = repositories;
        var fc = factory.Create();
        _firebaseClient = new FirebaseClient(fc);
    }

    public async Task LoadAsync()
    {
        var responce = await _firebaseClient.GetAsync("tg/moo_fo/");
        var users = JsonSerializer.Deserialize<Dictionary<string, Dictionary<string, object>>>(responce.Body);
        foreach (var user in users)
        {
            var userInfos = user.Key.Split('@');
            var chatId = long.Parse(userInfos[0]);
            var userId = long.Parse(userInfos[1]);
            
            var table = _repositories.Get(chatId, userId);

            foreach (var property in user.Value)
            {
                var value = property.Value;
                var state = 0;
                if (value is JsonElement jsonElement)
                {
                    table[property.Key] = jsonElement.ValueKind switch
                    {
                        JsonValueKind.String => jsonElement.GetString(),
                        JsonValueKind.Number => jsonElement.GetInt64(),
                        _ => throw new InvalidCastException()
                    };

                    if (property.Key is "mono_token" && jsonElement.ValueKind == JsonValueKind.String)
                    {
                        var sValue = jsonElement.GetString();
                        var options = new MonobankClientOptions()
                        {
                            Token = sValue,
                        };
                        var monoClient = new MonobankClient(options);
                        var request = TransactionRequest.FromNow(TimeSpan.Zero);
                        var responseType = await monoClient.GetTransactionsAsync(request);
                        if (responseType.IsSuccess)
                        {
                            table["monoclient"] = monoClient;
                        }
                    }
                }
            }
        }
    }

    public async Task SaveAsync()
    {
        foreach (var table in _repositories)
        {
            var data = table.Where(p => p.Value is string or int or long)
                .ToDictionary();
            
            await _firebaseClient.SetAsync($"tg/moo_fo/{table["chat_id"]}@{table["user_id"]}", data);
        }
    }
}