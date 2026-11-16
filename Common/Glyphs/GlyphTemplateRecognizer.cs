using System;
using System.Collections.Generic;

namespace TheLastTestament.Common.Glyphs;

/// <summary>A point captured from the player's cursor path in screen-space coordinates.</summary>
public readonly record struct GlyphPoint(float X, float Y);

/// <summary>
/// A named one-stroke glyph template. Template paths are normalized by the recognizer
/// before comparison, so their original size and position do not matter.
/// </summary>
public sealed record GlyphTemplate(string Id, IReadOnlyList<GlyphPoint> Points);

public readonly record struct GlyphRecognitionResult(string GlyphId, float Confidence, bool IsRecognized);

/// <summary>
/// Lightweight unistroke recognizer using path resampling, indicative-angle alignment,
/// square scaling, centroid translation, and pointwise path distance.
/// </summary>
public static class GlyphTemplateRecognizer
{
    public const int SampleCount = 64;
    public const float DefaultRecognitionThreshold = 0.20f;

    public static GlyphRecognitionResult Recognize(
        IReadOnlyList<GlyphPoint> input,
        IReadOnlyList<GlyphTemplate> templates,
        float recognitionThreshold = DefaultRecognitionThreshold)
    {
        if (!TryNormalize(input, out GlyphPoint[] candidate) || templates is null || templates.Count == 0)
            return new GlyphRecognitionResult(string.Empty, 0f, false);

        recognitionThreshold = float.IsFinite(recognitionThreshold) && recognitionThreshold > 0f
            ? recognitionThreshold
            : DefaultRecognitionThreshold;

        string bestId = string.Empty;
        float bestDistance = float.MaxValue;

        for (int i = 0; i < templates.Count; i++)
        {
            GlyphTemplate template = templates[i];
            if (template is null || string.IsNullOrWhiteSpace(template.Id)
                || !TryNormalize(template.Points, out GlyphPoint[] normalizedTemplate))
                continue;

            float distance = PathDistance(candidate, normalizedTemplate);
            if (distance < bestDistance)
            {
                bestDistance = distance;
                bestId = template.Id;
            }
        }

        if (bestId.Length == 0)
            return new GlyphRecognitionResult(string.Empty, 0f, false);

        float confidence = Math.Clamp(1f - bestDistance / recognitionThreshold, 0f, 1f);
        return new GlyphRecognitionResult(bestId, confidence, bestDistance <= recognitionThreshold);
    }

    private static bool TryNormalize(IReadOnlyList<GlyphPoint> input, out GlyphPoint[] normalized)
    {
        normalized = Array.Empty<GlyphPoint>();
        if (input is null || input.Count < 2)
            return false;

        var points = new GlyphPoint[input.Count];
        for (int i = 0; i < input.Count; i++)
        {
            GlyphPoint point = input[i];
            if (!float.IsFinite(point.X) || !float.IsFinite(point.Y))
                return false;
            points[i] = point;
        }

        if (!TryResample(points, out GlyphPoint[] resampled))
            return false;
        points = resampled;

        Center(points);
        double indicativeAngle = Math.Atan2(points[0].Y, points[0].X);
        Rotate(points, -indicativeAngle);
        ScaleToSquare(points);
        Center(points);
        normalized = points;
        return true;
    }

    private static bool TryResample(GlyphPoint[] points, out GlyphPoint[] samples)
    {
        samples = Array.Empty<GlyphPoint>();
        var cumulativeLength = new double[points.Length];
        for (int i = 1; i < points.Length; i++)
        {
            double dx = points[i].X - points[i - 1].X;
            double dy = points[i].Y - points[i - 1].Y;
            cumulativeLength[i] = cumulativeLength[i - 1] + Math.Sqrt(dx * dx + dy * dy);
        }

        double totalLength = cumulativeLength[^1];
        if (!double.IsFinite(totalLength) || totalLength < 0.001d)
            return false;

        samples = new GlyphPoint[SampleCount];
        int segment = 1;
        for (int sample = 0; sample < SampleCount; sample++)
        {
            double targetLength = totalLength * sample / (SampleCount - 1);
            while (segment < cumulativeLength.Length - 1 && cumulativeLength[segment] < targetLength)
                segment++;

            double segmentStart = cumulativeLength[segment - 1];
            double segmentLength = cumulativeLength[segment] - segmentStart;
            float t = segmentLength <= double.Epsilon
                ? 0f
                : (float)((targetLength - segmentStart) / segmentLength);

            GlyphPoint start = points[segment - 1];
            GlyphPoint end = points[segment];
            samples[sample] = new GlyphPoint(
                start.X + (end.X - start.X) * t,
                start.Y + (end.Y - start.Y) * t);
        }

        return true;
    }

    private static void Center(GlyphPoint[] points)
    {
        double sumX = 0d;
        double sumY = 0d;
        for (int i = 0; i < points.Length; i++)
        {
            sumX += points[i].X;
            sumY += points[i].Y;
        }

        float centerX = (float)(sumX / points.Length);
        float centerY = (float)(sumY / points.Length);
        for (int i = 0; i < points.Length; i++)
            points[i] = new GlyphPoint(points[i].X - centerX, points[i].Y - centerY);
    }

    private static void Rotate(GlyphPoint[] points, double angle)
    {
        float cosine = (float)Math.Cos(angle);
        float sine = (float)Math.Sin(angle);
        for (int i = 0; i < points.Length; i++)
        {
            GlyphPoint point = points[i];
            points[i] = new GlyphPoint(
                point.X * cosine - point.Y * sine,
                point.X * sine + point.Y * cosine);
        }
    }

    private static void ScaleToSquare(GlyphPoint[] points)
    {
        float minX = float.MaxValue;
        float minY = float.MaxValue;
        float maxX = float.MinValue;
        float maxY = float.MinValue;

        for (int i = 0; i < points.Length; i++)
        {
            GlyphPoint point = points[i];
            minX = Math.Min(minX, point.X);
            minY = Math.Min(minY, point.Y);
            maxX = Math.Max(maxX, point.X);
            maxY = Math.Max(maxY, point.Y);
        }

        float width = Math.Max(maxX - minX, 0.0001f);
        float height = Math.Max(maxY - minY, 0.0001f);
        for (int i = 0; i < points.Length; i++)
            points[i] = new GlyphPoint(points[i].X / width, points[i].Y / height);
    }

    private static float PathDistance(GlyphPoint[] first, GlyphPoint[] second)
    {
        double total = 0d;
        for (int i = 0; i < SampleCount; i++)
        {
            double dx = first[i].X - second[i].X;
            double dy = first[i].Y - second[i].Y;
            total += Math.Sqrt(dx * dx + dy * dy);
        }

        return (float)(total / SampleCount);
    }
}
