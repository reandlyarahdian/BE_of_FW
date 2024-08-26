using System.Collections;
using System.Threading.Tasks;
using Colyseus;
using UnityEngine;
using Watermelon;

public class ClientGameManager : MonoBehaviour
{
    private ColyseusRoom<MatchTileState> _room;

    private ColyseusClient _client;

    private static ClientGameManager instance;

    private static LevelData level;

    private static LevelDatabase database;

    public MatchTileState roomState = new MatchTileState();

    public static ClientGameManager Instance
    {
        get
        {
            if (instance == null)
                instance = GameObject.Find("ScriptHolder").GetComponent<ClientGameManager>();

            return instance;
        }
    }


    private async void Awake()
    {
        await InitializeRoomAsync();
    }

    private async Task InitializeRoomAsync()
    {
        _room = ClientManager.Instance.GameRoom();

        _client = ClientManager.Instance.Client();

        if (_room == null)
        {
            Debug.LogError("Room not initialized yet. Waiting for room initialization...");
            await WaitForRoomInitializationAsync();
        }

        if (_room != null)
        {
            InitializeClientData();
            StartPlaySession();
            SetupEventHandlers();
        }
        else
        {
            Debug.LogError("Room initialization failed after waiting.");
        }
    }

    private void InitializeClientData()
    {
        UserDetail userDetail = ClientManager.Instance.CurrentUserDetail;

        if (userDetail != null)
        {
            roomState.score = ClientManager.Instance.CurrentUserDetail.Points.Melon;
            roomState.currentLevel = ClientManager.Instance.CurrentUserDetail.Points.Level.CurrentLevel;
            roomState.energy = ClientManager.Instance.CurrentUserDetail.Points.Energy;
        }
        else
        {
            Debug.LogError("Failed to retrieve user data from ClientManager.");
        }
    }

    private async Task WaitForRoomInitializationAsync()
    {
        while (_room == null)
        {
            await Task.Delay(100);
            _room = ClientManager.Instance.GameRoom();
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
        if (isFirstState) 
        {
            SetScore(roomState.score);
            SetLevel(roomState.currentLevel);
            CurrenciesController.Set(CurrencyType.Coins, (int)roomState.score);
        }


        Debug.Log("State Updated: Level: " + state.currentLevel + ", Score: " + state.score);
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

    private void OnCurrentLevelReceived(float level)
    {
        Debug.Log("Current Level: " + level);
    }

    private void OnCurrentEnergyReceived(float energy)
    {
        Debug.Log("Current Energy: " + energy);
    }

    private void OnWheelsPointsReceived(float points)
    {
        Debug.Log("Wheels Points: " + points);
    }

    private void OnMultiplierReceived(int multiplier)
    {
        Debug.Log("Multiplier: " + multiplier);
    }

    private void OnLeaveRoom(int code)
    {
        Debug.Log("Left the room");
    }

    public void StartPlaySession()
    {
        _ = StartPlaySessionAsync();
    }

    public void SetLevel(float level)
    {
        _ = SetLevelAsync(level);
    }

    public void SetScore(float score)
    {
        _ = SetScoreAsync(score);
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
    }

    public void MultiplyPoints(float point)
    {
        _ = MultiplyPointsAsync(point);
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
