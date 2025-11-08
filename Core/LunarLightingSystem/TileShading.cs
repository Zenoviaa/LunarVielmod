using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using System;
using System.Collections.Generic;
using Terraria;

namespace Stellamod.Core.LunarLightingSystem
{
    public static class TileShading
    {
        private static Color _shadowColor;
        public static void Test()
        {
            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Point topLeftTile = cameraTopLeft.ToTileCoordinates();
            Point bottomRightTile = cameraBottomRight.ToTileCoordinates();
            PointLight pointLight = new PointLight();
            pointLight.position = Main.Camera.Center - new Vector2(0, 1000);
            pointLight.color = Main.ColorOfTheSkies.ToVector3();
            VertexPositionColor[] vertices = PrepareTilesForShading(topLeftTile.X, topLeftTile.Y, bottomRightTile.X, bottomRightTile.Y, pointLight);
            DrawVertices(vertices);
        }

        private static void MoveVertex(ref VertexPositionColor point, PointLight pointLight)
        {
            if (point.Position.Z <= 0)
                return;
            Vector2 dis = new Vector2(point.Position.X, point.Position.Y) - pointLight.position;
            Vector2 offset = dis / MathF.Sqrt(dis.X * dis.X + dis.Y * dis.Y) * pointLight.radius;
            point.Position += new Vector3(offset, 0);
        }

        public static void AddQuad(List<VertexPositionColor> vertices, Vector2 xy1, Vector2 xy2, PointLight pointLight)
        {
            //For the shadow color I want to take the inverse of the pointlight color and then lerp it towards black a bit       
            VertexPositionColor tl1 = new VertexPositionColor(new Vector3(xy1, 0), _shadowColor);
            VertexPositionColor tr1 = new VertexPositionColor(new Vector3(xy1, 1), _shadowColor);
            VertexPositionColor br1 = new VertexPositionColor(new Vector3(xy2, 0), _shadowColor);

            VertexPositionColor tl2 = new VertexPositionColor(new Vector3(xy1, 1), _shadowColor);
            VertexPositionColor tr2 = new VertexPositionColor(new Vector3(xy2, 0), _shadowColor);
            VertexPositionColor br2 = new VertexPositionColor(new Vector3(xy2, 1), _shadowColor);

            MoveVertex(ref tl1, pointLight);
            MoveVertex(ref tr1, pointLight);
            MoveVertex(ref br1, pointLight);
            MoveVertex(ref tl2, pointLight);
            MoveVertex(ref tr2, pointLight);
            MoveVertex(ref br2, pointLight);

            //0, 1, 2
            vertices.Add(tl1);
            vertices.Add(tr1);
            vertices.Add(br1);

            //0, 1, 3
            vertices.Add(tl2);
            vertices.Add(tr2);
            vertices.Add(br2);
        }
        public static VertexPositionColor[] PrepareTilesForShading(PointLight pointLight)
        {
            Vector2 topLeftOfPointLight = pointLight.position - new Vector2(pointLight.radius);
            Vector2 bottomRightOfPointLight = pointLight.position + new Vector2(pointLight.radius);


            Point topLeftTile = topLeftOfPointLight.ToTileCoordinates();
            Point bottomRightTIle = bottomRightOfPointLight.ToTileCoordinates();
            return PrepareTilesForShading(topLeftTile.X, topLeftTile.Y, bottomRightTIle.X, bottomRightTIle.Y, pointLight);
        }

        public static VertexPositionColor[] PrepareTilesForShading(
            int startTileX, int startTileY,
            int endTileX, int endTileY, PointLight pointLight)
        {
            Vector3 inverseColor = Vector3.One - pointLight.color;
            Color color = Color.Black * 0.25f;
            _shadowColor = color;
            List<VertexPositionColor> vertices = new List<VertexPositionColor>();
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

                    AddQuad(vertices, topLeft, bottomRight, pointLight);
                    AddQuad(vertices, topRight, bottomLeft, pointLight);
                }
            }

            return vertices.ToArray();
        }


        public static void DrawVertices(VertexPositionColor[] vertices)
        {
            if (vertices.Length % 6 != 0 || vertices.Length <= 3)
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
            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, vertices, 0, vertices.Length / 3);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
    }
}
