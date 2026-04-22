using Fusion;
using UnityEngine;

namespace MultiplayerZombies.Enemies
{
    public class ZombieSpawner : NetworkBehaviour
    {
        [SerializeField] private NetworkPrefabRef zombiePrefab;
        [SerializeField] private Transform[] spawnPoints;
        [SerializeField] private int baseCount = 4;
        [SerializeField] private int maxAliveZombies = 30;

        public int AliveZombieCount => FindObjectsByType<ZombieController>(FindObjectsSortMode.None).Length;

        public void SpawnWave(int wave)
        {
            if (!Object.HasStateAuthority || !zombiePrefab.IsValid)
            {
                return;
            }

            var desired = baseCount + wave * 2;
            var currentlyAlive = AliveZombieCount;
            var availableSlots = Mathf.Max(0, maxAliveZombies - currentlyAlive);
            var toSpawn = Mathf.Min(desired, availableSlots);

            for (var i = 0; i < toSpawn; i++)
            {
                var point = spawnPoints != null && spawnPoints.Length > 0
                    ? spawnPoints[Random.Range(0, spawnPoints.Length)]
                    : transform;

                Runner.Spawn(zombiePrefab, point.position, Quaternion.identity);
                Debug.Log($"[Spawn] Spawned zombie {i + 1}/{toSpawn} for wave {wave}");
            }
        }
    }
}
