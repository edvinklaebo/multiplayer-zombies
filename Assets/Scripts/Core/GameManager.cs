using System.Collections.Generic;
using Fusion;
using MultiplayerZombies.Enemies;
using MultiplayerZombies.Player;
using UnityEngine;

namespace MultiplayerZombies.Core
{
    public class GameManager : NetworkBehaviour
    {
        [SerializeField] private ZombieSpawner zombieSpawner;
        [SerializeField] private float waveDelaySeconds = 2f;

        [Networked] public GameState State { get; private set; }
        [Networked] public int Wave { get; private set; }
        [Networked] private TickTimer NextWaveTimer { get; set; }

        public static GameManager Instance { get; private set; }

        private readonly HashSet<PlayerHealth> _players = new();

        private void Awake()
        {
            Instance = this;
        }

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                State = GameState.Lobby;
                Wave = 0;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority)
            {
                return;
            }

            if (State == GameState.Lobby && AlivePlayers() > 0)
            {
                StartGame();
            }

            if (State == GameState.Playing)
            {
                if (AlivePlayers() == 0)
                {
                    State = GameState.GameOver;
                    Debug.Log("[Game] Game over: all players are dead");
                    return;
                }

                if (zombieSpawner != null && zombieSpawner.AliveZombieCount == 0)
                {
                    if (!NextWaveTimer.IsRunning)
                    {
                        NextWaveTimer = TickTimer.CreateFromSeconds(Runner, waveDelaySeconds);
                    }
                    else if (NextWaveTimer.Expired(Runner))
                    {
                        NextWaveTimer = TickTimer.None;
                        StartNextWave();
                    }
                }
            }
        }

        public static void RegisterPlayer(PlayerHealth player)
        {
            if (Instance != null)
            {
                Instance._players.Add(player);
            }
        }

        public static void UnregisterPlayer(PlayerHealth player)
        {
            if (Instance != null)
            {
                Instance._players.Remove(player);
            }
        }

        public bool TryGetNearestAlivePlayer(Vector3 fromPosition, out PlayerHealth player)
        {
            player = null;
            var nearestDistance = float.MaxValue;

            foreach (var candidate in _players)
            {
                if (candidate == null || !candidate.IsAlive)
                {
                    continue;
                }

                var distance = Vector3.SqrMagnitude(candidate.transform.position - fromPosition);
                if (distance < nearestDistance)
                {
                    nearestDistance = distance;
                    player = candidate;
                }
            }

            return player != null;
        }

        public static bool TryGetPlayerController(PlayerRef playerRef, out PlayerController controller)
        {
            controller = null;
            if (playerRef == PlayerRef.None)
            {
                return false;
            }

            var players = FindObjectsByType<PlayerController>(FindObjectsSortMode.None);
            foreach (var player in players)
            {
                if (player != null && player.Object != null && player.Object.InputAuthority == playerRef)
                {
                    controller = player;
                    return true;
                }
            }

            return false;
        }

        private int AlivePlayers()
        {
            var alive = 0;
            foreach (var player in _players)
            {
                if (player != null && player.IsAlive)
                {
                    alive++;
                }
            }

            return alive;
        }

        private void StartGame()
        {
            State = GameState.Playing;
            Debug.Log("[Game] State changed: Lobby -> Playing");
            StartNextWave();
        }

        private void StartNextWave()
        {
            Wave++;
            Debug.Log($"[Game] Spawning wave {Wave}");
            zombieSpawner?.SpawnWave(Wave);
        }
    }
}
