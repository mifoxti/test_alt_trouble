# Testing status

## Environment audit

- tModLoader stable v2026.09.3.1 is installed locally.
- The installed target file declares `net8.0` and C# 12.
- .NET SDK 8.0.428 was installed into the user's local Codex tool directory for this build session; it is not part of the repository.
- `dotnet build TheLastTestament.csproj -v:minimal` compiled and packaged the mod against the installed stable tModLoader targets with 0 warnings and 0 errors.
- On 2026-11-17, the same build was run from `feature/class-resource-gameplay` commit `a50487c`, which contains the bounded Phase Shift and server-validated activation request. The build packaged a 32,161-byte `TheLastTestament.tmod` into the user's tModLoader Mods directory; `enabled.json` lists the mod as enabled.
- The installed package was built from a feature branch, not `master`.

## Pending

- Launch tModLoader and verify that this package loads; no in-game launch has been performed yet.
- Check both new damage classes in game.
- Exercise Phase Shift, resource values, UI, rituals, and class combat in-game.
- Test single-player and multiplayer behavior.
- Verify every generated PNG and every animation frame after art approval.

No gameplay claim in this file should be treated as verified until the matching check is recorded here.
