using System.Collections;
using System.Threading.Tasks;
using Colyseus;
using UnityEngine;
using Watermelon;

public class ClientGameManager : MonoBehaviour
{
    private ColyseusRoom<MatchTileState> _room;

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


    private async void Start()
    {
        await InitializeRoomAsync();
    }

    private async Task InitializeRoomAsync()
    {
        _room = ClientManager.Instance.GameRoom();

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
        UserDetail userDetail = ClientManager.Instance.CurrentUserDetail;

        if (userDetail != null)
        {
            roomState.score = ClientManager.Instance.CurrentUserDetail.Points.Melon;
            roomState.currentLevel = ClientManager.Instance.CurrentUserDetail.Points.Level.CurrentLevel;
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
            await Task.Delay(100);  // Check every 100ms to see if the room is initialized
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
        if (roomState.currentLevel > 0 && roomState.score > 0 && isFirstState)
        {
            state.score = roomState.score;
            state.currentLevel = roomState.currentLevel;

            CurrenciesController.Set(CurrencyType.Coins, (int)state.score);
        }

        Debug.Log("State Updated: Level: " + state.currentLevel + ", Score: " + state.score);
    }

    public async Task StartPlaySessionAsync()
    {
        if (_room != null)
        {
            await _room.Send("start_playsession", null);
        }
        else
        {
            Debug.LogError("Cannot start play session: Room is not initialized.");
        }
    }

    public async Task SetLevelAsync(int level)
    {
        if (_room != null)
        {
            await _room.Send("set_lvl", level);
        }
        else
        {
            Debug.LogError("Cannot start play session: Room is not initialized.");
        }
    }

    public async Task SetScoreAsync(int score)
    {
        if (_room != null)
        {
            await _room.Send("set_score", score);
        }
        else
        {
            Debug.LogError("Cannot start play session: Room is not initialized.");
        }
    }

    public async Task WheelsPointsAsync(int score)
    {
        if (_room != null)
        {
            await _room.Send("wheels_points", score);
        }
        else
        {
            Debug.LogError("Cannot start play session: Room is not initialized.");
        }
    }

    public async Task MultiplyPointsAsync(int score)
    {
        if (_room != null)
        {
            await _room.Send("wheels_points", score);
        }
        else
        {
            Debug.LogError("Cannot start play session: Room is not initialized.");
        }
    }

    private void OnCurrentLevelReceived(int level)
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

    public void SetLevel(int level)
    {
        _ = SetLevelAsync(level);
    }

    public void SetScore(int score)
    {
        _ = SetScoreAsync(score);
    }

    public void GetCurrentEnergy()
    {
        _ = _room.Send("get_current_energy", null);
    }

    public void MultiplierWatchAdsFinished()
    {
        _ = _room.Send("multiplier_watch_ads_finished", null);
    }

    public void EnergyWatchAdsFinished()
    {
        _ = _room.Send("energy_watch_ads_finished", null);
    }

    public void WheelsPoints(int point)
    {
        _ = WheelsPointsAsync(point);
    }

    public void MultiplyPoints(int point)
    {
        _ = MultiplyPointsAsync(point);
    }

    public void EndPlaySession()
    {
        _ = _room.Send("end_playsession", null);
    }

    void OnApplicationQuit()
    {
        if (_room != null)
        {
            _ = _room.Leave();
        }
    }
}
