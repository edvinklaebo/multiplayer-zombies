using Fusion;
using MultiplayerZombies.Combat;
using MultiplayerZombies.Core;
using MultiplayerZombies.Player;
using UnityEngine;

namespace MultiplayerZombies.Enemies
{
    public class ZombieController : NetworkBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 60;
        [SerializeField] private float moveSpeed = 2.4f;
        [SerializeField] private float attackRange = 1.5f;
        [SerializeField] private int attackDamage = 10;
        [SerializeField] private float attacksPerSecond = 1f;
        [SerializeField] private NetworkPrefabRef ammoPickupPrefab;
        [SerializeField] private float baseDropChance = 0.25f;
        [SerializeField] private float movementBonus = 0.25f;
        [SerializeField] private float proximityBonus = 0.15f;
        [SerializeField] private float proximityDistance = 8f;

        [Networked] public int Health { get; private set; }
        public static int AliveCount { get; private set; }

        private float _nextAttackTime;

        public bool IsAlive => Health > 0;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                Health = maxHealth;
                AliveCount++;
            }
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            if (hasState)
            {
                AliveCount = Mathf.Max(0, AliveCount - 1);
            }

            base.Despawned(runner, hasState);
        }

        public override void FixedUpdateNetwork()
        {
            if (!Object.HasStateAuthority || !IsAlive || GameManager.Instance == null)
            {
                return;
            }

            if (!GameManager.Instance.TryGetNearestAlivePlayer(transform.position, out var target))
            {
                return;
            }

            var toTarget = target.transform.position - transform.position;
            var distance = toTarget.magnitude;

            if (distance > attackRange)
            {
                var direction = toTarget.normalized;
                transform.position += direction * (moveSpeed * Runner.DeltaTime);
                transform.rotation = Quaternion.LookRotation(direction, Vector3.up);
                return;
            }

            var now = (float)Runner.SimulationTime;
            if (now < _nextAttackTime)
            {
                return;
            }

            _nextAttackTime = now + 1f / Mathf.Max(1f, attacksPerSecond);
            DamageSystem.ApplyDamage(target, attackDamage, new DamageContext(PlayerRef.None));
        }

        public void TakeDamage(int amount, DamageContext context)
        {
            if (!Object.HasStateAuthority || !IsAlive)
            {
                return;
            }

            Health = Mathf.Max(0, Health - amount);
            Debug.Log($"[Damage] Zombie {Object.Id} now has {Health} hp");

            if (Health == 0)
            {
                TrySpawnAmmoDrop(context);
                Runner.Despawn(Object);
            }
        }

        private void TrySpawnAmmoDrop(DamageContext context)
        {
            if (!ammoPickupPrefab.IsValid)
            {
                return;
            }

            var dropChance = baseDropChance;

            if (GameManager.TryGetPlayerController(context.Instigator, out var player))
            {
                dropChance += player.RecentMovementMagnitude * movementBonus;
                if (Vector3.Distance(player.transform.position, transform.position) <= proximityDistance)
                {
                    dropChance += proximityBonus;
                }
            }

            if (Random.value <= Mathf.Clamp01(dropChance))
            {
                Runner.Spawn(ammoPickupPrefab, transform.position + Vector3.up * 0.5f, Quaternion.identity);
                Debug.Log($"[Spawn] Ammo dropped at {transform.position}");
            }
        }
    }
}
