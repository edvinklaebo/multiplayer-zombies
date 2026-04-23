using Fusion;

public class ZombieController : NetworkBehaviour
{
    [Networked] public int Health { get; set; } = 3;

    public void TakeDamage(int dmg)
    {
        if (!Object.HasStateAuthority) return;

        Health -= dmg;

        if (Health <= 0)
        {
            Runner.Despawn(Object);
        }
    }
}