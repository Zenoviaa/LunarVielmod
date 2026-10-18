using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.Utilities;

[Autoload(Side = ModSide.Client)]
public class Test : ModSystem
{
    public override void PostDrawTiles()
    {
        base.PostDrawTiles();
        if (Main.gameMenu)
            return;

   //     PixelationManager.QueuePrimitivesDrawAction(DrawLight);
    }

    private void DrawLight(GraphicsDevice graphicsDevice)
    {
        var waterlightPass = AssetReferences.Effects.Generic.WaterLight.CreatePixelPass();
        waterlightPass.Parameters.transformMatrix = TrailDrawer.UnscaledWorldViewPoint;
        waterlightPass.Apply();
        graphicsDevice.RasterizerState = RasterizerState.CullNone;
        graphicsDevice.SamplerStates[0] = SamplerState.AnisotropicClamp;
        graphicsDevice.BlendState = BlendState.Additive;
        graphicsDevice.DepthStencilState = DepthStencilState.None;
        var surfaceLight = new SurfaceLightCircle(16);
        surfaceLight.PrepareVertices(Main.MouseWorld, 256);
        surfaceLight.SetStrength(1f);
        surfaceLight.Draw(graphicsDevice);
    }


}
public struct SurfaceLightVertex : IVertexType
{
    private Vector3 _position;
    private Vector4 _color;
    private Vector3 _texCoord;
    public readonly static VertexDeclaration VertexDeclaration = new VertexDeclaration
    (
        new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0),
        new VertexElement(12, VertexElementFormat.Vector4, VertexElementUsage.Color, 0),
        new VertexElement(28, VertexElementFormat.Vector3, VertexElementUsage.TextureCoordinate, 0)
    );
    public SurfaceLightVertex(Vector3 position, Color color, Vector3 textureCoordinate)
    {
        _position = position;
        _color = color.ToVector4();
        _texCoord = textureCoordinate;
    }

    VertexDeclaration IVertexType.VertexDeclaration
    {
        get { return VertexDeclaration; }
    }

    public Vector3 Position
    {
        get { return _position; }
        set { _position = value; }
    }
    
    public Vector4 Color
    {
        get { return _color; }
        set { _color = value; }
    }

    public Vector3 TextureCoordinate
    {
        get { return _texCoord; }
        set { _texCoord = value; }
    }
}

//TODO: constructor that takes in an offsets array, this would be faster than recalculating it everytime
//TODO: constructor that doesn't need an indices array, we only need vertices
public class SurfaceLightCircle
{
    public SurfaceLightCircle(int sides)
    {

        //each side needs 3 vertices
        Vertices = new SurfaceLightVertex[1 + sides * 2];
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
    public readonly SurfaceLightVertex[] Vertices;
    public readonly int[] Indices;
    public void PrepareVertices(Vector2 centerPosition, float radius)
    {

        Vertices[0] = new(new Vector3(centerPosition, 0), Color.White, Vector3.One);
        for (var i = 1; i < Vertices.Length; i++)
        {
            ref var vertex = ref Vertices[i];
            var offset = Offsets[i];
            var pos = centerPosition;
            var stepSize = 16f;
            var maxSteps = radius / stepSize;
            var traveledDistance = 0F;
            for (var j = 0; j < maxSteps; j++)
            {
                pos += offset * stepSize;
                traveledDistance += stepSize;
                if (WorldGen.SolidOrSlopedTile(Main.tile[pos.ToTileCoordinates()]))
                    break;
            }
            vertex.Position = new Vector3(pos, 0);
            var dist = traveledDistance / radius;
            //Z coordinate in this case is how far this vertex is from the center point
            //We'll use the number so the gradient is always the same no matter where it is
            vertex.TextureCoordinate = new Vector3(0, 0, dist);
        }
    }

    public void SetStrength(float strength)
    {
        for(var i = 0; i < Vertices.Length;i++)
        {
            ref var vertex = ref Vertices[i];
            vertex.Color = (Color.White * strength).ToVector4();
        }
 
    }

    public void Draw(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList, Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
    }
}
