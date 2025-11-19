

using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Threading;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Light;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.LunarLightingSystem
{

    [Autoload(Side = ModSide.Client)]
    public class PointLightManager : ModSystem
    {
        public const int MAX_POINT_LIGHTS = 100;
        public const int POINT_LIGHT_VERTEX_COUNT = 6;
        public const int POINT_LIGHT_MAX_SHADOW_VERTEX_COUNT = 12000;
        public const int POINT_LIGHT_DOWN_SAMPLES = 4;
        public const int POINT_LIGHT_TEXTURE_SIZE = 800;
        public const int MAX_ATLAS_SIZE = 2000;


        //Set up our data for all of our point lights
        //We'll use a data-oriented approach so it's as fast as possible
        public static PointLightData[] PointLights = new PointLightData[MAX_POINT_LIGHTS];
        public static VertexPositionColorTexture[][] PointLightVertices = new VertexPositionColorTexture[MAX_POINT_LIGHTS][];
        public static VertexPositionColor[][] ShadowVertices = new VertexPositionColor[MAX_POINT_LIGHTS][];
        public static int[] ShadowPrimitiveCount = new int[MAX_POINT_LIGHTS];

        public static PointLightState[] LightStates = new PointLightState[MAX_POINT_LIGHTS];
        public static Rectangle[] LightAtlasRectangles = new Rectangle[MAX_POINT_LIGHTS];
        public static Point[] LightPoints = new Point[MAX_POINT_LIGHTS];
        public static bool[,] EmittingTiles;
        public static RasterizerState ScissorRasterizer;
        public static Color ShadowColor;
        public override void OnModLoad()
        {
            base.OnModLoad();
            PrepareArrays();
            ScissorRasterizer = new RasterizerState();
            ScissorRasterizer.ScissorTestEnable = true;
            ShadowColor = Color.Black * 0.3f;
        }
        public override void ClearWorld()
        {
            base.ClearWorld();
            EmittingTiles = new bool[Main.maxTilesX, Main.maxTilesY];
        }
        public override void PostUpdateDusts()
        {
            base.PostUpdateDusts();
            Update();

        }


        public static int AddPointLight(PointLightData pointLightData)
        {
            for(int i = 0; i < MAX_POINT_LIGHTS; i++)
            {
                if (LightStates[i] == PointLightState.INACTIVE)
                {
                    PointLights[i] = pointLightData;
                    LightStates[i] = PointLightState.NEEDS_UPDATING;
                    
                    return i;
                }
            }


            return -1;
        }

        public static void RemovePointLight(int index)
        {
            ref PointLightData data = ref PointLights[index];
            int x = (int)(data.position.X / 16);
            int y = (int)(data.position.Y / 16);

            LightStates[index] = PointLightState.INACTIVE;
            PointLights[index] = default;
            LightPoints[index] = default;
            EmittingTiles[x, y] = false;
        }

        private static void PrepareArrays()
        {
            int x = 0, y = 0;
            int pointLightSize = POINT_LIGHT_TEXTURE_SIZE / POINT_LIGHT_DOWN_SAMPLES;
            int numLightsPer = MAX_ATLAS_SIZE / pointLightSize;

            for(int i = 0; i < MAX_POINT_LIGHTS; i++)
            {
                PointLightVertices[i] = new VertexPositionColorTexture[POINT_LIGHT_VERTEX_COUNT];
                ShadowVertices[i] = new VertexPositionColor[POINT_LIGHT_MAX_SHADOW_VERTEX_COUNT];
                LightAtlasRectangles[i] = new Rectangle(x * pointLightSize, y * pointLightSize, pointLightSize, pointLightSize);
                y++;
                if(y >= numLightsPer)
                {
                    y = 0;
                    x++;
                }
            }

        }

        private static bool IsReallyVisible(Vector2 position)
        {
            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

            const float range = 1000;
            cameraTopLeft -= new Vector2(range);
            cameraBottomRight += new Vector2(range);
            return position.X >= cameraTopLeft.X && position.X <= cameraBottomRight.X && position.Y >= cameraTopLeft.Y && position.Y <= cameraBottomRight.Y;
        }

        private static void FindPointLightSourcesFromTiles()
        {
            Vector2 cameraCenterWorld = Main.Camera.Center;
            Vector2 cameraTopLeft = cameraCenterWorld - new Vector2(Main.screenWidth, Main.screenHeight) / 2;
            Vector2 cameraBottomRight = cameraCenterWorld + new Vector2(Main.screenWidth, Main.screenHeight) / 2;

            const float range = 256;
            cameraTopLeft -= new Vector2(range);
            cameraBottomRight += new Vector2(range);

            Point topLeftTile = cameraTopLeft.ToTileCoordinates();
            Point bottomRightTile = cameraBottomRight.ToTileCoordinates();

            for (int x = topLeftTile.X; x < bottomRightTile.X; x++)
            {
                for (int y = topLeftTile.Y; y < bottomRightTile.Y; y++)
                {
                    if (!WorldGen.InWorld(x, y))
                        continue;
                    Tile tile = Main.tile[x, y];
                    Point lightTilePoint = new Point(x, y);
                    if (LightingSets.PointLitTiles[tile.TileType].A > 0)
                    {

                        if (!EmittingTiles[lightTilePoint.X, lightTilePoint.Y] )
                        {
                            Vector2 position = lightTilePoint.ToWorldCoordinates();
                            Color lightColor = LightingSets.PointLitTiles[tile.TileType];
                            lightColor.A = 1;
                            PointLightData pointLightData = new PointLightData(lightColor, position, 0.5f, 800);
                            int index = AddPointLight(pointLightData);
                            if (index == -1)
                                continue;
                            EmittingTiles[lightTilePoint.X, lightTilePoint.Y] = true;
                            LightPoints[index] = lightTilePoint;
                        }
                    }
                }
            }
        }


        public static Vector3 GetPlayerLightColor()
        {
            Player player = Main.LocalPlayer;
            Item heldItem = player.HeldItem;
            if (LightingSets.EmissiveHeldItems[heldItem.type].A > 0)
            {

                int c = TorchLightingHelper.TorchItemToTorchID(heldItem.type);
                if (c != -1)
                {
                    TorchID.TorchColor(c, out float r, out float g, out float b);
                    Color myColor = new Color(r, g, b);
                    return myColor.ToVector3();
                }


                Vector3 color = LightingSets.EmissiveHeldItems[heldItem.type].ToVector3();
                return color;

            }
            else
            {
                return Vector3.One;
            }
        }

        public static float GetPlayerLightRadius()
        {
            Player player = Main.LocalPlayer;
            Item heldItem = player.HeldItem;
            if (LightingSets.EmissiveHeldItems[heldItem.type].A > 0)
            {
                return 400;
            }
            else
            {
                return 200;
            }
        }

        public static void Update()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.BeamingLights)
                return;
 

            //The last index in the array is reserved for the player light and needs to update every frame
            //So it'll update first, and will always be active, it should never bake, it's a special light
            ref PointLightData playerLightData = ref PointLights[MAX_POINT_LIGHTS - 1];
            playerLightData.position = Main.LocalPlayer.Center;
            playerLightData.color = new Color(GetPlayerLightColor());
            playerLightData.intensity = 1;
            playerLightData.radius = GetPlayerLightRadius();
            LightStates[MAX_POINT_LIGHTS - 1] = PointLightState.CUSTOM;
            ProcessLight(MAX_POINT_LIGHTS - 1);
            //We don't need to check for lights every single frame either
            //It won't be noticeable doing this every few frames instead
            if (Main.GameUpdateCount % 4 != 0)
                return;

            FindPointLightSourcesFromTiles();

            //Since we're using a data oriented structure this is now thread safe! We wouldn't have been able to do that before
            FastParallel.For(0, MAX_POINT_LIGHTS, delegate (int start, int end, object context) {
                for (int j = start; j < end; j++)
                {
                    ProcessLight(j);
                }
            }); 
        }

        public static Matrix CreateLightViewMatrix(Vector2 position, float radius)
        {
            Vector3 screenPosition = new Vector3(Main.screenPosition.X, Main.screenPosition.Y, 0);
            Vector3 lightPosition = new Vector3(position - new Vector2(radius / 2), 0);
            Matrix world = Matrix.CreateTranslation(-lightPosition);
            Matrix view = Main.GameViewMatrix.TransformationMatrix;
            Matrix projection = Matrix.CreateOrthographicOffCenter(0, Main.screenWidth, Main.screenHeight, 0, -1, 1);
            Matrix wvp = world * view * projection;
            return wvp;
        }

        private static void RenderLight(int index, Matrix matrix)
        {
            //First we need to get our data
            ref PointLightData pointLightData = ref PointLights[index];
            ref VertexPositionColorTexture[] lightVertices = ref PointLightVertices[index];
            ref VertexPositionColor[] shadowVertices = ref ShadowVertices[index];
            int primitiveCount = ShadowPrimitiveCount[index];


            GraphicsDevice graphicsDevice = Main.instance.GraphicsDevice;
            graphicsDevice.DepthStencilState = DepthStencilState.None;
            graphicsDevice.RasterizerState.CullMode = CullMode.None;
            graphicsDevice.BlendState = BlendState.Additive;
            graphicsDevice.SamplerStates[0] = SamplerState.PointClamp;

            //Apply the shader now that the graphics device is ready
            //I think the black square issue was just a race condition with the graphics state?
            var shader = PointLightShader.Instance;
            shader.TransformMatrix = matrix;
            foreach (EffectPass pass in shader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                // Set this _after_ Apply, otherwise EffectParameters override it!
                graphicsDevice.Textures[0] = null;
                graphicsDevice.DrawUserPrimitives(
                  PrimitiveType.TriangleList, lightVertices, 0, lightVertices.Length / 3);

            }



            if (primitiveCount <= 0)
                return;

            graphicsDevice.BlendState = BlendState.AlphaBlend;

            var shadowShader = TileShadowShader.Instance;
            shadowShader.TransformMatrix = matrix;
            foreach (EffectPass pass in shadowShader.Effect.CurrentTechnique.Passes)
            {
                pass.Apply();
                // Set this _after_ Apply, otherwise EffectParameters override it!
                graphicsDevice.Textures[0] = null;
                graphicsDevice.DrawUserPrimitives(
                      PrimitiveType.TriangleList, shadowVertices, 0, primitiveCount);

            }

     
        }
        
        private static void RenderLight(int index)
        {
            RenderLight(index, TrailDrawer.WorldViewPoint2);
        }

        private static void BakeLightToRenderTarget(int index)
        {
            ref PointLightData pointLightData = ref PointLights[index];
            RenderLight(index, CreateLightViewMatrix(pointLightData.position, pointLightData.radius));
        }

        public static void RenderLight(int index, RenderTarget2D pointLightRenderTarget, RenderTarget2D accumulatedLightRenderTarget)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            graphicsDevice.SetRenderTarget(pointLightRenderTarget);
            graphicsDevice.Clear(Color.Black);

            RenderLight(index);

            graphicsDevice.SetRenderTarget(accumulatedLightRenderTarget);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, null, Main.Rasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(pointLightRenderTarget, Vector2.Zero, Color.White);
            spriteBatch.End();
        }

        public static void BakeLight(int index, RenderTarget2D pointLightRenderTarget, RenderTarget2D lightMapAtlasRenderTarget)
        {
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;

            graphicsDevice.SetRenderTarget(pointLightRenderTarget);
            graphicsDevice.Clear(Color.Black);
            BakeLightToRenderTarget(index);

            graphicsDevice.SetRenderTarget(lightMapAtlasRenderTarget);
         

            Rectangle destinationRect = LightAtlasRectangles[index];
            Vector2 location = destinationRect.Location.ToVector2();

            Rectangle oldScissor = graphicsDevice.ScissorRectangle;
            graphicsDevice.ScissorRectangle = destinationRect;


            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, null, ScissorRasterizer, null, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(pointLightRenderTarget, location, null, Color.White, 0, Vector2.Zero, 1 / (float)POINT_LIGHT_DOWN_SAMPLES, SpriteEffects.None, 0);
            spriteBatch.End();

          
            spriteBatch.Begin(SpriteSortMode.Immediate, CustomBlendStates.Multiply, SamplerState.PointClamp, null, ScissorRasterizer, PointLightSoftenShader.Instance.Effect, Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(pointLightRenderTarget, destinationRect, null, Color.White);
            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
            graphicsDevice.ScissorRectangle = oldScissor;

            //Set the light to active
            LightStates[index] = PointLightState.ACTIVE;
        }

  
        private static bool IsEmittingLight(int i, int j)
        {
            Tile tile = Main.tile[i, j];
            return tile.HasTile && TileID.Sets.Torch[tile.TileType];
        }

        public static void ProcessLight(int index)
        {
            
            ref PointLightData data = ref PointLights[index];
            ref PointLightState state = ref LightStates[index];

            switch (state)
            {
                case 0:
                    //do nothing
                    break;
                case PointLightState.ACTIVE:
                    Point lightPoint = LightPoints[index];
                    if (!IsReallyVisible(data.position) || !IsEmittingLight(lightPoint.X, lightPoint.Y))
                    {
                        RemovePointLight(index);
                    }

                    break;
                case PointLightState.NEEDS_UPDATING:
            
                    CastLight(index);
                    CastShadow(index);
                    state = PointLightState.NEEDS_BAKING;
                    break;
                case PointLightState.NEEDS_BAKING:
         
                    break;
                case PointLightState.CUSTOM:
                    CastLight(index);
                    CastShadow(index);
                    break;
            }
        }

        /// <summary>
        /// Create the vertices for the light source
        /// </summary>
        /// <param name="index"></param>
        public static void CastLight(int index)
        {
            ref PointLightData data = ref PointLights[index];
            ref VertexPositionColorTexture[] vertices = ref PointLightVertices[index];

            Vector2 position = data.position;
            float radius = data.radius;
            Color color = data.color;

            Vector2 topLeft = position + new Vector2(-radius, -radius);
            Vector2 bottomLeft = position + new Vector2(-radius, radius);
            Vector2 bottomRight = position + new Vector2(radius, radius);
            Vector2 topRight = position + new Vector2(radius, -radius);

            vertices[0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), color, new Vector2(0, 0));
            vertices[1] = new VertexPositionColorTexture(new Vector3(topRight, 0), color, new Vector2(1, 0));
            vertices[2] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), color, new Vector2(1, 1));

            vertices[3] = new VertexPositionColorTexture(new Vector3(topLeft, 0), color, new Vector2(0, 0));
            vertices[4] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), color, new Vector2(0, 1));
            vertices[5] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), color, new Vector2(1, 1));
        }


        /// <summary>
        /// Moves a vertex in relation to a point light for shadowing
        /// </summary>
        /// <param name="point"></param>
        private static void MoveVertex(ref VertexPositionColor point, Vector2 position, float radius)
        {
            Vector2 dis = new Vector2(point.Position.X, point.Position.Y) - position;
            Vector2 offset = dis / MathF.Sqrt(dis.X * dis.X + dis.Y * dis.Y) * radius;
            point.Position += new Vector3(offset, 0);
        }

        private static void AddQuad(Vector2 xy1, Vector2 xy2, Vector2 position, float radius, ref int primIndex, ref int index)
        {
            if (primIndex >= POINT_LIGHT_MAX_SHADOW_VERTEX_COUNT)
                return;
            ref VertexPositionColor[] vertices = ref ShadowVertices[index];
 

                //For the shadow color I want to take the inverse of the pointlight color and then lerp it towards black a bit       
                
            VertexPositionColor tl1 = new VertexPositionColor(new Vector3(xy1, 0), ShadowColor);
            VertexPositionColor tr1 = new VertexPositionColor(new Vector3(xy1, 1), ShadowColor);
            VertexPositionColor br1 = new VertexPositionColor(new Vector3(xy2, 0), ShadowColor);

            VertexPositionColor tl2 = new VertexPositionColor(new Vector3(xy1, 1), ShadowColor);
            VertexPositionColor tr2 = new VertexPositionColor(new Vector3(xy2, 0), ShadowColor);
            VertexPositionColor br2 = new VertexPositionColor(new Vector3(xy2, 1), ShadowColor);

            //MoveVertex(ref tl1);
            MoveVertex(ref tr1, position, radius);
            //MoveVertex(ref br1);
            MoveVertex(ref tl2, position, radius);
            //MoveVertex(ref tr2);
            MoveVertex(ref br2, position, radius);


            int tri1Index = primIndex;
            primIndex += 3;

            //0, 1, 2
            vertices[tri1Index] = tl1;
            vertices[tri1Index + 1] = tr1;
            vertices[tri1Index + 2] = br1;

            //0, 1, 3
            int tri2Index = primIndex;
            primIndex += 3;

            vertices[tri2Index] = tl2;
            vertices[tri2Index + 1] = tr2;
            vertices[tri2Index + 2] = br2;
        }

        public static void CastShadow(int index)
        {
            ref PointLightData data = ref PointLights[index];
            Vector2 position = data.position;
            float radius = data.radius;
            int primIndex = 0;

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

                    //TODO: directional lights should be somewhere else
                    /*
                    if (lightNormal != Vector2.Zero)
                    {
                        Vector2 lightPosition = position;

                        //Calculate normal
                        Vector2 tileNormal = worldPoint - lightPosition;
                        tileNormal = tileNormal.SafeNormalize(Vector2.Zero);

                        float dot = Vector2.Dot(lightNormal, tileNormal);
                        if (dot < threshold)
                            continue;
                    }*/

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

                    AddQuad(topLeft, bottomRight, position, radius, ref primIndex, ref index);
                    AddQuad(topRight, bottomLeft, position, radius, ref primIndex, ref index);
                }
            }

            //Calculate the number of primitives in this shadow
            ShadowPrimitiveCount[index] = primIndex / 3;
        }
    }
}
