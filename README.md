# multiplayer-zombies

Unity 6.3 + Photon Fusion host-authoritative multiplayer wave defense prototype.

## Implemented MVP scripts

- Host/client bootstrap (`NetworkBootstrap`)
- Player input + movement (`PlayerController`)
- Host-validated hitscan shooting with local muzzle flash (`PlayerShooting`, `HitScanWeapon`)
- Host-only zombie AI and wave spawning (`ZombieController`, `ZombieSpawner`)
- Host-only centralized damage handling (`DamageSystem`)
- Ammo pickup/drop flow with anti-camping bonuses (`AmmoPickup`, zombie drop chance)
- Game state + wave progression (`GameManager`, `GameState`)
- Basic HUD data binding (`HUDController`)
- Authority debug labels and combat/spawn logs for debugging
