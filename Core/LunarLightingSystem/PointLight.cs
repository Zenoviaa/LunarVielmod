

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Shaders;
using System;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class PointLight
    {
        private bool _needsUpdating;
        private int _index;
        private int _primitiveCount;
        public const int Max_Shadow_Vertex_Count = 12 * 1000;
        public PointLight(Vector2 position, Color color, float intensity, float radius, int maxShadowVertexCount = Max_Shadow_Vertex_Count)
        {
            _needsUpdating = true;
            this.position = position;
            this.color = color;
            this.intensity = intensity;
            this.radius = radius;
            lightVertices = new VertexPositionColorTexture[6];
            shadowVertices = new VertexPositionColor[maxShadowVertexCount];
            shadowColor = Color.Black * 0.3f;
            threshold = 0.9f;
            renderShadows = true;
        }

        public Vector2 position;
        public Color color;
        public float intensity;
        public float radius;
        public int extraRenders;
        public bool faint;
        public Vector2 directionOverride;
        public VertexPositionColorTexture[] lightVertices;
        public VertexPositionColor[] shadowVertices;
        public Color shadowColor;
        public Vector2 lightNormal;
        public float threshold;
        public bool renderShadows;
        public bool globalLight;

        public bool NeedsUpdating()
        {
            return _needsUpdating;
        }
        public void Update()
        {
            CastLight();
            CastShadow();
            _needsUpdating = false;
        }

        public bool IsVisible()
        {
            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            cameraTopLeft -= new Vector2(128);
            cameraBottomRight += new Vector2(128);
            return position.X >= cameraTopLeft.X && position.X <= cameraBottomRight.X && position.Y >= cameraTopLeft.Y && position.Y <= cameraBottomRight.Y;
        }

        private void MoveVertex(ref VertexPositionColor point)
        {
            if (point.Position.Z <= 0)
                return;

            if (directionOverride != Vector2.Zero)
            {
                point.Position += new Vector3(directionOverride, 0);
            }
            else
            {
                Vector2 dis = new Vector2(point.Position.X, point.Position.Y) - position;
                Vector2 offset = dis / MathF.Sqrt(dis.X * dis.X + dis.Y * dis.Y) * radius;
                point.Position += new Vector3(offset, 0);
            }
        }

        public bool IsFull()
        {
            return _index >= shadowVertices.Length;
        }

        public void Clear()
        {
            _index = 0;
        }

        public int AddTriangle()
        {
            int index = _index;
            _index += 3;
            return index;
        }

        private void AddQuad(Vector2 xy1, Vector2 xy2)
        {
            if (IsFull())
            {
                //      Console.WriteLine("Full");
                return;

            }

            //For the shadow color I want to take the inverse of the pointlight color and then lerp it towards black a bit       
            VertexPositionColor tl1 = new VertexPositionColor(new Vector3(xy1, 0), shadowColor);
            VertexPositionColor tr1 = new VertexPositionColor(new Vector3(xy1, 1), shadowColor);
            VertexPositionColor br1 = new VertexPositionColor(new Vector3(xy2, 0), shadowColor);

            VertexPositionColor tl2 = new VertexPositionColor(new Vector3(xy1, 1), shadowColor);
            VertexPositionColor tr2 = new VertexPositionColor(new Vector3(xy2, 0), shadowColor);
            VertexPositionColor br2 = new VertexPositionColor(new Vector3(xy2, 1), shadowColor);

            MoveVertex(ref tl1);
            MoveVertex(ref tr1);
            MoveVertex(ref br1);
            MoveVertex(ref tl2);
            MoveVertex(ref tr2);
            MoveVertex(ref br2);


            int tri1Index = AddTriangle();

            //0, 1, 2
            shadowVertices[tri1Index] = tl1;
            shadowVertices[tri1Index + 1] = tr1;
            shadowVertices[tri1Index + 2] = br1;
            _primitiveCount += 1;

            //0, 1, 3
            int tri2Index = AddTriangle();

            shadowVertices[tri2Index] = tl2;
            shadowVertices[tri2Index + 1] = tr2;
            shadowVertices[tri2Index + 2] = br2;
            _primitiveCount += 1;
        }

        public void CastLight()
        {


            Vector2 topLeft = position + new Vector2(-radius, -radius);
            Vector2 bottomLeft = position + new Vector2(-radius, radius);
            Vector2 bottomRight = position + new Vector2(radius, radius);
            Vector2 topRight = position + new Vector2(radius, -radius);

            lightVertices[0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), color, new Vector2(0, 0));
            lightVertices[1] = new VertexPositionColorTexture(new Vector3(topRight, 0), color, new Vector2(1, 0));
            lightVertices[2] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), color, new Vector2(1, 1));

            lightVertices[3] = new VertexPositionColorTexture(new Vector3(topLeft, 0), color, new Vector2(0, 0));
            lightVertices[4] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), color, new Vector2(0, 1));
            lightVertices[5] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), color, new Vector2(1, 1));
        }

        public void CastShadow()
        {
            Clear();
            _primitiveCount = 0;
            Vector2 topLeftOfPointLight = position - new Vector2(radius);
            Vector2 bottomRightOfPointLight = position + new Vector2(radius);

            Point topLeftTile = topLeftOfPointLight.ToTileCoordinates();
            Point bottomRightTIle = bottomRightOfPointLight.ToTileCoordinates();


            int startTileX = topLeftTile.X;
            int startTileY = topLeftTile.Y;
            int endTileX = bottomRightTIle.X;
            int endTileY = bottomRightTIle.Y;

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
                    Point tilePoint = new Point(x, y);
                    Vector2 worldPoint = tilePoint.ToWorldCoordinates(0, 0);

                    if (lightNormal != Vector2.Zero)
                    {
                        Vector2 lightPosition = position;

                        //Calculate normal
                        Vector2 tileNormal = worldPoint - lightPosition;
                        tileNormal = tileNormal.SafeNormalize(Vector2.Zero);

                        float dot = Vector2.Dot(lightNormal, tileNormal);
                        if (dot < threshold)
                            continue;
                    }

                    //Now we calculate vertices
                    //There's no texture here so it doesn't matter what order we do the triangles in
                    //Pretty sure we start from top left?


                    //Vertex 0
                    Vector2 topLeft = worldPoint;

                    //Vertex 1
                    Vector2 topRight = worldPoint + new Vector2(16, 0);

                    //Vertex 2
                    Vector2 bottomLeft = worldPoint + new Vector2(0, 16);

                    //Vertex 3
                    Vector2 bottomRight = worldPoint + new Vector2(16, 16);

                    AddQuad(topLeft, bottomRight);
                    AddQuad(topRight, bottomLeft);
                }
            }
        }

        public void DrawLight()
        {
            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            if (globalLight)
            {
                Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Circle").Value;
                Vector2 drawOrigin = texture.Size() / 2f;
                SpriteBatch spriteBatch = Main.spriteBatch;
                spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
                for (int i = 0; i < 3; i++)
                    spriteBatch.Draw(texture, Vector2.Zero + new Vector2(Main.screenWidth, Main.screenHeight) / 2f, null, color, 0, drawOrigin, 40, SpriteEffects.None, 0);
                spriteBatch.End();
                return;
            }
            var shader = PointLightShader.Instance;
            shader.Apply();

            foreach (var pass in shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
            }


            BlendState originalBlendState = graphicsDevice.BlendState;
            CullMode oldCullMode = graphicsDevice.RasterizerState.CullMode;
            SamplerState originalSamplerState = graphicsDevice.SamplerStates[0];

            graphicsDevice.RasterizerState.CullMode = CullMode.None;
            graphicsDevice.BlendState = BlendState.Additive;

            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, lightVertices, 0, lightVertices.Length / 3);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }

        public void DrawShadow()
        {
            if (!renderShadows)
                return;
            if (_primitiveCount == 0)
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
              PrimitiveType.TriangleList, shadowVertices, 0, _primitiveCount);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
    }
}
