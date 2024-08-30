using System;
using System.Collections;
using System.Collections.Generic;
using System.Threading.Tasks;
using Colyseus;
using UnityEngine;
using UnityEngine.SceneManagement;
using Watermelon;

[DefaultExecutionOrder(-999)]
public class ClientManager : MonoBehaviour
{
    private static ColyseusClient _client = null;
    private static ColyseusRoom<MatchTileState> _room = null;

    private string authToken = "auth-token";
    private string appKey = "app-key";
    private CustomAuth _customAuth;

    public UserDetail CurrentUserDetail { get; private set; }

    public MatchTileState roomState = new MatchTileState();

    private static ClientManager _instance;

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

        bool isRoomCreated = await CreateGame();

        if (isRoomCreated)
        {
            await InitializeRoomAsync();
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

    private async Task<bool> CreateGame()
    {
        try
        {
            _room = await _client.Create<MatchTileState>("match_tile");
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


    private async Task InitializeRoomAsync()
    {
        _room = GameRoom();

        _client = Client();

        if (_room == null)
        {
            Debug.LogError("Room not initialized yet. Waiting for room initialization...");
            await WaitForRoomInitializationAsync();
        }

        if (_room != null)
        {
            InitializeClientData();
            SetupEventHandlers();
        }
        else
        {
            Debug.LogError("Room initialization failed after waiting.");
        }
    }

    private void InitializeClientData()
    {
        RequestData();
        _room.OnMessage<MatchTileState>("initial_data", OnReceiveInitialData);
    }

    public void OnReceiveInitialData(object initialData)
    {
        var data = JsonUtility.ToJson(initialData);

        Debug.Log(data);

        InitialData initial = new InitialData();

        JsonUtility.FromJsonOverwrite(data, initial);

        roomState.score = initial.score;
        roomState.currentLevel = initial.currentLevel;
        roomState.energy = initial.energy;

        LivesManager.instance.InitLivesServer((int)roomState.energy);
        CurrenciesController.Set(CurrencyType.Coins, (int)roomState.score);
        LevelController.instance.InitLevelServer((int)roomState.currentLevel);

        Debug.Log($"Level: {initial.currentLevel}, Score: {initial.score}, Energy: {initial.energy}");
    }

    private async Task WaitForRoomInitializationAsync()
    {
        while (_room == null)
        {
            await Task.Delay(100);
            _room = GameRoom();
        }
    }

    private void SetupEventHandlers()
    {
        if (_room != null)
        {
            _room.OnStateChange += OnStateChange;
            _room.OnLeave += OnLeaveRoom;
        }
    }

    private void OnStateChange(MatchTileState state, bool isFirstState)
    {
        SetScore(roomState.score);
        SetLevel(roomState.currentLevel);
        CurrenciesController.Set(CurrencyType.Coins, (int)roomState.score);

        Debug.Log("State Updated: Level: " + state.currentLevel + ", Score: " + state.score + ", Energy: " + state.energy);
    }

    public async Task AddEnergyAsync() 
    {
        await _room.Send("add_energy", null);
    }
    public async Task DecreeseEnergyAsync()
    {
        await _room.Send("decreese_energy", null);
    }

    public async Task StartPlaySessionAsync()
    {
        await _room.Send("start_playsession", null);
    }

    public async Task SetLevelAsync(float level)
    {
        await _room.Send("set_lvl", level);
    }

    public async Task SetScoreAsync(float score)
    {
        await _room.Send("set_score", score);
    }

    public async Task WheelsPointsAsync(float score)
    {
        await _room.Send("wheels_points", score);
    }

    public async Task MultiplyPointsAsync(float score)
    {
        await _room.Send("multiply_points", score);
    }

    public async Task EnergyAfterAdsAsync()
    {
        await _room.Send("energy_watch_ads_finished", null);
    }

    public async Task MultipierAdsFinished()
    {
        await _room.Send("multiplier_watch_ads_finished", null);
    }

    public async Task EnergyGet()
    {
        await _room.Send("get_current_energy", null);
    }

    public async Task EndPlaySessionAsync()
    {
        await _room.Send("end_playsession", null);
    }

    public async Task RequestDataAsync()
    {
        await _room.Send("request_initial_data");
    }

    private void OnLeaveRoom(int code)
    {
        Debug.Log("Left the room");
    }

    public void AddEnergy()
    {
        _ = AddEnergyAsync();
    }

    public void DecreeseEnergy()
    {
        _ = DecreeseEnergyAsync();
    }

    public void RequestData()
    {
        _ = RequestDataAsync();
    }

    public void StartPlaySession()
    {
        _ = StartPlaySessionAsync();
    }

    public void SetLevel(float level)
    {
        _ = SetLevelAsync(level);
        roomState.currentLevel = level;
    }

    public void SetScore(float score)
    {
        _ = SetScoreAsync(score);
        roomState.score = score;
    }

    public void GetCurrentEnergy()
    {
        _ = EnergyGet();
    }

    public void MultiplierWatchAdsFinished()
    {
        _ = MultipierAdsFinished();
    }

    public void EnergyWatchAdsFinished()
    {
        _ = EnergyAfterAdsAsync();
    }

    public void WheelsPoints(float point)
    {
        _ = WheelsPointsAsync(point);
        roomState.score += point;
    }

    public void MultiplyPoints(float point)
    {
        _ = MultiplyPointsAsync(point);
        roomState.score *= point;
    }

    public void EndPlaySession()
    {
        _ = EndPlaySessionAsync();
    }

    void OnApplicationQuit()
    {
        if (_room != null)
        {
            EndPlaySession();
            _ = _room.Leave();
        }
    }
}


[Serializable]
public class InitialData
{
    public float currentLevel;
    public float score;
    public float energy;
}