using Microsoft.CodeAnalysis.CSharp.Syntax;
using Stellamod.Common.Shaders;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Core.Utilities;


/*
To make this tornado, I think to get the 3D'ness of it, the easiest way would be to make and spin a cylinder mesh,
and scroll/blend a scrolling texture along that mesh/
So we'ed need to create multiple quads
FURTHERMORE, the curves and movement tornados can do is REALLY cool
I don't want to make a perfectly uniform tornado as that looks a bit stiff, I wanna make the best tornado everrr


Let's break this down into steps
1. Create several quads around a central point, it should divide the size of the tornado to see how many bands to make

The reason we need so many bands is for the distortion and movement that the tornado needs to do, it's not uniform
If we curve it correctly and make the top lazily follow the bottom, it'll give it a very dynamic form

2. Scroll a texture along the mesh and make the back sides of the quads darken, we could also cull 
counter clockwise faces it depends on whatever looks better, but I think I want counterclockwise faces to just be darker
if push comes to shove we just render it twice, just the second time culls the opposite side and the 2nd one is darker/rendered first

*/
[Serializable]
[StructLayout(LayoutKind.Sequential, Pack = 1)]
public struct VertexPositionColorNormalTexture : IVertexType
{
    public Vector3 Position;

    public Vector3 Normal;

    public Vector2 TextureCoordinate;
    public Color Color;
    public static readonly VertexDeclaration VertexDeclaration;

    VertexDeclaration IVertexType.VertexDeclaration => VertexDeclaration;

    static VertexPositionColorNormalTexture()
    {
        VertexDeclaration = new VertexDeclaration(
            new VertexElement(0, VertexElementFormat.Vector3, VertexElementUsage.Position, 0), 
            new VertexElement(12, VertexElementFormat.Vector3, VertexElementUsage.Normal, 0), 
            new VertexElement(24, VertexElementFormat.Vector2, VertexElementUsage.TextureCoordinate, 0),
            new VertexElement(32, VertexElementFormat.Color, VertexElementUsage.Color, 0));
    }

    public VertexPositionColorNormalTexture(Vector3 position, Color color,Vector2 textureCoordinate,  Vector3 normal )
    {
        Position = position;
        Normal = normal;
        TextureCoordinate = textureCoordinate;
        Color = color;
    }

    public override int GetHashCode()
    {
        return 0;
    }

    public override string ToString()
    {
        return "{{Position:" + Position.ToString() + " Normal:" + Normal.ToString() + " TextureCoordinate:" + TextureCoordinate.ToString() + "}}";
    }

    public static bool operator ==(VertexPositionColorNormalTexture left, VertexPositionColorNormalTexture right)
    {
        if (left.Position == right.Position && left.Normal == right.Normal)
        {
            return left.TextureCoordinate == right.TextureCoordinate;
        }

        return false;
    }

    public static bool operator !=(VertexPositionColorNormalTexture left, VertexPositionColorNormalTexture right)
    {
        return !(left == right);
    }

    public override bool Equals(object obj)
    {
        if (obj == null)
        {
            return false;
        }

        if (obj.GetType() != GetType())
        {
            return false;
        }

        return this == (VertexPositionColorNormalTexture)obj;
    }
}
public class StormingTornado : ModProjectile
{
    CylinderMesh _closeCylinderMesh;
    CylinderMesh _cylinderMesh;
    TexturedQuad _quad;
    ref float Timer => ref Projectile.ai[0];
    public override string Texture => TextureRegistry.EmptyTexture;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Projectile.tileCollide = false;
        Projectile.light = 0.6f;
        Projectile.width = 16;
        Projectile.height = 16;
        Projectile.timeLeft = 600;
    }
    public override bool ShouldUpdatePosition()
    {
        return false;
    }
    public override void AI()
    {
        base.AI();
        Timer++;

    }
    public override bool PreDraw(ref Color lightColor)
    {
 
        var spriteBatch = Main.spriteBatch;
        spriteBatch.EndOut(out var oldParameters);
        _cylinderMesh ??= new CylinderMesh(1024, radius: 128, heightPerBand: 256, sidesPerBand: 16);
        _cylinderMesh.SetColor(Color.Lerp(Color.White, Color.Black, 0.75f));
        _closeCylinderMesh ??= new CylinderMesh(1024, 128, 256, 16);
        
        var gDevice = spriteBatch.GraphicsDevice;
        gDevice.RasterizerState = RasterizerState.CullNone;
        gDevice.BlendState = BlendState.AlphaBlend;
        gDevice.SamplerStates[0] = SamplerState.LinearWrap;

        var pass = AssetReferences.Effects.Weather.ThunderingTornado.CreatePrimitivesPass();

        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix = 
            Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), Main.GlobalTimeWrappedHourly * 1)
            * Matrix.CreateTranslation(Projectile.Center.X, Projectile.Center.Y, 0) 
            * TrailDrawer.WorldViewPointCullClose;
        pass.Apply();
        Main.graphics.GraphicsDevice.Textures[0]  = AssetReferences.Assets.Noise.TornadoNoise.Asset.Value;
        _cylinderMesh.SetFades(Main.GlobalTimeWrappedHourly, 1f);
        _cylinderMesh.Draw(gDevice);

        pass.Parameters.time = Main.GlobalTimeWrappedHourly;
        pass.Parameters.transformMatrix =
            Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), Main.GlobalTimeWrappedHourly * 1)
            * Matrix.CreateTranslation(Projectile.Center.X, Projectile.Center.Y, 0)
            * TrailDrawer.WorldViewPointCullFarAway;
        pass.Apply();
        gDevice.BlendState = BlendState.AlphaBlend;
        _closeCylinderMesh.SetFades(Main.GlobalTimeWrappedHourly);
        _closeCylinderMesh.Draw(gDevice);
        spriteBatch.Begin(oldParameters);

        return false;
    }
}

public class DebugWand : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.DebugWand.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToStaff(ModContent.ProjectileType<StormingTornado>(), 1, 30, 1);
    }

}

/// <summary>
/// Represents a cylindrical mesh, there is no top or bottom. For use with tornados!
/// </summary>
public class CylinderMesh
{
    public CylinderMesh(float height, float radius, float heightPerBand, int sidesPerBand)
    {
        var vertices = new List<VertexPositionColorNormalTexture>();
        //Calculate the number of bands
        var numberOfBands = height / heightPerBand;
        var yHeight = 0F;
        var halfQuadSize = new Vector2(radius / (float)sidesPerBand, heightPerBand / 2f);

        var matricies = new Matrix[sidesPerBand];
        for(var v = 0; v < sidesPerBand; v++)
        {
            var ratio = (float)v / (float)(sidesPerBand-1);
            var radians = ratio * MathHelper.TwoPi;
            matricies[v] = Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), radians);
        }

        //So what we need to do 
        //Is create bands of points, not quads
        //and then create quads between those bands of points so they connect correctly
    //    var bandPoints = new Vector3[(int)numberOfBands * (int)sidesPerBand];
        var bandPoints = new Vector3[(int)numberOfBands][];


        var xTexScale = 1f / (sidesPerBand - 1);
        var yTexScale = 3f;
        var xScale = 0F;
        var yScale = 0F;
        var texCoords = new Vector2[(int)numberOfBands][];
        for(var n = 0; n < numberOfBands; n++)
        {
     
            bandPoints[n] = new Vector3[sidesPerBand];
            texCoords[n] = new Vector2[sidesPerBand];
            for (var v = 0; v < sidesPerBand; v++)
            {
                var ratio = (float)n / (float)numberOfBands;
                var bandRadius = radius * ratio + 8f;
                ref var bandPoint = ref bandPoints[n][v];
                bandPoint = new Vector3(0, -yHeight, -bandRadius * 2);

                ref var transformationMatrix = ref matricies[v];
                bandPoint = Vector3.Transform(bandPoint, transformationMatrix);

                ref var texCoord = ref texCoords[n][v];
                texCoord = new Vector2(xScale, yScale);
                xScale += xTexScale;
            }
            yHeight += heightPerBand;
            yScale += yTexScale;
        }

        //For any given point index, if we offset by the sides per band we get the point opposite of it
        //if we add 1 we get the point to the right of it
        //remember we start from the bottom so
        //hey wait this method is more performant than the old one :sob:
        var primitiveCount = 0;
        for (var n = 0; n < numberOfBands - 1; n++)
        {
       
            for (var v = 0; v < sidesPerBand; v++)
            {
                var leftIndex = v;
                var rightIndex = (v + 1) % sidesPerBand;

                var bottomLeft = bandPoints[n][leftIndex];
                var bottomRight = bandPoints[n][rightIndex];
                var topLeft = bandPoints[n + 1][leftIndex];
                var topRight = bandPoints[n + 1][rightIndex];

                var bottomLeftCoord = texCoords[n][leftIndex];
                var bottomRightCoord = texCoords[n][rightIndex];
                var topLeftCoord = texCoords[n + 1][leftIndex];
                var topRightCoord = texCoords[n + 1][rightIndex];

                var primaryColor = Color.Lerp(Color.White, Color.White, (float)v / (float)sidesPerBand);
                var color = Color.Lerp(primaryColor, Color.Transparent, (float)n / (float)numberOfBands);
                var nextColor = Color.Lerp(primaryColor, Color.Transparent, (float)(n + 1) / (float)numberOfBands);
                var normal = new Vector3(0, 0, -1);

     
                var topLeftVertex = new VertexPositionColorNormalTexture(topLeft, nextColor, topLeftCoord, normal);
                var topRightVertex = new VertexPositionColorNormalTexture(topRight, nextColor, topRightCoord, normal);
                var bottomLeftVertex = new VertexPositionColorNormalTexture(bottomLeft, color, bottomLeftCoord, normal);
                var bottomRightVertex = new VertexPositionColorNormalTexture(bottomRight, color, bottomRightCoord, normal);

                vertices.Add(topLeftVertex);
                vertices.Add(topRightVertex);
                vertices.Add(bottomLeftVertex);
                vertices.Add(bottomRightVertex);
   
                primitiveCount += 1;
            }
        }

        /*
        for (var n = 0; n < numberOfBands; n++)
        {
            for(var v = 0; v < sidesPerBand; v++)
            {
                var ratio = (float)n / (float)numberOfBands;
                var bandRadius = radius * ratio;
                var topLeft = new Vector3(-halfQuadSize.X, -halfQuadSize.Y - yHeight, -bandRadius * 2);
                var topRight = new Vector3(halfQuadSize.X, -halfQuadSize.Y - yHeight, -bandRadius * 2);
                var bottomLeft = new Vector3(-halfQuadSize.X, halfQuadSize.Y - yHeight, -bandRadius * 2);
                var bottomRight = new Vector3(halfQuadSize.X, halfQuadSize.Y - yHeight, -bandRadius * 2);

                ref var transformationMatrix = ref matricies[v];
                topLeft = Vector3.Transform(topLeft, transformationMatrix);
                topRight = Vector3.Transform(topRight, transformationMatrix);
                bottomLeft = Vector3.Transform(bottomLeft, transformationMatrix);
                bottomRight = Vector3.Transform(bottomRight, transformationMatrix);

                var topLeftVertex = new VertexPositionColorTexture(topLeft, Color.White, new Vector2(0, 0));
                var topRightVertex = new VertexPositionColorTexture(topRight, Color.White, new Vector2(1, 0));
                var bottomLeftVertex = new VertexPositionColorTexture(bottomLeft, Color.White, new Vector2(0, 1));
                var bottomRightVertex = new VertexPositionColorTexture(bottomRight, Color.White, new Vector2(1, 1));

                vertices.Add(topLeftVertex);
                vertices.Add(topRightVertex);
                vertices.Add(bottomLeftVertex);
                vertices.Add(bottomRightVertex);
            }

   
        }*/

        Vertices = vertices.ToArray();
        Indices = DrawUtilities.TexturedQuadIndices(primitiveCount);
    }
    public readonly VertexPositionColorNormalTexture[] Vertices;
    public readonly int[] Indices;
    public void SetFades(float radians, float dir)
    {
        var sidesPerBand = 16;
        var matrix = Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), radians);


        for (var h = 0; h < Vertices.Length; h++)
        {
            ref var normal = ref Vertices[h].Normal;
            var position = Vertices[h].Position;
            position = Vector3.Transform(position, matrix);
            var fade = (dir * position.Z) / 64F;
            //fade = MathF.Pow(fade, 0.5F);
            // fade = 1f - fade;
            normal.X = fade;

        }
    }
    public void SetFades(float radians)
    {
        var sidesPerBand = 16;
        var matrix = Matrix.CreateFromAxisAngle(new Vector3(0, 1, 0), radians);


        for (var h = 0; h < Vertices.Length; h++)
        {
            ref var normal = ref Vertices[h].Normal;
            var position = Vertices[h].Position;
            position = Vector3.Transform(position, matrix);
            var fade = -position.Z / 64F;
            //fade = MathF.Pow(fade, 0.5F);
           // fade = 1f - fade;
            normal.X = fade; 
           
        }
    }
    public void SetColor(Color c)
    {
        for(var h = 0; h < Vertices.Length; h++)
        {
            Vertices[h].Color = c;
        }
    }
    public void Draw(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
    }
}
