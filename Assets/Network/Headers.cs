using System.Collections;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.Networking;

public static class FetchApi
{
    private static string baseUrl = Constants.EndPointApiBackend;

    public static async Task<T> Get<T>(string endpoint, HeadersAuth headers) where T : class
    {
        for (int attempt = 0; attempt < Constants.MaxRetries; attempt++)
        {
            using (UnityWebRequest request = UnityWebRequest.Get(baseUrl + endpoint))
            {
                AddHeaders(request, headers);
                var operation = request.SendWebRequest();
                while (!operation.isDone)
                {
                    await Task.Yield();
                }

                if (request.result == UnityWebRequest.Result.Success)
                {
                    return JsonUtility.FromJson<T>(request.downloadHandler.text);
                }
                else
                {
                    Debug.LogError($"Attempt {attempt + 1} failed: {request.error}");

                    if (attempt == Constants.MaxRetries - 1)
                    {
                        return default;
                    }
                }
            }
        }
        return null;
    }

    private static void AddHeaders(UnityWebRequest request, HeadersAuth headers)
    {
        request.SetRequestHeader("Authorization", headers.Authorization);
        request.SetRequestHeader("app-pub-key", headers.AppPubKey);

        if (headers is HeadersAuthWithAdmin adminHeaders)
        {
            request.SetRequestHeader("hx_secret", adminHeaders.HxSecret);
        }
    }
}
public class UserDetail
{
    public string AddressWallet { get; set; }
    public string Email { get; set; }
    public object Username { get; set; }
    public UserPoints Points { get; set; }
}
public class UserPoints
{
    public float Energy { get; set; }
    public float Lpx { get; set; }
    public float Roulette { get; set; }
    public float Melon { get; set; }
    public float Experience { get; set; }
    public UserLevel Level { get; set; }
}

public class UserLevel
{
    public int CurrentLevel { get; set; }
    public int RequiredExp { get; set; }
    public int CurrentExp { get; set; }
    public int ExpToNextLevel { get; set; }
}

public static class Constants
{
    public const string EndPointApiBackend = "https://localhost:2567";
    public const int MaxRetries = 3;
    public const int TimeoutMs = 60000;
}

public class HeadersAuth
{
    public string Authorization { get; set; }
    public string AppPubKey { get; set; }

    public HeadersAuth(string authorization, string appPubKey)
    {
        Authorization = authorization;
        AppPubKey = appPubKey;
    }
}

public class HeadersAuthWithAdmin : HeadersAuth
{
    public string HxSecret { get; set; }

    public HeadersAuthWithAdmin(string authorization, string appPubKey, string hxSecret)
        : base(authorization, appPubKey)
    {
        HxSecret = hxSecret;
    }
}

public class CustomAuth
{
    private HeadersAuth _headers;

    public CustomAuth(HeadersAuth headers)
    {
        _headers = headers;
    }

    public async Task<UserDetail> GetUserDetail()
    {
        return await FetchApi.Get<UserDetail>("/v1/user/detail", _headers);
    }

    public async Task<UserLevel> GetUserLevel()
    {
        return await FetchApi.Get<UserLevel>("/v1/user/detail/points/level", _headers);
    }

    public void SetHeaders(HeadersAuth headers)
    {
        _headers = headers;
    }
}

