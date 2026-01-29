using System.Numerics;
using AVATempest.Levels;
using SkiaSharp;

namespace AVATempest.Rendering;

public class TubeRenderer
{
    private readonly VectorRenderer _vectorRenderer;

    public TubeRenderer(VectorRenderer vectorRenderer)
    {
        _vectorRenderer = vectorRenderer;
    }

    public void Render(SKCanvas canvas, Tube tube, SKColor color, int? highlightSegment = null)
    {
        // Draw the tube segments
        for (int i = 0; i < tube.SegmentCount; i++)
        {
            var segment = tube.Segments[i];
            var segmentColor = (highlightSegment == i)
                ? new SKColor(255, 255, 255)
                : color;

            // Draw the lane lines (from inner to outer)
            _vectorRenderer.DrawLine(canvas, segment.InnerLeft, segment.OuterLeft, segmentColor, 1.5f);

            // Draw inner edge
            _vectorRenderer.DrawLine(canvas, segment.InnerLeft, segment.InnerRight, segmentColor, 1.5f);

            // Draw outer edge
            _vectorRenderer.DrawLine(canvas, segment.OuterLeft, segment.OuterRight, segmentColor, 2f);

            // Draw spikes if present
            if (segment.HasSpike)
            {
                DrawSpike(canvas, segment);
            }

            // Draw electrification if active
            if (segment.IsElectrified)
            {
                DrawElectrification(canvas, segment);
            }
        }

        // Draw the final lane line for closed tubes
        if (!tube.IsOpen)
        {
            var lastSegment = tube.Segments[^1];
            _vectorRenderer.DrawLine(canvas, lastSegment.InnerRight, lastSegment.OuterRight, color, 1.5f);
        }
    }

    private void DrawSpike(SKCanvas canvas, TubeSegment segment)
    {
        var spikeColor = ColorPalette.Spike;
        var innerCenter = segment.GetInnerCenter();
        var spikeEnd = segment.GetCenterAtDepth(segment.SpikeDepth);

        // Draw spike as a series of connected lines
        _vectorRenderer.DrawLine(canvas, innerCenter, spikeEnd, spikeColor, 2f);

        // Add some spiky protrusions
        int spikes = (int)(segment.SpikeDepth * 5) + 1;
        for (int i = 1; i <= spikes; i++)
        {
            float t = i / (float)(spikes + 1);
            var point = Vector2.Lerp(innerCenter, spikeEnd, t);
            var left = segment.GetPointAtDepth(t * segment.SpikeDepth, true);
            var right = segment.GetPointAtDepth(t * segment.SpikeDepth, false);

            float offset = (i % 2 == 0 ? 0.1f : -0.1f) * segment.GetWidthAtDepth(t * segment.SpikeDepth);
            var spikePoint = point + new Vector2(offset, offset);

            _vectorRenderer.DrawLine(canvas, point, spikePoint, spikeColor, 1f, false);
        }
    }

    private void DrawElectrification(SKCanvas canvas, TubeSegment segment)
    {
        var color = ColorPalette.PulsarActive;
        var random = Random.Shared;

        // Draw electric arcs along the segment
        for (int i = 0; i < 3; i++)
        {
            float depth1 = random.NextSingle();
            float depth2 = random.NextSingle();

            var p1 = segment.GetPointAtDepth(depth1, random.Next(2) == 0);
            var p2 = segment.GetPointAtDepth(depth2, random.Next(2) == 0);

            // Add some randomness to create jagged lightning
            var mid = (p1 + p2) / 2 + new Vector2(
                (random.NextSingle() - 0.5f) * 20,
                (random.NextSingle() - 0.5f) * 20);

            _vectorRenderer.DrawLine(canvas, p1, mid, color, 1f, false);
            _vectorRenderer.DrawLine(canvas, mid, p2, color, 1f, false);
        }
    }

    public void RenderDepthLines(SKCanvas canvas, Tube tube, SKColor color, int depthLines = 4)
    {
        // Draw perspective depth lines
        for (int d = 1; d < depthLines; d++)
        {
            float depth = d / (float)depthLines;
            var depthColor = color.WithAlpha((byte)(100 + 50 * depth));

            for (int i = 0; i < tube.SegmentCount; i++)
            {
                var segment = tube.Segments[i];
                var left = segment.GetPointAtDepth(depth, true);
                var right = segment.GetPointAtDepth(depth, false);
                _vectorRenderer.DrawLine(canvas, left, right, depthColor, 0.5f, false);
            }
        }
    }
}
