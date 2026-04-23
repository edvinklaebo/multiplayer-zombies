using System;
using System.Linq;
using Fusion;
using Fusion.Sockets;
using System.Threading.Tasks;
using UnityEngine;
using UnityEngine.SceneManagement;

public class NetworkBootstrap : MonoBehaviour, INetworkRunnerCallbacks
{
    private NetworkRunner _runner;
    
    [SerializeField] private NetworkPrefabRef playerPrefab;
    [SerializeField] private TMPro.TextMeshProUGUI lobbyCodeText;
    [SerializeField] private TMPro.TMP_InputField joinInput;
    [SerializeField] private GameObject lobbyCanvas;
    [SerializeField] private GameObject loadingOverlay;
    
    private bool _isConnecting;

    public void Awake()
    {
        lobbyCanvas.gameObject.SetActive(true);
    }
    
    public async void OnHostClicked()
    {
        string code = LobbyCodeGenerator.Generate();

        bool success = await RunWithLoading(StartHost(code));

        if (!success)
        {
            ReloadScene();
        }
    }
    
    public async void OnJoinClicked()
    {
        string code = joinInput.text.ToUpper();

        if (!IsValidCode(code))
        {
            Debug.Log("Invalid code");
            return;
        }

        var success = await RunWithLoading(JoinGame(code));

        if (!success)
        {
            ReloadScene();
        }
    }
    
    private static bool IsValidCode(string code)
    {
        return code.Length == 5 && code.All(c => c is >= 'A' and <= 'Z');
    }
    
    public async Task StartHost(string sessionName)
    {
        _runner = gameObject.AddComponent<NetworkRunner>();
        _runner.ProvideInput = true;

        var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

        var result = await _runner.StartGame(new StartGameArgs
        {
            GameMode = GameMode.Host,
            SessionName = sessionName,
            Scene = scene,
            SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
        });

        if (!result.Ok)
        {
            Debug.LogError($"Failed to start: {result.ShutdownReason}");
            return;
        }
        lobbyCanvas.SetActive(false);
    }
    
    public async Task JoinGame(string sessionName)
    {
        if (_isConnecting)
        {
            Debug.Log("Already connecting...");
            return;
        }

        _isConnecting = true;

        try
        {
            if (_runner != null)
            {
                await _runner.Shutdown();
                Destroy(_runner);
                _runner = null;
            }

            _runner = gameObject.AddComponent<NetworkRunner>();
            _runner.ProvideInput = true;

            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);

            var result = await _runner.StartGame(new StartGameArgs
            {
                GameMode = GameMode.Client,
                SessionName = sessionName,
                Scene = scene,
                SceneManager = gameObject.AddComponent<NetworkSceneManagerDefault>()
            });
            
            if(result.Ok)
                lobbyCanvas.SetActive(false);
        }
        finally
        {
            _isConnecting = false;
        }
    }

    private void ReloadScene()
    {
        Debug.Log("Reloading scene...");

        SceneManager.LoadScene(SceneManager.GetActiveScene().buildIndex);
    }
    
    private async Task<bool> RunWithLoading(Task task)
    {
        loadingOverlay.SetActive(true);

        try
        {
            await task;
            return true;
        }
        catch (Exception e)
        {
            Debug.LogError(e);
            return false;
        }
        finally
        {
            loadingOverlay.SetActive(false);
        }
    }
    
    public void OnPlayerJoined(NetworkRunner runner, PlayerRef player)
    {
        if (!runner.IsServer) 
            return;
        
        Debug.Log("Spawning player...");
        
        var spawnPos = new Vector3(0, 2, 0);
        
        runner.Spawn(
            playerPrefab,
            spawnPos,
            Quaternion.identity,
            player
        );
    }

    public void OnInput(NetworkRunner runner, NetworkInput input) 
    {
        var data = new PlayerInputData
        {
            Move = new Vector2(Input.GetAxis("Horizontal"), Input.GetAxis("Vertical")),
            Look = new Vector2(Input.GetAxis("Mouse X"), Input.GetAxis("Mouse Y")),
            Fire = Input.GetMouseButton(0)
        };

        input.Set(data);
    }
    
    // --- REQUIRED CALLBACKS (empty for now) ---
    public void OnConnectedToServer(NetworkRunner runner) { }
    public void OnObjectExitAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnObjectEnterAOI(NetworkRunner runner, NetworkObject obj, PlayerRef player) { }
    public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
    public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
    public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
    public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
    public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
    public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
    public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
    public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, ArraySegment<byte> data) { }
    public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }
    public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
    public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
    public void OnSceneLoadDone(NetworkRunner runner) { }
    public void OnSceneLoadStart(NetworkRunner runner) { }
}