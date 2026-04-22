using UnityEngine;

namespace MultiplayerZombies.Combat
{
    public static class DamageSystem
    {
        public static void ApplyDamage(IDamageable target, int amount, DamageContext context)
        {
            if (target == null || !target.IsAlive || amount <= 0)
            {
                return;
            }

            Debug.Log($"[Damage] Applying {amount} damage to {target} from {context.Instigator}");
            target.TakeDamage(amount, context);
        }
    }
}
