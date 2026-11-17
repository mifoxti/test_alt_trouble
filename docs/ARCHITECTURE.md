# Architecture

## Project target

- Stable tModLoader: `1.4.4` (`v2026.09.3.1` on the development machine).
- Runtime target: `net8.0`, C# 12, as declared by the installed `tMLMod.targets`.
- The mod assembly and tModLoader ID are `TheLastTestament`.

## Current modules

| Module | Responsibility |
| --- | --- |
| `Content/DamageClasses` | Independent Occultist and Specter damage types. |
| `Common/Players` | Per-player class resources and temporary class states. |
| `Common/Systems` | Balance settings and world progression persistence. |
| `Common/NPCs` | Small shared helpers for Cursed Grove enemy state and targeting. |
| `Content/NPCs/CursedGrove` | Three bespoke enemy state machines and their hostile projectiles. |
| `Localization` | English and Russian player-facing strings. |
| `Tools/AssetGeneration` | Reproducible, source-controlled sprite generation. |

The first Cursed Grove enemy AI implementations now live in focused `ModNPC` classes. NPC state and hostile projectile spawning are synchronized through tModLoader; damaging projectiles are created by the server or the single-player client. The dedicated biome, world generation, animation sheets, bosses, and class-combat content remain future modules.

## Resources

`ClassResourcePlayer` owns Corruption and Ethereality values. `BalanceConfig` centralizes their limits, thresholds, regeneration, and phase timing. Resource drawing is deliberately separate from resource state so the visual design can be approved and iterated independently.

## Lost City extension point

`LostCityLayout` now generates close facade dimensions and moth-light placements from a saved per-world seed. Its data does not depend on a particular rendering path. The dream scene can draw that layout now; a later Subworld Library adapter can consume the same layout to build an explorable Terraria subworld. This avoids coupling the story sequence to the later dimension implementation.

See [Subworld Library research](SUBWORLDS.md) for the integration decision and constraints.
