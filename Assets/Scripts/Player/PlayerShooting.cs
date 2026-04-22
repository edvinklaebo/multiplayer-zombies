using Fusion;
using MultiplayerZombies.Combat;
using UnityEngine;

namespace MultiplayerZombies.Player
{
    public class PlayerShooting : NetworkBehaviour
    {
        [SerializeField] private HitScanWeapon weapon;
        [SerializeField] private int maxAmmo = 120;
        [SerializeField] private int ammoPerShot = 1;
        [SerializeField] private float shotsPerSecond = 8f;

        [Networked] public int Ammo { get; private set; }

        private float _nextServerShotTime;
        private float _nextLocalFxTime;

        public override void Spawned()
        {
            if (Object.HasStateAuthority)
            {
                Ammo = maxAmmo;
            }
        }

        public override void FixedUpdateNetwork()
        {
            if (!GetInput(out PlayerInputData input) || !input.Fire)
            {
                return;
            }

            var now = (float)Runner.SimulationTime;
            var shotInterval = 1f / Mathf.Max(1f, shotsPerSecond);

            if (Object.HasInputAuthority && now >= _nextLocalFxTime)
            {
                _nextLocalFxTime = now + shotInterval;
                weapon?.PlayLocalMuzzleFlash();
            }

            if (!Object.HasStateAuthority || now < _nextServerShotTime || Ammo < ammoPerShot)
            {
                return;
            }

            _nextServerShotTime = now + shotInterval;
            Ammo -= ammoPerShot;
            weapon?.Fire(input);
        }

        public bool TryAddAmmo(int amount)
        {
            if (!Object.HasStateAuthority || amount <= 0)
            {
                return false;
            }

            var previous = Ammo;
            Ammo = Mathf.Clamp(Ammo + amount, 0, maxAmmo);
            return Ammo > previous;
        }
    }
}
