# Repository audit

## Starting state

- Source: `https://github.com/mifoxti/test_alt_trouble`, cloned at its single `Initial commit` on `master`.
- The checkout contained a tModLoader skeleton, one world boss flag, a broken sample-boss file, an overpowered sample sword, furniture, a UI-frame-like icon, and tracked `obj/` compiler output.
- The previous project targeted `net6.0`, imported a `..	ModLoader.targets` file that was not present in this checkout, and had an old `tModLoader.CodeAssist` package reference.
- The boss source used the `ExampleMod` namespace and referenced numerous absent example assets/classes, so it was not a buildable boss implementation.
- The old Russian mod description and item text contained the retired satirical setting.

## Local toolchain

- Installed tModLoader stable: v2026.09.3.1, branch 1.4.4.
- Installed `tMLMod.targets`: .NET 8 / C# 12 and its native mod-packaging step.
- The workspace originally had only .NET 6 runtimes and no SDK. SDK 8.0.428 was installed outside the repository under the local Codex tools directory.

## Foundation changes

- Replaced the old project identity with `TheLastTestament`, targeting net8.0 and importing the local stable tModLoader targets.
- Removed stale sample gameplay, incorrect localization, the empty-frame icon, and tracked build caches. Git history retains the original files.
- Added independent damage classes, resource state with packet synchronization hooks, centralized balance settings, and world story/progression persistence.
- Added a deterministic, renderer-independent Lost City layout with seeded building profiles and moth-light positions.
- Added Russian and English localization roots and project documentation.

## Branching and release process

Feature branches follow `feature/<short-name>`. The foundation is in `feature/last-testament-foundation`; subsequent gameplay systems will use separate feature branches as requested.
