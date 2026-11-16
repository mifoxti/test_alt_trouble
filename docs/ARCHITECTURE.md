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
| `Localization` | English and Russian player-facing strings. |
| `Tools/AssetGeneration` | Reproducible, source-controlled sprite generation. |

Future NPC, boss, combat, rendering, UI, and world-generation systems will be kept in focused modules. Multiplayer actions that affect combat will be validated on the server, while graphics and interface code will be client-only.

## Resources

`ClassResourcePlayer` owns Corruption and Ethereality values. `BalanceConfig` centralizes their limits, thresholds, regeneration, and phase timing. Resource drawing is deliberately separate from resource state so the visual design can be approved and iterated independently.

## Lost City extension point

`LostCityLayout` now generates close facade dimensions and moth-light placements from a saved per-world seed. Its data does not depend on a particular rendering path. The dream scene can draw that layout now; a later Subworld Library adapter can consume the same layout to build an explorable Terraria subworld. This avoids coupling the story sequence to the later dimension implementation.

See [Subworld Library research](SUBWORLDS.md) for the integration decision and constraints.
