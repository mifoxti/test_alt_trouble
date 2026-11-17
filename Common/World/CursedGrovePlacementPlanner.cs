using System;
using System.Collections.Generic;

namespace TheLastTestament.Common.World;

/// <summary>A bounded, seed-stable surface region reserved for one Cursed Grove patch.</summary>
public readonly record struct CursedGroveRegion(int CenterTileX, int HalfWidthTiles, int Seed)
{
    public int LeftTileX => CenterTileX - HalfWidthTiles;
    public int RightTileX => CenterTileX + HalfWidthTiles;
}

/// <summary>
/// Selects candidate horizontal ranges without modifying Terraria tiles. Terrain suitability,
/// biome tiles, structures, and final worldgen pass integration are handled by the caller.
/// </summary>
public static class CursedGrovePlacementPlanner
{
    public const int MinimumHalfWidthTiles = 90;
    public const int MaximumHalfWidthTiles = 150;
    public const int SpawnClearanceTiles = 220;
    public const int MinimumGapBetweenRegions = 180;
    public const int MaximumAttemptsPerRegion = 64;

    /// <summary>
    /// Returns up to <paramref name="desiredCount"/> non-overlapping candidate regions.
    /// Small worlds may yield one grove; unsuitable or narrow worlds can yield none.
    /// </summary>
    public static IReadOnlyList<CursedGroveRegion> Plan(
        int worldWidthTiles,
        int spawnTileX,
        int worldSeed,
        int desiredCount = 2)
    {
        if (worldWidthTiles <= 0)
            throw new ArgumentOutOfRangeException(nameof(worldWidthTiles));
        if (spawnTileX < 0 || spawnTileX >= worldWidthTiles)
            throw new ArgumentOutOfRangeException(nameof(spawnTileX));
        if (desiredCount < 0 || desiredCount > 4)
            throw new ArgumentOutOfRangeException(nameof(desiredCount), "Plan zero to four bounded regions.");
        if (desiredCount == 0)
            return Array.Empty<CursedGroveRegion>();

        int edgePadding = Math.Min(150, worldWidthTiles / 20);
        int minCenter = edgePadding + MaximumHalfWidthTiles;
        int maxCenter = worldWidthTiles - edgePadding - MaximumHalfWidthTiles - 1;
        if (maxCenter <= minCenter)
            return Array.Empty<CursedGroveRegion>();

        var random = new StableRandom(unchecked((uint)worldSeed));
        var regions = new List<CursedGroveRegion>(desiredCount);

        for (int regionIndex = 0; regionIndex < desiredCount; regionIndex++)
        {
            bool placed = false;
            for (int attempt = 0; attempt < MaximumAttemptsPerRegion; attempt++)
            {
                int halfWidth = random.Next(MinimumHalfWidthTiles, MaximumHalfWidthTiles + 1);
                int centerX = random.Next(minCenter, maxCenter + 1);

                if (Math.Abs(centerX - spawnTileX) < halfWidth + SpawnClearanceTiles)
                    continue;

                if (!HasRequiredGap(regions, centerX, halfWidth))
                    continue;

                int regionSeed = unchecked((int)random.NextUInt());
                regions.Add(new CursedGroveRegion(centerX, halfWidth, regionSeed));
                placed = true;
                break;
            }

            if (!placed)
                break;
        }

        return regions.AsReadOnly();
    }

    private static bool HasRequiredGap(List<CursedGroveRegion> regions, int centerX, int halfWidth)
    {
        foreach (CursedGroveRegion existing in regions)
        {
            int requiredDistance = halfWidth + existing.HalfWidthTiles + MinimumGapBetweenRegions;
            if (Math.Abs(centerX - existing.CenterTileX) < requiredDistance)
                return false;
        }

        return true;
    }

    private sealed class StableRandom
    {
        private uint state;

        public StableRandom(uint seed) => state = seed == 0 ? 0xA341316Cu : seed;

        public int Next(int minInclusive, int maxInclusive)
        {
            if (minInclusive > maxInclusive)
                throw new ArgumentOutOfRangeException(nameof(maxInclusive));

            uint range = (uint)(maxInclusive - minInclusive + 1);
            return minInclusive + (int)(NextUInt() % range);
        }

        public uint NextUInt()
        {
            state ^= state << 13;
            state ^= state >> 17;
            state ^= state << 5;
            return state;
        }
    }
}
