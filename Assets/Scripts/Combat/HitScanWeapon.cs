using Fusion;
using UnityEngine;

namespace MultiplayerZombies.Combat
{
    public class HitScanWeapon : NetworkBehaviour
    {
        [SerializeField] private int damage = 20;
        [SerializeField] private float range = 60f;
        [SerializeField] private LayerMask hitMask = ~0;
        [SerializeField] private Transform muzzle;
        [SerializeField] private ParticleSystem muzzleFlash;

        public void Fire(Player.PlayerInputData input)
        {
            if (!Object.HasStateAuthority)
            {
                return;
            }

            var origin = muzzle != null ? muzzle.position : transform.position + Vector3.up;
            var direction = muzzle != null ? muzzle.forward : transform.forward;
            var ray = new Ray(origin, direction);

            Debug.DrawRay(origin, direction * range, Color.red, 1.5f);

            if (Physics.Raycast(ray, out var hit, range, hitMask, QueryTriggerInteraction.Ignore))
            {
                var target = hit.collider.GetComponentInParent<IDamageable>();
                if (target != null)
                {
                    DamageSystem.ApplyDamage(target, damage, new DamageContext(Object.InputAuthority));
                }
            }
        }

        public void PlayLocalMuzzleFlash()
        {
            if (muzzleFlash != null)
            {
                muzzleFlash.Play();
            }
        }
    }
}
