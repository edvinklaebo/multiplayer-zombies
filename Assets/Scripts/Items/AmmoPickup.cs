using Fusion;
using MultiplayerZombies.Player;
using UnityEngine;

namespace MultiplayerZombies.Items
{
    public class AmmoPickup : NetworkBehaviour
    {
        [SerializeField] private int ammoAmount = 20;

        private void OnTriggerEnter(Collider other)
        {
            if (!Object.HasStateAuthority)
            {
                return;
            }

            var shooter = other.GetComponentInParent<PlayerShooting>();
            if (shooter != null && shooter.TryAddAmmo(ammoAmount))
            {
                Runner.Despawn(Object);
            }
        }
    }
}
