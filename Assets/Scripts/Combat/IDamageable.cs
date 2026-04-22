using Fusion;

namespace MultiplayerZombies.Combat
{
    public readonly struct DamageContext
    {
        public readonly PlayerRef Instigator;

        public DamageContext(PlayerRef instigator)
        {
            Instigator = instigator;
        }
    }

    public interface IDamageable
    {
        bool IsAlive { get; }
        void TakeDamage(int amount, DamageContext context);
    }
}
