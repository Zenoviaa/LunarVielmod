using System;
using System.Collections.Generic;

namespace Stellamod.Common.TornadoRendering;

/// <summary>
/// Represents a cylindrical mesh, there is no top or bottom. For use with tornados!
/// </summary>
public class TornadoMesh
{
    public TornadoMesh(float height, float radius, float heightPerBand, int sidesPerBand, Color tornadoColor, Func<float, float> sizeMultiplierFunc)
    {
        var vertices = new List<VertexPositionColorTexture>();
        //Calculate the number of bands
        var numberOfBands = height / heightPerBand;
        var yHeight = 0F;
        var halfQuadSize = new Vector2(radius / sidesPerBand, heightPerBand / 2f);

        var matricies = new Matrix[sidesPerBand];

        for (var v = 0; v < sidesPerBand; v++)
        {
            var ratio = v / (float)(sidesPerBand - 1);
            var radians = ratio * MathHelper.TwoPi;
            matricies[v] = Matrix.CreateFromAxisAngle(new Vector3(0, 1f, 0), radians);
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
        for (var n = 0; n < numberOfBands; n++)
        {

            bandPoints[n] = new Vector3[sidesPerBand];
            texCoords[n] = new Vector2[sidesPerBand];
            for (var v = 0; v < sidesPerBand; v++)
            {
                var ratio = n / (float)numberOfBands;
                var bandRadius = radius * ratio;
                bandRadius *= sizeMultiplierFunc(ratio);
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


                var color = Color.Lerp(tornadoColor, Color.Transparent, n / (float)numberOfBands);
                var nextColor = Color.Lerp(tornadoColor, Color.Transparent, (n + 1) / (float)numberOfBands);
                var normal = new Vector3(0, 0, -1);


                var topLeftVertex = new VertexPositionColorTexture(topLeft, nextColor, topLeftCoord);
                var topRightVertex = new VertexPositionColorTexture(topRight, nextColor, topRightCoord);
                var bottomLeftVertex = new VertexPositionColorTexture(bottomLeft, color, bottomLeftCoord);
                var bottomRightVertex = new VertexPositionColorTexture(bottomRight, color, bottomRightCoord);

                vertices.Add(topLeftVertex);
                vertices.Add(topRightVertex);
                vertices.Add(bottomLeftVertex);
                vertices.Add(bottomRightVertex);

                primitiveCount += 1;
            }
        }

        Vertices = vertices.ToArray();
        Indices = DrawUtilities.TexturedQuadIndices(primitiveCount);
    }

    public readonly VertexPositionColorTexture[] Vertices;
    public readonly int[] Indices;
    public void Draw(GraphicsDevice graphicsDevice)
    {
        graphicsDevice.DrawUserIndexedPrimitives(PrimitiveType.TriangleList,
            Vertices, 0, Vertices.Length, Indices, 0, Indices.Length / 3);
    }
}
