using Microsoft.CodeAnalysis.CSharp.Syntax;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Core.Utilities;

/// <summary>
/// Represents a cylindrical mesh, there is no top or bottom. For use with tornados!
/// </summary>
public class TornadoMesh
{
    public TornadoMesh(float height, float radius, float heightPerBand, int sidesPerBand)
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
