# Subworld Library research

**Research checked 16 November 2026.** The local game installation is tModLoader stable 1.4.4, v2026.09.3.1.

## Candidate libraries

Two relevant releases now exist:

1. The [original Subworld Library](https://github.com/jjohnsnaill/SubworldLibrary) by John Snail. Its current Steam page still lists tModLoader 1.4.4, and its change log shows a maintenance update in November 2025. It provides compact dimensions, world-generation tasks, persistence controls, loading screens, and multiplayer routing.
2. The [Subworld Library Community Edition](https://github.com/Path-of-Terraria/SubworldLibraryCommunityFork), a community-maintained fork published for tModLoader 1.4.4 in August 2026. Its stated goal is current compatibility fixes and community-reviewed maintenance.

The community edition changes the internal mod/dependency ID to `SubworldLibraryCommunityFork` and the C# namespace to `SubworldLibraryCommunityFork`. Its API type names remain `Subworld` and `SubworldSystem`. The original library uses the `SubworldLibrary` ID/namespace. **They must not be enabled together** because both install hooks for the same game systems.

Both expose the shape needed by the Lost City: derive from `Subworld`, provide dimensions and `GenPass` tasks, then enter and exit through `SubworldSystem`. The library handles travel, world save placement, and a subserver for occupied multiplayer dimensions. It can also configure a subworld and player changes as temporary.

## Integration recommendation

- Keep the dream's deterministic city layout generator independent of Subworld Library types.
- Implement the cinematic first encounter first, using the same world-seeded layout data.
- The cinematic street composition should include localized light sources (moth-like drifting lights), while keeping the generated building layout reproducible for later tile generation.
- Before the Lost City becomes explorable, re-check which library is actively maintained and tested against that tModLoader release. Current evidence favors evaluating the Community Edition first, while the original still has a 1.4.4 Workshop listing.
- Treat the library as an explicit, documented mod dependency when real subworld travel is added. Do not bundle its code into The Last Testament.
- During that milestone, test entry/exit, save persistence, disconnects, host migration expectations, dedicated server setup, and multiplayer player synchronization. The library routes occupied subworlds through a subserver, so network/firewall behavior needs real multiplayer verification.
- Never enable the original and Community Edition together.

## API references

- [Original library README and design notes](https://github.com/jjohnsnaill/SubworldLibrary/blob/master/README.md)
- [Original Subworld Library wiki](https://github.com/jjohnsnaill/SubworldLibrary/wiki)
- [Community Edition repository and migration notes](https://github.com/Path-of-Terraria/SubworldLibraryCommunityFork)
- [Community Edition Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=3782861152)
- [Original Workshop page](https://steamcommunity.com/sharedfiles/filedetails/?id=2785100219)
