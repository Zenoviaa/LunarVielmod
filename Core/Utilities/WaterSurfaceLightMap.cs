using System.Collections.Generic;
using Terraria;

namespace Stellamod.Core.Utilities;

public class WaterSurfaceLightMap
{
    SurfaceLightVertex[] _vertices;
    int[] _indices;
    public WaterSurfaceLightMap()
    {

    }

    public void ReCalculate(Rectangle area, float surfaceLightRadius = 164)
    {
        area = TileUtilities.Clamp(area);
        if (area.Y == 0)
            area.Y += 1;

        var waterSurfacePoints = new List<Point>();
        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                var tile = Main.tile[x, y];
                var tileAbove = Main.tile[x, y - 1];
                if (tile.LiquidAmount > 0 && tileAbove.LiquidAmount <= 0 && !WorldGen.SolidOrSlopedTile(tileAbove))
                {
                    waterSurfacePoints.Add(new Point(x, y - 1));
                }
            }
        }
        var worldPoints = new List<Vector3>();
        var vertexCircles = new List<SurfaceLightCircle>();
        foreach (var tp in waterSurfacePoints)
        {
            var airCount = 0F;
            var rect = TileUtilities.CenterTileRectangle(tp, 8, 8);
            for(var x = rect.Left; x < rect.Right; x++)
            {
                for(var y = rect.Top; y < rect.Bottom; y++)
                {
                    var t = Main.tile[x, y];
                    if(!t.HasTile && t.LiquidAmount <= 0)
                    {
                        airCount++;
                    }
                }
            }

            var size = (float)(rect.Width * rect.Height);
            size *= 0.5f;
            worldPoints.Add(new Vector3(tp.ToWorldCoordinates(), airCount / size));
        }

        foreach (var wp in worldPoints)
        {
            var surfaceLight = new SurfaceLightCircle(16);
            surfaceLight.PrepareVertices(wp.XY(), surfaceLightRadius);
            surfaceLight.SetStrength(wp.Z);
            vertexCircles.Add(surfaceLight);
        }

        var vertices = new List<SurfaceLightVertex>();
        var indices = new List<int>();
        var offset = 0;
        foreach (var surfaceLight in vertexCircles)
        {
            vertices.AddRange(surfaceLight.Vertices);
            foreach (var index in surfaceLight.Indices)
            {
                indices.Add(index + offset);
            }
            offset += surfaceLight.Vertices.Length;
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
