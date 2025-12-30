

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Shaders;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{
    public class SunLightManager : ModSystem
    {
        private static int _primitiveIndex;
        private static float _overSunTimer;
        private static float _daylightFadeTimer;
        public const int SUN_LIGHT_MAX_SHADOW_VERTEX_COUNT = 240000;
        public static VertexPositionColor[] ShadowVertices = new VertexPositionColor[SUN_LIGHT_MAX_SHADOW_VERTEX_COUNT];
        public static Vector2 ShadowDirection;
        public static Color ShadowColor;
        public static Color SunColor;
        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            Update();
        }

        public static void Update()
        {
            Point point = Main.LocalPlayer.position.ToTileCoordinates();
            bool overworld = (double)point.Y <= Main.worldSurface;
            if (!overworld)
            {
                _overSunTimer--;
                if (_overSunTimer <= 0)
                    return;
            }
            else
            {
                _overSunTimer++;
            }


            _overSunTimer = MathHelper.Clamp(_overSunTimer, 0, 120);
            float interpolant = _overSunTimer / 120f;
            Vector2 sunLeft = Main.Camera.Center + new Vector2(-Main.screenWidth / 2, -Main.screenHeight / 2);
            Vector2 sunRight = Main.Camera.Center + new Vector2(Main.screenWidth / 2, -Main.screenHeight / 2);

            float dayProgress = Main.dayTime ? (float)Main.time / (float)Main.dayLength : (float)Main.time / (float)Main.nightLength;
            float radians = MathHelper.Lerp(MathHelper.ToRadians(-45), MathHelper.ToRadians(45), dayProgress);
            Vector2 sunDirection = Vector2.UnitY.RotatedBy(radians) * 500;
            if (dayProgress <= 0.1f || dayProgress >= 0.9f)
            {

                _daylightFadeTimer--;
            }
            else
            {
                _daylightFadeTimer++;
            }

            _daylightFadeTimer = MathHelper.Clamp(_daylightFadeTimer, 0, 120);
            float shadowDaylightFadeInterpolant = _daylightFadeTimer / 120f;

            Vector2 sunPosition = Main.Camera.Center + new Vector2(0, 0);
            SunColor = Main.ColorOfTheSkies * interpolant;
            ShadowDirection = sunDirection;
            ShadowColor = Color.Black * 0.05f * shadowDaylightFadeInterpolant;

            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.SunShadows)
                return;

            CastShadow();
        }

        public static void RenderSunLight()
        {
            Texture2D texture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Circle").Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            for (int i = 0; i < 3; i++)
                spriteBatch.Draw(texture, Vector2.Zero + new Vector2(Main.screenWidth, Main.screenHeight) / 2f, null, SunColor, 0, drawOrigin, 40, SpriteEffects.None, 0);
            spriteBatch.End();


            //shadows
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.SunShadows)
                return;
            int primitiveCount = _primitiveIndex / 3;
            if (primitiveCount <= 0)
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
              PrimitiveType.TriangleList, ShadowVertices, 0, primitiveCount);

            graphicsDevice.RasterizerState.CullMode = oldCullMode;
            graphicsDevice.BlendState = originalBlendState;
            graphicsDevice.SamplerStates[0] = originalSamplerState;
        }
        private static bool IsFull()
        {
            return _primitiveIndex >= SUN_LIGHT_MAX_SHADOW_VERTEX_COUNT;
        }

        private static void MoveVertex(ref VertexPositionColor point)
        {
            point.Position += new Vector3(ShadowDirection, 0);
        }

        private static void AddQuad(Vector2 xy1, Vector2 xy2)
        {
            if (IsFull())
            {
                return;

            }

            //For the shadow color I want to take the inverse of the pointlight color and then lerp it towards black a bit       
            VertexPositionColor tl1 = new VertexPositionColor(new Vector3(xy1, 0), ShadowColor);
            VertexPositionColor tr1 = new VertexPositionColor(new Vector3(xy1, 1), ShadowColor);
            VertexPositionColor br1 = new VertexPositionColor(new Vector3(xy2, 0), ShadowColor);

            VertexPositionColor tl2 = new VertexPositionColor(new Vector3(xy1, 1), ShadowColor);
            VertexPositionColor tr2 = new VertexPositionColor(new Vector3(xy2, 0), ShadowColor);
            VertexPositionColor br2 = new VertexPositionColor(new Vector3(xy2, 1), ShadowColor);

            //MoveVertex(ref tl1);
            MoveVertex(ref tr1);
            // MoveVertex(ref br1);
            MoveVertex(ref tl2);
            //MoveVertex(ref tr2);
            MoveVertex(ref br2);


            int tri1Index = _primitiveIndex;
            _primitiveIndex += 3;
            //0, 1, 2
            ShadowVertices[tri1Index] = tl1;
            ShadowVertices[tri1Index + 1] = tr1;
            ShadowVertices[tri1Index + 2] = br1;

            //0, 1, 3
            int tri2Index = _primitiveIndex;

            ShadowVertices[tri2Index] = tl2;
            ShadowVertices[tri2Index + 1] = tr2;
            ShadowVertices[tri2Index + 2] = br2;
            _primitiveIndex += 3;
        }

        private static void CastShadow()
        {

            _primitiveIndex = 0;
            Vector2 position = Main.Camera.Center;
            float radius = 4000;
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

                    if (LightingSets.NoShadows[tile.TileType])
                        continue;
                    Point tilePoint = new Point(x, y);
                    Vector2 worldPoint = tilePoint.ToWorldCoordinates(0, 0);

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
    }
}
