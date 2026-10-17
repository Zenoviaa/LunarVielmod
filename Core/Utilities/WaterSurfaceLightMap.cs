using System.Collections.Generic;
using Terraria;

namespace Stellamod.Core.Utilities;

public class WaterSurfaceLightMap
{
    VertexPositionColorTexture[] _vertices;
    int[] _indices;
    public WaterSurfaceLightMap()
    {
        WaterSurfaceLights = new();
    }
    public readonly List<VertexCircle> WaterSurfaceLights;

    public void ReCalculate(Rectangle area, float surfaceLightRadius = 164)
    {
        area = TileUtilities.Clamp(area);
        if (area.Y == 0)
            area.Y += 1;
        WaterSurfaceLights.Clear();
        var worldPoints = new List<Vector2>();
        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                var tile = Main.tile[x, y];
                var tileAbove = Main.tile[x, y - 1];
                if (tile.LiquidAmount > 0 && tileAbove.LiquidAmount <= 0 && !WorldGen.SolidOrSlopedTile(tileAbove))
                {
                    worldPoints.Add(new Point(x, y - 1).ToWorldCoordinates());
                }
            }
        }


        foreach (var wp in worldPoints)
        {
            var surfaceLight = new VertexCircle(16);
            surfaceLight.PrepareVertices(wp, surfaceLightRadius);
            WaterSurfaceLights.Add(surfaceLight);
        }
        var vertices = new List<VertexPositionColorTexture>();
        var indices = new List<int>();
        var offset = 0;
        foreach (var surfaceLight in WaterSurfaceLights)
        {
            vertices.AddRange(surfaceLight.Vertices);
            foreach (var index in surfaceLight.Indices)
            {
                indices.Add(index + offset);
            }
            offset += surfaceLight.Vertices.Length;
            //indices.AddRange(surfaceLight.Indices);
        }
        _vertices = vertices.ToArray();
        _indices = indices.ToArray();
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        if (_vertices == null || _indices == null)
            return;
        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 3);
    }
}
