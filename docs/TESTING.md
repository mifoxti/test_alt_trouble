# Testing status

## Environment audit

- tModLoader stable v2026.09.3.1 is installed locally.
- The installed target file declares `net8.0` and C# 12.
- .NET SDK 8.0.428 was installed into the user's local Codex tool directory for this build session; it is not part of the repository.
- `dotnet build TheLastTestament.csproj -v:minimal` compiled and packaged the mod, including the first three Cursed Grove enemy classes, against stable tModLoader targets with 0 warnings and 0 errors.

## Pending

- Load the packaged mod in Terraria and confirm the three new enemies appear and can be fought in the temporary night-Forest spawn location.
- Check the game can load the mod and both new damage classes.
- Exercise the burrow telegraph, two branch strikes and recovery; the Moth triple fire fan and falling/burning death; the Pilgrim rune lock, beam and statue death.
- Exercise resources, UI, rituals, and class combat in-game.
- Test single-player and multiplayer behavior.
- Review and approve the revised 8-frame movement previews before wiring animation sheets into the NPCs.
- Verify every generated PNG and every animation frame after art approval.

No gameplay claim in this file should be treated as verified until the matching check is recorded here.
