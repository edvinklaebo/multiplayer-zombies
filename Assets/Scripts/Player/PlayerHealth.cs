using Fusion;
using MultiplayerZombies.Combat;
using MultiplayerZombies.Core;
using UnityEngine;

namespace MultiplayerZombies.Player
{
    public class PlayerHealth : NetworkBehaviour, IDamageable
    {
        [SerializeField] private int maxHealth = 100;

        [Networked] public int Health { get; private set; }
        [Networked] public NetworkBool Dead { get; private set; }

        public bool IsAlive => !Dead;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                Health = maxHealth;
                Dead = false;
            }

            GameManager.RegisterPlayer(this);
        }

        public override void Despawned(NetworkRunner runner, bool hasState)
        {
            GameManager.UnregisterPlayer(this);
            base.Despawned(runner, hasState);
        }

        public void TakeDamage(int amount, DamageContext context)
        {
            if (!Object.HasStateAuthority || Dead)
            {
                return;
            }

            Health = Mathf.Max(0, Health - amount);
            Debug.Log($"[Damage] Player {Object.InputAuthority} now has {Health} hp");

            if (Health == 0)
            {
                Dead = true;
                Debug.Log($"[Player] Player {Object.InputAuthority} died");
            }
        }
    }
}
