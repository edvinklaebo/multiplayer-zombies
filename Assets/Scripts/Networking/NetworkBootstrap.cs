using System.Threading.Tasks;
using Fusion;
using Fusion.Sockets;
using MultiplayerZombies.Player;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace MultiplayerZombies.Networking
{
    public class NetworkBootstrap : MonoBehaviour, INetworkRunnerCallbacks
    {
        [SerializeField] private NetworkRunner runnerPrefab;

        private NetworkRunner _runner;

        public async void StartHost()
        {
            await StartGame(GameMode.Host, "Room");
        }

        public async void JoinGame(string sessionName)
        {
            await StartGame(GameMode.Client, sessionName);
        }

        private async Task StartGame(GameMode mode, string sessionName)
        {
            if (_runner != null)
            {
                return;
            }

            _runner = Instantiate(runnerPrefab);
            _runner.name = "NetworkRunner";
            _runner.ProvideInput = true;
            _runner.AddCallbacks(this);

            var scene = SceneRef.FromIndex(SceneManager.GetActiveScene().buildIndex);
            var result = await _runner.StartGame(new StartGameArgs
            {
                GameMode = mode,
                PlayerCount = 4,
                SessionName = string.IsNullOrWhiteSpace(sessionName) ? "Room" : sessionName,
                Scene = scene,
                SceneManager = _runner.GetComponent<NetworkSceneManagerDefault>()
            });

            if (!result.Ok)
            {
                Debug.LogError($"[Network] Failed to start: {result.ShutdownReason}");
                Destroy(_runner.gameObject);
                _runner = null;
            }
        }

        public void OnInput(NetworkRunner runner, NetworkInput input)
        {
            input.Set(PlayerController.CaptureInput());
        }

        public void OnInputMissing(NetworkRunner runner, PlayerRef player, NetworkInput input) { }
        public void OnConnectedToServer(NetworkRunner runner) { }
        public void OnConnectFailed(NetworkRunner runner, NetAddress remoteAddress, NetConnectFailedReason reason) { }
        public void OnConnectRequest(NetworkRunner runner, NetworkRunnerCallbackArgs.ConnectRequest request, byte[] token) { }
        public void OnCustomAuthenticationResponse(NetworkRunner runner, System.Collections.Generic.Dictionary<string, object> data) { }
        public void OnDisconnectedFromServer(NetworkRunner runner, NetDisconnectReason reason) { }
        public void OnHostMigration(NetworkRunner runner, HostMigrationToken hostMigrationToken) { }
        public void OnPlayerJoined(NetworkRunner runner, PlayerRef player) { }
        public void OnPlayerLeft(NetworkRunner runner, PlayerRef player) { }
        public void OnReliableDataProgress(NetworkRunner runner, PlayerRef player, ReliableKey key, float progress) { }
        public void OnReliableDataReceived(NetworkRunner runner, PlayerRef player, ReliableKey key, System.ArraySegment<byte> data) { }
        public void OnSceneLoadDone(NetworkRunner runner) { }
        public void OnSceneLoadStart(NetworkRunner runner) { }
        public void OnSessionListUpdated(NetworkRunner runner, System.Collections.Generic.List<SessionInfo> sessionList) { }
        public void OnShutdown(NetworkRunner runner, ShutdownReason shutdownReason) { }
        public void OnUserSimulationMessage(NetworkRunner runner, SimulationMessagePtr message) { }
    }
}
