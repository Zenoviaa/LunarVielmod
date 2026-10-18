using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Core.Utilities;

//The issue is we're currently recalculating the ENTIRE MAP every half a second
//This is really slow, 4 ms to recalculate on intensive areas is abyssmal

//1. What we should do is create a WaterSurfaceScanner, which scans for water surface points and then outputs them
//2. Then we have a WaterSurfacePointLightMap, which takes those points and creates surface lights, likely dictionary lookup of point to light
//3. If the point is no longer a valid surface point or it's very far from the player it'll be removed
//4. If necessary, only create lights on water tiles that are surrounded by water to further optimize it
//5. Don't create an entirely new array for every single calculation, create a buffer of maximum expected vertices and use that until it runs out

//This might neot be the fastest impl, but I'll see how afst this structure is first
//Since it caches the raycast calculations it can't be slow

enum WaterSurfaceState : byte
{
    INACTIVE,
    ACTIVE
}

public struct WaterSurfaceLightData
{
    public WaterSurfaceLightData(int x, int y, int z)
    {
        TileX = x;
        TileY = y;
        VertexStartIndex = z;
    }

    public WaterSurfaceLightData(Point p, int z)
    {
        TileX = p.X;
        TileY = p.Y;
        VertexStartIndex = z;
    }

    public int TileX;
    public int TileY;
    public int VertexStartIndex;
}
public class WaterSurfacePointLightMap
{
    float LightRadius => 212;
    WaterSurfaceState[,] _waterSurfacePoints;
    ulong[] _bitMask;
    Queue<Point> _waterSurfaceLightVertexPool;
    List<Point> _activeSurfactLightVertexPoints;
    WaterSurfaceLightData[] _activeWaterSurfaceLights;
    SurfaceLightVertex[] _vertices;
    int[] _indices;
    readonly Vector2[] _offsets;
    int _width;
    (int chunkIndex, byte bitIndex) GetBitMaskIndices(int tileX, int tileY)
    {
        var rawIndex = (tileX + tileY * _width);
        var chunkIndex = rawIndex / 64;
        var bitIndex = rawIndex % 64;
        return (chunkIndex, (byte)bitIndex);
    }

    void SetBitMask(int chunkIndex, byte bitIndex, bool state)
    {
        byte indexToSetTo = state ? (byte)1 : (byte)0;
        ulong chunk = _bitMask[chunkIndex];
        ulong mask = (ulong)(indexToSetTo << bitIndex);
        chunk |= mask;
        _bitMask[chunkIndex] = chunk;
    }

    bool GetBitMask(int chunkIndex, byte bitIndex)
    {
        ulong chunk = _bitMask[chunkIndex];
        return (chunk & (1U << bitIndex)) == 1;
    }

    void SetTileSolidMask(Rectangle area)
    {
        for (var x = area.Left; x < area.Right; x++)
        {
            for (var y = area.Top; y < area.Bottom; y++)
            {
                var tile = Main.tile[x, y];
                var indices = GetBitMaskIndices(x, y);
                bool isActive = (tile.HasTile && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type]);
                SetBitMask(indices.chunkIndex, indices.bitIndex, isActive);
            }
        }
    }

    public WaterSurfacePointLightMap(int lightCount, int sideCount, int width, int height)
    {
        VertexCountPerLight = 1 + sideCount * 2;
        _width = width;
        var bitMaskSize = (width * height) / 64;
        _bitMask = new ulong[bitMaskSize];
        _waterSurfacePoints = new WaterSurfaceState[width, height];
        _activeSurfactLightVertexPoints = new ();
        _activeWaterSurfaceLights = new WaterSurfaceLightData[lightCount];
        //calculate the buffer
        var vertices = new List<SurfaceLightVertex>();
        var indices = new List<int>();

        _offsets = new Vector2[VertexCountPerLight];
        for (var i = 1F; i < _offsets.Length; i++)
        {
            var ratio = (i - 1) / _offsets.Length;
            var radians = ratio * MathHelper.TwoPi;
            _offsets[(int)i] = radians.ToRotationVector2();
        }
        _waterSurfaceLightVertexPool = new();
        var offset = 0;
        for(var i = 0; i < lightCount; i++)
        {
            var light = new SurfaceLightCircle(sideCount);
            _waterSurfaceLightVertexPool.Enqueue(new Point(i, i * light.Vertices.Length));
            vertices.AddRange(light.Vertices);
            foreach (var index in light.Indices)
            {
                indices.Add(index + offset);
            }
            offset += light.Vertices.Length;
        }
        _vertices = vertices.ToArray();
        _indices = indices.ToArray();
    }
    public readonly int VertexCountPerLight;

    public void ApplyWaterSurfacePoints(IEnumerable<Vector3> points)
    {
        var cameraTileRect = TileUtilities.CameraTileRectangle(384);
        var centerTile = Main.Camera.Center.ToTileCoordinates();


        var toRemove = new List<Point>();
        for (var i = 0; i < _activeSurfactLightVertexPoints.Count; i++)
        {
            Point p = _activeSurfactLightVertexPoints[i];
            var lightIndex = p.X;
            ref var surfaceLightData = ref _activeWaterSurfaceLights[lightIndex];
            var pt = new Point(surfaceLightData.TileX, surfaceLightData.TileY);
            if (!cameraTileRect.Contains(pt))
            {
                //Remove this light
                _waterSurfaceLightVertexPool.Enqueue(p);
                _waterSurfacePoints[pt.X, pt.Y] = WaterSurfaceState.INACTIVE;
                toRemove.Add(p);
            }
        }
        foreach (var t in toRemove)
            _activeSurfactLightVertexPoints.Remove(t);
        /*
    using(new TickTimer())
    {

        Main.NewText(cameraTileRect.Width * cameraTileRect.Height);
        for (var x = cameraTileRect.Left; x < cameraTileRect.Right; x++)
        {
            for (var y = cameraTileRect.Top; y < cameraTileRect.Bottom; y++)
            {
                var tile = Main.tile[x, y];
                var indices = GetBitMaskIndices(x, y);
                bool isActive = (tile.HasTile && Main.tileSolid[tile.type] && !Main.tileSolidTop[tile.type]);
               SetBitMask(indices.chunkIndex, indices.bitIndex, isActive);
                //_tileMask[x, y] = isActive ? (byte)1 : (byte)0;
            }
        }
    }*/
        var size = (int)(LightRadius / 16F);

        //Remove every active water surface light that's off screen
        //For each point, check if the light is initialized, if it's not, we get what from the pool and calculate vertices
        foreach (var point in points)
        {
            var centerPosition = point.XY();
            var tp = centerPosition.ToTileCoordinates();
            //A light has been placed here already don't put another one
            if (_waterSurfacePoints[tp.X, tp.Y] == WaterSurfaceState.ACTIVE)
                continue;
       
            //Move the vertices
            if (_waterSurfaceLightVertexPool.Count <= 0)
                break;
            var lightDataIndex = _waterSurfaceLightVertexPool.Dequeue();
            _activeSurfactLightVertexPoints.Add(lightDataIndex);
            var startIndex = lightDataIndex.Y;
            var lightIndex = lightDataIndex.X;
            _vertices[startIndex] = new(new Vector3(centerPosition, 0), Color.White * point.Z, Vector3.One);

            var rect = TileUtilities.CenterTileRectangle(centerPosition.ToTileCoordinates(), size, size);
            SetTileSolidMask(rect);
            for (var i = 1; i < VertexCountPerLight; i++)
            {
                ref var vertex = ref _vertices[startIndex + i];
                var offset = _offsets[i];
                var pos = centerPosition;
                var stepSize = 16f;
                var maxSteps = LightRadius / stepSize;
                var traveledDistance = 0F;
                for (var j = 0; j < maxSteps; j++)
                {
                    pos += offset * stepSize;
                    traveledDistance += stepSize;
                    var tileCoords = pos.ToTileCoordinates();
                    var indices = GetBitMaskIndices(tileCoords.X, tileCoords.Y);
                    if (GetBitMask(indices.chunkIndex, indices.bitIndex))
                        break;
                }
                vertex.Position = new Vector3(pos, 0);
                var dist = traveledDistance / LightRadius;

                //Z coordinate in this case is how far this vertex is from the center point
                //We'll use the number so the gradient is always the same no matter where it is
                vertex.TextureCoordinate = new Vector3(0, 0, dist);
                vertex.Color = (Color.White * point.Z).ToVector4();
            }

            //Set the surface light active
            ref var surfaceLightData = ref _activeWaterSurfaceLights[lightIndex];
            surfaceLightData.TileX = tp.X;
            surfaceLightData.TileY = tp.Y;
            surfaceLightData.VertexStartIndex = startIndex;
            _waterSurfacePoints[tp.X, tp.Y] = WaterSurfaceState.ACTIVE;
        }    
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, _vertices, 0, _vertices.Length, _indices, 0, _indices.Length / 3);
    }
}
public static class WaterSurfaceScanner
{
    public static IEnumerable<Vector3> Scan(Rectangle area)
    {

        var waterSurfacePoints = new List<Point>();
        for (int x = area.Left; x < area.Right; x++)
        {
            for (int y = area.Top; y < area.Bottom; y++)
            {
                if (x % 2 != 0)
                    continue;

                var tile = Main.tile[x, y];
                if (tile.LiquidAmount == 0)
                    continue;

                if (tile.HasTile)
                    continue;
        
                var tileAbove = Main.tile[x, y - 1];
                if (tileAbove.LiquidAmount <= 0
                    && !WorldGen.SolidOrSlopedTile(tileAbove))
                {
                    waterSurfacePoints.Add(new Point(x, y - 1));
                }
            }
        }

        var worldPoints = new List<Vector3>();

        foreach (var tp in waterSurfacePoints)
        {
            var airCount = 0F;
            var rect = TileUtilities.CenterTileRectangle(tp, 4, 4);
            for (var x = rect.Left; x < rect.Right; x++)
            {
                for (var y = rect.Top; y < rect.Bottom; y++)
                {
                    var t = Main.tile[x, y];
                    if (!t.HasTile && t.LiquidAmount <= 0)
                    {
                        airCount++;
                    }
                }
            }

            var size = (float)(rect.Width * rect.Height);
            size *= 0.5f;
            worldPoints.Add(new Vector3(tp.ToWorldCoordinates(), airCount / size));
        }

        return worldPoints;
    }
}

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
