using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Core.LunarLightingSystem
{
    public struct TileShadow
    {
        public int vertexOffset;
        public int primitiveCount;
        public bool useSunBuffer;
    }
    public static class TileSunShadowVertexBuffer
    {
        private static int _index;
        public static VertexPositionColor[] ShadowVertices = new VertexPositionColor[Max_Sun_Shadow_Vertices];
        public const int Max_Sun_Shadow_Vertices = 12 * 20000;


        public static int GetVertexOffset()
        {
            return _index;
        }
        public static bool IsFull()
        {
            return _index >= Max_Sun_Shadow_Vertices;
        }


        public static void Clear()
        {
            _index = 0;
        }

        public static int AddTriangle()
        {
            int index = _index;
            _index += 3;
            return index;
        }
    }
    public static class TileShadowVertexBuffer
    {
        private static int _index;
        public static VertexPositionColor[] ShadowVertices = new VertexPositionColor[Max_Torch_Shadow_Vertices];
        public const int Max_Torch_Shadow_Vertices = 6 * 100000;

        public static int GetVertexOffset()
        {
            return _index;
        }


        public static bool IsFull()
        {
            return _index >= Max_Torch_Shadow_Vertices;
        }

        public static void Clear()
        {
            _index = 0;
        }

        public static int AddTriangle()
        {
            int index = _index;
            _index += 3;
            return index;
        }
    }
    public static class TileShading
    {
        private static Color _shadowColor;
        private static void MoveVertex(ref VertexPositionColor point, PointLight pointLight)
        {
            if (point.Position.Z <= 0)
                return;
    
            if(pointLight.directionOverride != Vector2.Zero)
            {
                point.Position += new Vector3(pointLight.directionOverride, 0);
            }
            else
            {
                float radius = pointLight.radius;
                if (pointLight.faint)
                    radius *= 0.1f;
                Vector2 dis = new Vector2(point.Position.X, point.Position.Y) - pointLight.position;
                Vector2 offset = dis / MathF.Sqrt(dis.X * dis.X + dis.Y * dis.Y) * radius;
                point.Position += new Vector3(offset, 0);
            }
        }

        public static void AddQuad(Vector2 xy1, Vector2 xy2, PointLight pointLight, bool useSunBuffer)
        {
            if(useSunBuffer && TileSunShadowVertexBuffer.IsFull())
            {
                return;
            }
            else if (!useSunBuffer && TileShadowVertexBuffer.IsFull())
            {
                return;
            }
            //For the shadow color I want to take the inverse of the pointlight color and then lerp it towards black a bit       
            VertexPositionColor tl1 = new VertexPositionColor(new Vector3(xy1, 0), _shadowColor);
            VertexPositionColor tr1 = new VertexPositionColor(new Vector3(xy1, 1), _shadowColor );
            VertexPositionColor br1 = new VertexPositionColor(new Vector3(xy2, 0), _shadowColor);

            VertexPositionColor tl2 = new VertexPositionColor(new Vector3(xy1, 1), _shadowColor);
            VertexPositionColor tr2 = new VertexPositionColor(new Vector3(xy2, 0), _shadowColor );
            VertexPositionColor br2 = new VertexPositionColor(new Vector3(xy2, 1), _shadowColor);

            MoveVertex(ref tl1, pointLight);
            MoveVertex(ref tr1, pointLight);
            MoveVertex(ref br1, pointLight);
            MoveVertex(ref tl2, pointLight);
            MoveVertex(ref tr2, pointLight);
            MoveVertex(ref br2, pointLight);


            if (useSunBuffer)
            {
                int tri1Index = TileSunShadowVertexBuffer.AddTriangle();

                //0, 1, 2
                TileSunShadowVertexBuffer.ShadowVertices[tri1Index] = tl1;
                TileSunShadowVertexBuffer.ShadowVertices[tri1Index + 1] = tr1;
                TileSunShadowVertexBuffer.ShadowVertices[tri1Index + 2] = br1;

                //0, 1, 3
                int tri2Index = TileSunShadowVertexBuffer.AddTriangle();

                TileSunShadowVertexBuffer.ShadowVertices[tri2Index] = tl2;
                TileSunShadowVertexBuffer.ShadowVertices[tri2Index + 1] = tr2;
                TileSunShadowVertexBuffer.ShadowVertices[tri2Index + 2] = br2;
            }
            else
            {
                int tri1Index = TileShadowVertexBuffer.AddTriangle();

                //0, 1, 2
                TileShadowVertexBuffer.ShadowVertices[tri1Index] = tl1;
                TileShadowVertexBuffer.ShadowVertices[tri1Index + 1] = tr1;
                TileShadowVertexBuffer.ShadowVertices[tri1Index + 2] = br1;

                //0, 1, 3
                int tri2Index = TileShadowVertexBuffer.AddTriangle();

                TileShadowVertexBuffer.ShadowVertices[tri2Index] = tl2;
                TileShadowVertexBuffer.ShadowVertices[tri2Index + 1] = tr2;
                TileShadowVertexBuffer.ShadowVertices[tri2Index + 2] = br2;
            }

        }

        public static TileShadow PrepareTilesForShading(PointLight pointLight, bool useSunBuffer)
        {
            Vector2 topLeftOfPointLight = pointLight.position - new Vector2(pointLight.radius);
            Vector2 bottomRightOfPointLight = pointLight.position + new Vector2(pointLight.radius);


            Point topLeftTile = topLeftOfPointLight.ToTileCoordinates();
            Point bottomRightTIle = bottomRightOfPointLight.ToTileCoordinates();
            return PrepareTilesForShading(topLeftTile.X, topLeftTile.Y, bottomRightTIle.X, bottomRightTIle.Y, pointLight, useSunBuffer);
        }

        public static TileShadow PrepareTilesForShading(
            int startTileX, int startTileY,
            int endTileX, int endTileY, PointLight pointLight, bool useSunBuffer)
        {

            Color color = Color.Black * 0.3f;
            if (pointLight.faint)
                color *= 0.18f;
            _shadowColor = color;
            TileShadow tileShadow = new TileShadow();
            tileShadow.vertexOffset = TileShadowVertexBuffer.GetVertexOffset();
            if (useSunBuffer)
                tileShadow.vertexOffset = TileSunShadowVertexBuffer.GetVertexOffset();
            for (int x = startTileX; x < endTileX; x++)
            {
                for (int y = startTileY; y < endTileY; y++)
                {
                    //If a tile is outside of the world just ignore it, otherwise we'll get an error
                    if (!WorldGen.InWorld(x, y))
                        continue;
                    Tile tile = Main.tile[x, y];
                    if (!tile.HasTile)
                        continue;


                    //Only cast a shadow if a tile is touching air, so we aren't drawing unnecessary shadows
                    if (!WorldGen.TileIsExposedToAir(x, y))
                        continue;

                    if (!Main.tileSolid[tile.TileType])
                        continue;


                    //Now we calculate vertices
                    //There's no texture here so it doesn't matter what order we do the triangles in
                    //Pretty sure we start from top left?
                    Point tilePoint = new Point(x, y);
                    Vector2 worldPoint = tilePoint.ToWorldCoordinates(0, 0);

                    //Vertex 0
                    Vector2 topLeft = worldPoint;

                    //Vertex 1
                    Vector2 topRight = worldPoint + new Vector2(16, 0);

                    //Vertex 2
                    Vector2 bottomLeft = worldPoint + new Vector2(0, 16);

                    //Vertex 3
                    Vector2 bottomRight = worldPoint + new Vector2(16, 16);

                    AddQuad(topLeft, bottomRight, pointLight, useSunBuffer);
                    AddQuad(topRight, bottomLeft, pointLight, useSunBuffer);
                    tileShadow.primitiveCount += 4;
                }
            }
            tileShadow.useSunBuffer = useSunBuffer; 
            return tileShadow;
        }


        public static void DrawVertices(TileShadow tileShadow)
        {
            if (tileShadow.primitiveCount <= 0)
                return;

            var shader = TileShadowShader.Instance;
            shader.Apply();
            foreach (var pass in shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }

            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            BlendState originalBlendState = graphicsDevice.BlendState;
            CullMode oldCullMode = graphicsDevice.RasterizerState.CullMode;
            SamplerState originalSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.RasterizerState.CullMode = CullMode.None;
            graphicsDevice.BlendState = BlendState.AlphaBlend;

            var vertexBuffer = tileShadow.useSunBuffer ? TileSunShadowVertexBuffer.ShadowVertices : TileShadowVertexBuffer.ShadowVertices;
            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, vertexBuffer, tileShadow.vertexOffset, tileShadow.primitiveCount);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
    }
}
