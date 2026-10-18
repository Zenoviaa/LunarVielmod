using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;

namespace Stellamod.Core.Utilities;


public struct TickTimer : IDisposable
{
    public TickTimer()
    {
        watch = Stopwatch.StartNew();
    }
    public Stopwatch watch;
    public void Dispose()
    {
        watch.Stop();
        Main.NewText($"{watch.ElapsedTicks} ticks");
    //    throw new NotImplementedException();
    }
}

public class VertexCircle
{
    public VertexCircle(int sides, ref int connectIndex)
    {
        //each side needs 3 vertices
        Vertices = new VertexPositionColorTexture[1 + sides * 2];
        Offsets = new Vector2[Vertices.Length];
        for (var i = 1F; i < Vertices.Length; i++)
        {
            ref var vertex = ref Vertices[(int)i];
            var ratio = (i - 1) / Vertices.Length;
            var radians = ratio * MathHelper.TwoPi;
            var offset = radians.ToRotationVector2();
            Offsets[(int)i] = offset;
        }
        var startIndex = connectIndex;
        var indiceList = new List<int>();
        for (var i = 0; i < sides * 6; i += 3)
        {
            indiceList.Add(connectIndex);
            indiceList.Add(startIndex);
            indiceList.Add(connectIndex + 1); //connectIndex + 1;
            connectIndex += 1;
        }

        indiceList.Add(connectIndex);
        indiceList.Add(startIndex);
        indiceList.Add(startIndex + 1);
        Indices = indiceList.ToArray();
    }
    public VertexCircle(int sides)
    {
        //each side needs 3 vertices
        Vertices = new VertexPositionColorTexture[1 + sides * 2];
        Offsets = new Vector2[Vertices.Length];
        for (var i = 1F; i < Vertices.Length; i++)
        {
            ref var vertex = ref Vertices[(int)i];
            var ratio = (i - 1) / Vertices.Length;
            var radians = ratio * MathHelper.TwoPi;
            var offset = radians.ToRotationVector2();
            Offsets[(int)i] = offset;
        }
        var connectIndex = 0;
        var indiceList = new List<int>();
        for (var i = 0; i < sides * 6; i += 3)
        {
            indiceList.Add(connectIndex);
            indiceList.Add(0);
            indiceList.Add(connectIndex + 1); //connectIndex + 1;
            connectIndex += 1;
        }

        indiceList.Add(connectIndex);
        indiceList.Add(0);
        indiceList.Add(1);
        Indices = indiceList.ToArray();
    }
    public readonly Vector2[] Offsets;
    public readonly VertexPositionColorTexture[] Vertices;
    public readonly int[] Indices;
    public void PrepareVertices(Vector2 centerPosition, float radius)
    {
        Vertices[0] = new(new Vector3(centerPosition, 0), Color.White, Vector2.One);
        for (var i = 1; i < Vertices.Length; i++)
        {
            ref var vertex = ref Vertices[i];
            var offset = Offsets[i];
            var pos = centerPosition;
            var maxSteps = radius / 16f;
            for (var j = 0; j < maxSteps; j++)
            {
                pos += offset * 16;
                if (WorldGen.SolidTile(pos.ToTileCoordinates()))
                    break;
            }

            vertex.Position = new Vector3(pos, 0);
            vertex.TextureCoordinate = Vector2.Zero;
        }
    }
    public void SetStrength(float strength)
    {
        Vertices[0].Color = Color.White * strength;
    }
    public void Draw(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
    }
}
