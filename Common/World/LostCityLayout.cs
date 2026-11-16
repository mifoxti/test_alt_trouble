using System;
using System.Collections.Generic;

namespace TheLastTestament.Common.World;

/// <summary>
/// A deterministic, renderer-independent layout shared by the dream scene and a future explorable subworld.
/// Coordinates are in abstract pixels; a renderer or tile generator maps them to its own scale.
/// </summary>
public sealed class LostCityLayout
{
    public const int DefaultWidth = 3200;
    public const int DefaultHeight = 900;

    public int Seed { get; }
    public int Width { get; }
    public int Height { get; }
    public IReadOnlyList<CityBuilding> Buildings { get; }
    public IReadOnlyList<CityMoth> Moths { get; }

    private LostCityLayout(int seed, int width, int height, CityBuilding[] buildings, CityMoth[] moths)
    {
        Seed = seed;
        Width = width;
        Height = height;
        Buildings = buildings;
        Moths = moths;
    }

    public static LostCityLayout Generate(int seed, int width = DefaultWidth, int height = DefaultHeight)
    {
        if (width < 800)
            throw new ArgumentOutOfRangeException(nameof(width), "The dream street needs room for several facades.");
        if (height < 400)
            throw new ArgumentOutOfRangeException(nameof(height), "The dream city needs enough vertical space for its towers.");

        var random = new StableRandom(unchecked((uint)seed));
        int count = random.Next(10, 15);
        var buildings = new CityBuilding[count];
        int cursor = -80;

        for (int i = 0; i < count; i++)
        {
            int buildingWidth = random.Next(150, 340);
            int buildingHeight = random.Next(height / 3, height - 100);
            int archStyle = random.Next(0, 4);
            int windowSeed = random.Next(1, int.MaxValue);
            buildings[i] = new CityBuilding(cursor, buildingWidth, buildingHeight, archStyle, windowSeed);
            cursor += buildingWidth - random.Next(20, 100);
        }

        int mothCount = random.Next(8, 17);
        var moths = new CityMoth[mothCount];
        for (int i = 0; i < moths.Length; i++)
        {
            moths[i] = new CityMoth(
                random.Next(0, width),
                random.Next(height / 4, height - 90),
                random.Next(45, 150),
                random.Next(0, 360));
        }

        return new LostCityLayout(seed, width, height, buildings, moths);
    }
}

public readonly struct CityBuilding
{
    public int X { get; }
    public int Width { get; }
    public int Height { get; }
    public int ArchStyle { get; }
    public int WindowSeed { get; }

    public CityBuilding(int x, int width, int height, int archStyle, int windowSeed)
    {
        X = x;
        Width = width;
        Height = height;
        ArchStyle = archStyle;
        WindowSeed = windowSeed;
    }
}

public readonly struct CityMoth
{
    public int X { get; }
    public int Y { get; }
    public int OrbitRadius { get; }
    public int Phase { get; }

    public CityMoth(int x, int y, int orbitRadius, int phase)
    {
        X = x;
        Y = y;
        OrbitRadius = orbitRadius;
        Phase = phase;
    }
}

/// <summary>A small PRNG with fixed behavior across runtime versions so clients and future generators agree.</summary>
internal sealed class StableRandom
{
    private uint state;

    public StableRandom(uint seed) => state = seed == 0 ? 0x9E3779B9u : seed;

    public int Next(int minInclusive, int maxExclusive)
    {
        if (minInclusive >= maxExclusive)
            throw new ArgumentOutOfRangeException(nameof(maxExclusive));

        uint value = NextUInt();
        return minInclusive + (int)(value % (uint)(maxExclusive - minInclusive));
    }

    private uint NextUInt()
    {
        state ^= state << 13;
        state ^= state >> 17;
        state ^= state << 5;
        return state;
    }
}
