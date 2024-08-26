using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using UnityEngine;
using UnityEngine.SceneManagement;

[DefaultExecutionOrder(-999)]
public class ClientManager : MonoBehaviour
{
    private static ColyseusClient _client = null;
    private static ColyseusRoom<MatchTileState> _room = null;

    private string authToken = "auth-token";
    private string appKey = "app-key";
    private CustomAuth _customAuth;

    private static ClientManager _instance;

    public UserDetail CurrentUserDetail { get; private set; }

    public static ClientManager Instance
    {
        get
        {
            if (_instance == null)
            {
                _instance = FindObjectOfType<ClientManager>();

                if (_instance == null)
                {
                    GameObject singletonObject = new GameObject(typeof(ClientManager).Name);
                    _instance = singletonObject.AddComponent<ClientManager>();
                    DontDestroyOnLoad(singletonObject);
                }
            }
            return _instance;
        }
    }

    public async void Awake()
    {
        InitializeColyseusClient();

        HeadersAuth headersAuth = new HeadersAuth(authToken, appKey);
        _customAuth = new CustomAuth(headersAuth);

        await FetchUserDetails();

        bool isRoomCreated = await JoinOrCreateGame();
        if (isRoomCreated)
        {
            Debug.Log(SceneManager.GetActiveScene().name);
        }
        else
        {
            Debug.LogError("Failed to create or join the room. Initialization aborted.");
        }
    }

    private void InitializeColyseusClient()
    {
        _client = new ColyseusClient("ws://localhost:2567");
    }

    private async Task FetchUserDetails()
    {
        CurrentUserDetail = await _customAuth.GetUserDetail();
        if (CurrentUserDetail != null)
        {
            Debug.Log($"User Email: {CurrentUserDetail.Email}");
        }
        else
        {
            Debug.LogWarning("Failed to fetch user detail. Using dummy data.");

            CurrentUserDetail = new UserDetail
            {
                AddressWallet = "dummy-wallet-address",
                Email = "dummy@example.com",
                Username = "DummyUser",
                Points = new UserPoints
                {
                    Energy = 100,
                    Lpx = 50,
                    Roulette = 10,
                    Melon = 30,
                    Experience = 0,
                    Level = new UserLevel
                    {
                        CurrentLevel = 1,
                        RequiredExp = 100,
                        CurrentExp = 0,
                        ExpToNextLevel = 100
                    }
                }
            };
        }
    }

    private async Task<bool> JoinOrCreateGame()
    {
        try
        {
            _room = await _client.JoinOrCreate<MatchTileState>("match_tile");
            DontDestroyOnLoad(this.gameObject);
            return true;
        }
        catch (System.Exception ex)
        {
            Debug.LogError("Error while joining or creating room: " + ex.Message);
            return false;
        }
    }

    public ColyseusClient Client()
    {
        return _client;
    }

    public ColyseusRoom<MatchTileState> GameRoom()
    {
        return _room;
    }
}
