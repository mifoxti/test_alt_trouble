using System;
using System.Collections.Generic;
using Microsoft.Xna.Framework;

namespace TheLastTestament.Common.Rituals;

public enum RitualPattern
{
    TriangleOfBinding,
    PentagramOfSorrow
}

public readonly record struct RitualSegment(Vector2 Start, Vector2 End);

/// <summary>Deterministic, renderer-free geometry for a temporary candle ritual.</summary>
public sealed class RitualGeometry
{
    private readonly Vector2[] pathVertices;

    public RitualPattern Pattern { get; }
    public Vector2 Center { get; }
    public IReadOnlyList<Vector2> BoundaryVertices { get; }
    public IReadOnlyList<Vector2> PathVertices => pathVertices;

    private RitualGeometry(RitualPattern pattern, Vector2 center, Vector2[] boundary, Vector2[] path)
    {
        Pattern = pattern;
        Center = center;
        BoundaryVertices = Array.AsReadOnly(boundary);
        pathVertices = path;
    }

    public static bool TryCreate(RitualPattern pattern, IReadOnlyList<Vector2> candlePositions, out RitualGeometry? geometry)
    {
        geometry = null;
        int expectedCount = pattern switch
        {
            RitualPattern.TriangleOfBinding => 3,
            RitualPattern.PentagramOfSorrow => 5,
            _ => 0
        };

        if (expectedCount == 0 || candlePositions is null || candlePositions.Count != expectedCount)
            return false;

        Vector2 center = Vector2.Zero;
        for (int i = 0; i < candlePositions.Count; i++)
        {
            Vector2 position = candlePositions[i];
            if (!float.IsFinite(position.X) || !float.IsFinite(position.Y))
                return false;
            center += position;
        }
        center /= candlePositions.Count;

        Vector2[] boundary = new Vector2[candlePositions.Count];
        for (int i = 0; i < candlePositions.Count; i++)
            boundary[i] = candlePositions[i];

        Array.Sort(boundary, (a, b) =>
            MathF.Atan2(a.Y - center.Y, a.X - center.X)
                .CompareTo(MathF.Atan2(b.Y - center.Y, b.X - center.X)));

        if (HasDuplicateOrDegenerateBoundary(boundary))
            return false;

        Vector2[] path = pattern == RitualPattern.PentagramOfSorrow
            ? BuildStarPath(boundary)
            : (Vector2[])boundary.Clone();

        geometry = new RitualGeometry(pattern, center, boundary, path);
        return true;
    }

    public IReadOnlyList<RitualSegment> GetSegments()
    {
        var segments = new RitualSegment[pathVertices.Length];
        for (int i = 0; i < pathVertices.Length; i++)
            segments[i] = new RitualSegment(pathVertices[i], pathVertices[(i + 1) % pathVertices.Length]);
        return segments;
    }

    /// <summary>Uses even-odd fill, so the empty center of the five-point star stays safe.</summary>
    public bool Contains(Vector2 point)
    {
        if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
            return false;

        bool inside = false;
        for (int i = 0, j = pathVertices.Length - 1; i < pathVertices.Length; j = i++)
        {
            Vector2 a = pathVertices[i];
            Vector2 b = pathVertices[j];
            bool crossesScanline = (a.Y > point.Y) != (b.Y > point.Y);
            if (!crossesScanline)
                continue;

            float crossingX = (b.X - a.X) * (point.Y - a.Y) / (b.Y - a.Y) + a.X;
            if (point.X < crossingX)
                inside = !inside;
        }

        return inside;
    }

    private static Vector2[] BuildStarPath(Vector2[] boundary)
    {
        var path = new Vector2[boundary.Length];
        int index = 0;
        for (int i = 0; i < boundary.Length; i++)
        {
            path[i] = boundary[index];
            index = (index + 2) % boundary.Length;
        }
        return path;
    }

    private static bool HasDuplicateOrDegenerateBoundary(Vector2[] vertices)
    {
        const float minimumDistanceSquared = 16f;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 next = vertices[(i + 1) % vertices.Length];
            if (Vector2.DistanceSquared(vertices[i], next) < minimumDistanceSquared)
                return true;
        }

        float twiceArea = 0f;
        for (int i = 0; i < vertices.Length; i++)
        {
            Vector2 a = vertices[i];
            Vector2 b = vertices[(i + 1) % vertices.Length];
            twiceArea += a.X * b.Y - b.X * a.Y;
        }

        return MathF.Abs(twiceArea) < 32f;
    }
}
