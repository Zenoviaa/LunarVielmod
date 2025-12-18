using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using ReLogic.Threading;
using Stellamod.Content.Areas.Abyss.BossesAB.VerlianSingularity;
using Stellamod.Core;
using Stellamod.Core.MoonWaters;
using Stellamod.Core.RenderTargetSystem;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class BlackSeaPlatform
    {
        private readonly Asset<Texture2D> _platformTextureAsset;
        public BlackSeaPlatform(Asset<Texture2D> platformTextureAsset)
        {
            _platformTextureAsset = platformTextureAsset;
            scale = 1f;
            randScale = Main.rand.NextFloat(-0.4f, 0);
        }

        public Vector3 initialPosition;
        public Vector3 rotatedPosition;
        public float rotation;
        public float scale;
        public float randScale;
        public Color color;
        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Texture2D textureToDraw = _platformTextureAsset.Value;
            Vector2 drawOrigin = new Vector2(textureToDraw.Width / 2f, textureToDraw.Height / 2f);
            Vector2 drawPosition = new Vector2(rotatedPosition.X, rotatedPosition.Y);
            drawPosition += Main.Camera.Center;
            drawPosition -= screenPos;

            float drawScale = scale + randScale;
            spriteBatch.Draw(textureToDraw, drawPosition, null, color, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }

    public class PlatformZLayerComparer : IComparer<BlackSeaPlatform>
    {
        public int Compare(BlackSeaPlatform x, BlackSeaPlatform y)
        {
            return x.rotatedPosition.Y.CompareTo(y.rotatedPosition.Y);
        }
    }

    /// <summary>
    /// Manages the little particles that orbit around the singularity, lots of little particles
    /// </summary>
    public class LittleStarParticleManager
    {
        private float _timer;
        private readonly VertexPositionColor[] _particleVertexBufferArr;
        private readonly Vector2[] _particleOldPos;
        private readonly Vector2[] _trailWidths;
        private readonly FastNoiseLite _fastNoise;
        private readonly float[] _noiseValues;
        public LittleStarParticleManager(int particleCount, int trailLength)
        {
            _fastNoise = new FastNoiseLite();
            ParticleCount = particleCount;
            TrailLength = trailLength;

            //Calculate the number of vertices that we'll need to draw the tornado
            //This should be equal to the particle count times the trail length times the nubmer of vertices per point
            int verticesPerPosition = 6;
            int vertexCount = particleCount * trailLength * verticesPerPosition;
            _particleVertexBufferArr = new VertexPositionColor[vertexCount];
            _particleOldPos = new Vector2[particleCount * trailLength];

            //We can pre calculate the uv floats since it's always the same
            //We increase the trail length by 1 here because in the trailing functionwe need to get the next point, this last position is basically just a duplicate
            _trailWidths = new Vector2[trailLength + 1];
            for (int i = 0; i < _trailWidths.Length; i++)
            {
                float ratio = (float)i / (float)trailLength;
                _trailWidths[i] = GetTrailWidth(ratio) * Vector2.One;
            }


            //calculate noise values of each particle
            _noiseValues = new float[particleCount];
            for (int n = 0; n < _noiseValues.Length; n++)
            {
                _noiseValues[n] = _fastNoise.GetNoise(n, n) * 0.5f + 0.5f;
                _noiseValues[n] *= 0.5f;
            }
        }

        public readonly int ParticleCount;
        public readonly int TrailLength;
        public float xOvalRadius;
        public float yOvalRadius;


        /// <summary>
        /// Calculate the position of the particle at specific a timestep
        /// </summary>
        /// <param name="time"></param>
        /// <param name="index"></param>
        /// <returns></returns>
        public Vector2 CalculateParticlePosition(float time, int index)
        {
            const float revolutionTime = 100f;

            //Calculate the rotation offset for this particle
            const float maxRadiansOffset = MathHelper.TwoPi;

            float particleRatio = (float)index / (float)TrailLength;
            float particleRadiansOffset = particleRatio * maxRadiansOffset;
            float timeRadians = time / revolutionTime * MathHelper.TwoPi;
            float rotationRadians = particleRadiansOffset + timeRadians;


            //Calculate the initial position of the particle

            float off = index * 0.1f;
            float x = 200f;
            if (index > ParticleCount / 2)
            {
                x *= 3;
            }
            float xRadius = x + ExtraMath.Osc(-500, 500, 0, off);
            float yRadius = ExtraMath.Osc(-150f, 0f, 1, off) + ExtraMath.Osc(-500f, 500f, 0f, offset: off);
            Vector3 initialPosition = new Vector3(xRadius, yRadius / 2f, yRadius);

            //Create the rotation matrix and Rotate the particle
            Matrix rotationMatrix = Matrix.CreateFromAxisAngle(new Vector3(1, 1, 0.25f), rotationRadians);
            Vector3 rotatedPosition = Vector3.Transform(initialPosition, rotationMatrix);
            Vector2 flatPosition = new Vector2(rotatedPosition.X, rotatedPosition.Y);
            return flatPosition;
        }


        public Vector2 GetRotation(int particleIndex, int index)
        {
            Vector2 prev;
            Vector2 next;

            /*
            Vector2 prev = CalculateParticlePosition(time - 1, index);
            Vector2 next = CalculateParticlePosition(time + 1, index);
            */



            if (index > 0 && index < TrailLength - 1)
            {
                //Read from the old pos array
                int oldPosIndex = particleIndex * TrailLength + index;
                next = _particleOldPos[oldPosIndex];
                prev = _particleOldPos[oldPosIndex + 1];
                return Vector2.Normalize(next - prev).RotatedBy(MathHelper.Pi / 2);
            }
            else
            {
                return Vector2.One;
            }

        }

        private void SimulateParticles()
        {
            float numPoints = TrailLength;
            int numVerticesPerParticle = TrailLength * 6;

            _fastNoise.SetFrequency(2);
            //Shift our position array backward
            FastParallel.For(0, ParticleCount, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    for (int j = TrailLength - 1; j > 0; j--)
                    {
                        int oldPosIndex = i * TrailLength + j;
                        _particleOldPos[oldPosIndex] = _particleOldPos[oldPosIndex - 1];
                    }
                }
            });

            //Simulate all of our particles
            FastParallel.For(0, ParticleCount, delegate (int start, int end, object context)
            {
                for (int i = start; i < end; i++)
                {
                    //Fast noise returns a value between -1 and 1, so we're normalizing it to 0-1 for the lerp function
                    float noiseColorInterpolant = _noiseValues[i];

                    //Width multiplier for the trail
                    float widthMultiplier = 1f;
                    if (noiseColorInterpolant > 0.4f)
                        widthMultiplier *= 8;
                    Color black = Color.Black;
                    for (int j = 0; j < TrailLength; j++)
                    {
                        //Substract to get the previous frames of the particle
                        float timeStep = _timer - j;

                        //Now we have the position of the particle at this specific time step
                        Vector2 currentPosition;
                        Vector2 prevPosition;

                        if (j > 0 && j < TrailLength - 1)
                        {
                            //Read from the old pos array
                            int oldPosIndex = i * TrailLength + j;
                            currentPosition = _particleOldPos[oldPosIndex];
                            prevPosition = _particleOldPos[oldPosIndex + 1];
                        }
                        else
                        {
                            currentPosition = CalculateParticlePosition(timeStep, i);
                            prevPosition = CalculateParticlePosition(timeStep - 1, i);
                            _particleOldPos[i * TrailLength] = currentPosition;
                        }

                        //Calculate the widths
                        Vector2 width = _trailWidths[j] * widthMultiplier;
                        Vector2 width2 = _trailWidths[j + 1] * widthMultiplier;

                        //Calculate the rotation offsets
                        Vector2 off1 = GetRotation(i, j) * width;
                        Vector2 off2 = GetRotation(i, j + 1) * width2;

                        Color col1 = Color.White;
                        Color col2 = Color.White;

                        col1 = Color.Lerp(col1, black, noiseColorInterpolant);
                        col2 = Color.Lerp(col2, black, noiseColorInterpolant);

                        //Apply camera offset
                        currentPosition += Main.Camera.Center;
                        prevPosition += Main.Camera.Center;

                        //Calcualte the index of the vertices
                        int primIndex = i * numVerticesPerParticle + j * 6;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(currentPosition + off1, 0f), col1);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(currentPosition - off1, 0f), col1);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(prevPosition + off2, 0f), col2);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(prevPosition + off2, 0f), col2);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(prevPosition - off2, 0f), col2);

                        primIndex++;
                        _particleVertexBufferArr[primIndex] = new VertexPositionColor(new Vector3(currentPosition - off1, 0f), col1);
                    }
                }
            });
        }

        public void Update()
        {
            _timer++;
            SimulateParticles();
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(0.66f, 0, completionRatio);
        }

        public void Draw()
        {
            var particleShader = TileShadowShader.Instance;
            particleShader.ApplyPasses();

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.DrawUserPrimitives(
              PrimitiveType.TriangleList, _particleVertexBufferArr, 0, _particleVertexBufferArr.Length / 3);

        }
    }
    public class BlackSeaPlatformManager
    {
        private float _timer;
        private float _oscTimer;
        private UnifiedRandom _random;
        private readonly BlackSeaPlatform[] _platforms;
        private readonly BlackSeaPlatform[] _platformsByZLayer;
        private PlatformZLayerComparer _zLayerComparer;
        public BlackSeaPlatformManager()
        {
            _platforms = new BlackSeaPlatform[Platform_Count];

            Asset<Texture2D>[] assets = GetAssets();
            //Initialize all of our platforms
            for (int i = 0; i < _platforms.Length; i++)
            {
                Asset<Texture2D> asset = assets[Main.rand.Next(0, assets.Length)];
                _platforms[i] = new BlackSeaPlatform(asset);
            }

            xOvalRadius = 196;
            yOvalRadius = 64;
        }
        public BlackSeaPlatformManager(int platformCount)
        {
            _platforms = new BlackSeaPlatform[platformCount];
            _platformsByZLayer = new BlackSeaPlatform[platformCount];
            _zLayerComparer = new PlatformZLayerComparer();
            Asset<Texture2D>[] assets = GetAssets();
            //Initialize all of our platforms
            for (int i = 0; i < _platforms.Length; i++)
            {
                Asset<Texture2D> asset = assets[Main.rand.Next(0, assets.Length)];
                _platforms[i] = new BlackSeaPlatform(asset);
                _platformsByZLayer[i] = _platforms[i];
            }

            xOvalRadius = 196;
            yOvalRadius = 64;
        }

        public float xOvalRadius;
        public float yOvalRadius;

        public const int Platform_Count = 100;
        public const string Platform_FileName = "SingularPlatform_";
        private Asset<Texture2D>[] GetAssets()
        {
            int numUniqueAssets = 7;
            Asset<Texture2D>[] platformTextureAssets = new Asset<Texture2D>[numUniqueAssets];
            //ANaing scheme for the textures is just SingularPlatform_[num]
            for (int i = 0; i < numUniqueAssets; i++)
            {
                string rootTexturePath = this.GetType().DirectoryHere() + $"/{Platform_FileName}";
                string platformTexturePath = $"{rootTexturePath}{i}";
                Asset<Texture2D> platformTextureAsset = ModContent.Request<Texture2D>(platformTexturePath);
                platformTextureAssets[i] = platformTextureAsset;
            }
            return platformTextureAssets;

        }

        public void Update()
        {
            _timer++;
            //_timer = Main.Camera.Center.X / 16f;
            if (NPC.AnyNPCs(ModContent.NPCType<E>()))
            {
                SingularityFallSystem fallSystem = ModContent.GetInstance<SingularityFallSystem>();
                fallSystem.noWings = true;
                fallSystem.inSpace = true;
                fallSystem.hoveringPlatform = true;
                fallSystem.hoverPlatformY = 16000;
            }

            const float revolutionTime = 400;

            //Calculate the radians offset
            float radiansToRotate = _timer / revolutionTime * MathHelper.TwoPi;


            yOvalRadius = 400;
            xOvalRadius = 600;
            _random ??= new UnifiedRandom();
            _random.SetSeed(1337);

            for (int i = 0; i < _platforms.Length; i++)
            {
                BlackSeaPlatform platform = _platforms[i];
                //Calculate the initial position of the platform
                //Every platform would have the same initial position

                if (i > _platforms.Length / 2)
                {
                    float off = _random.NextFloat(0f, 10f);
                    float xRadius = 200f + ExtraMath.Osc(-1000, 1000, 0, off);
                    float yRadius = ExtraMath.Osc(-150f, 0f, 1, off) + ExtraMath.Osc(-1000, 1000, 0f, offset: off);
                    Vector3 initialPosition = new Vector3(xRadius, yRadius, yRadius);
                    platform.initialPosition = initialPosition;
                }
                else
                {
                    Vector3 initialPosition = new Vector3(xOvalRadius, -yOvalRadius / 2f, yOvalRadius);
                    platform.initialPosition = initialPosition;
                }


                //Calculate the new rotation of this point
                //We need to offset the radians
                float completionRatio = (float)i / (float)_platforms.Length;
                float maxRadiansOffset = MathHelper.TwoPi;
                float offset = completionRatio * maxRadiansOffset;
                float rotationRadians = offset + radiansToRotate;

                Matrix rotationMatrix = Matrix.CreateFromAxisAngle(new Vector3(0.1F, 1, 0.5f + ExtraMath.Osc(-0.5f, 0.5f, speed: 0.3f, i)), rotationRadians);

                //Calculate the rotated position of the platform
                platform.rotatedPosition = Vector3.Transform(platform.initialPosition, rotationMatrix);

                //Calculate the scale of this platform
                float zPosition = platform.rotatedPosition.Z + xOvalRadius;
                float zScale = 1f - (zPosition / (xOvalRadius * 2f));
                platform.scale = MathHelper.Lerp(0.2f, 1f, zScale);

                float range = MathHelper.ToRadians(5);
                platform.rotation = (new Vector2(platform.rotatedPosition.X, platform.rotatedPosition.Y) - Vector2.Zero).ToRotation();
                platform.color = Color.Lerp(Color.Black, Color.White, EasingFunction.OutExpo(zScale));
            }
        }

        public void Draw(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            if (_platformsByZLayer == null)
                return;

            Array.Sort(_platformsByZLayer, _zLayerComparer);
            for (int i = 0; i < _platformsByZLayer.Length; i++)
            {
                BlackSeaPlatform platform = _platformsByZLayer[i];
                platform.Draw(spriteBatch, screenPos);
            }
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class BlackSeaRenderingEdit : ModSystem
    {
        private BlackSeaPlatformManager _platformManager;
        private LittleStarParticleManager _starParticleManager;
        private ManagedRenderTarget _blackHurricaneRT;
        private ManagedRenderTarget _reflectionGradientRT;
        private ManagedRenderTarget _reflectionRT;
        private ManagedRenderTarget _magicGroundRT;
        public bool drawBlackSea;
        public Vector2? miniOrbDrawPosition;
        public float miniOrbDrawScale;
        public override void Load()
        {
            base.Load();
            On_Main.CheckMonoliths += RenderBlackHurricaneRT;
            On_Main.DrawNPCs += DrawBlackHurricaneRTToScreen;
            On_OverlayManager.Draw += ApplyReflection;

        }


        public override void Unload()
        {
            base.Unload();
            On_Main.CheckMonoliths -= RenderBlackHurricaneRT;
            On_Main.DrawNPCs -= DrawBlackHurricaneRTToScreen;
            On_OverlayManager.Draw -= ApplyReflection;
        }
        public override void OnModLoad()
        {
            base.OnModLoad();
            _blackHurricaneRT = ManagedRenderTarget.New(GetScreenSize);
            _reflectionGradientRT = ManagedRenderTarget.New(GetScreenSize);
            _reflectionRT = ManagedRenderTarget.New(GetScreenSize);
            _magicGroundRT = ManagedRenderTarget.New(GetScreenSize);
            //Create a new platform manager
            _platformManager = new BlackSeaPlatformManager(24);
            _starParticleManager = new LittleStarParticleManager(250, 16);
        }

        public override void OnModUnload()
        {
            base.OnModUnload();
        }

        private void RenderToBlackHurricaneRT()
        {
            if (Main.gameMenu)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_blackHurricaneRT);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin();
            Vector2 drawCenter = Main.Camera.Center;
            drawCenter.Y += ExtraMath.Osc(-2, 2, speed: 8);
            Vector2 screenPos = Main.screenPosition;
            DrawSingularity(drawCenter, screenPos);
            _platformManager.Draw(spriteBatch, screenPos);
            _starParticleManager.Draw();
            DrawHoveringPlatform(spriteBatch);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
        }

        private void RenderToReflectionGradientRT()
        {
            if (Main.gameMenu)
                return;

            SingularityFallSystem singularityFallSystem = ModContent.GetInstance<SingularityFallSystem>();
            //Calculate a gradient texture so we know where the reflection mapping goes
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_reflectionGradientRT);
            graphicsDevice.Clear(Color.Black);

            YGradientShader yGradientShader = YGradientShader.Instance;
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, yGradientShader.Effect);

            Vector2 drawPosition = new Vector2(Main.Camera.Center.X, singularityFallSystem.hoverPlatformY);
            drawPosition -= Main.screenPosition;
            drawPosition.Y += 48;
            drawPosition.X -= _reflectionGradientRT.Width / 2;

            spriteBatch.Draw(_reflectionGradientRT, drawPosition, null, Color.White, 0, Vector2.Zero, new Vector2(1f, 1), SpriteEffects.None, 0f);
            spriteBatch.End();


            graphicsDevice.SetRenderTarget(null);
        }
        private void RenderToReflectionRT()
        {
            if (Main.gameMenu)
                return;

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            graphicsDevice.SetRenderTarget(_reflectionRT);
            graphicsDevice.Clear(Color.Transparent);

            ManagedRenderTarget reflectionRT = ModContent.GetInstance<MoonWaterSystem>().GetReflectionRenderTarget();
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, null);

            spriteBatch.Draw(reflectionRT, Vector2.Zero - new Vector2(Main.offScreenRange), null, Color.White, 0, Vector2.Zero, 2, SpriteEffects.None, 0);

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void RenderToMagicGroundRT()
        {
            if (Main.gameMenu)
                return;

            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            SpriteBatch spriteBatch = Main.spriteBatch;
            graphicsDevice.SetRenderTarget(_magicGroundRT);
            graphicsDevice.Clear(Color.Transparent);


            Effect reflectionCombineEffect = GameShaders.Misc["LunarVeil:SingularReflection"].Shader;
            float mipBias = 1;
            float reflectionDistance = 512;
            Vector2 reflectionTexelSize = (Vector2.One * mipBias) / new Vector2((float)_reflectionRT.Width, (float)_reflectionRT.Height);

            reflectionCombineEffect.Parameters["reflectionDistance"].SetValue(reflectionDistance);
            reflectionCombineEffect.Parameters["reflectionTexelSize"].SetValue(reflectionTexelSize);
            reflectionCombineEffect.Parameters["reflectionPower"].SetValue(4);
            reflectionCombineEffect.Parameters["HeightMapTexture"].SetValue(_reflectionGradientRT);


            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer, reflectionCombineEffect);

            spriteBatch.Draw(_reflectionRT, Vector2.Zero, Color.White);

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void DrawHoveringPlatform(SpriteBatch spriteBatch)
        {
            SingularityFallSystem singularityFallSystem = ModContent.GetInstance<SingularityFallSystem>();
            if (singularityFallSystem.hoveringPlatform)
            {
                Texture2D bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
                Vector2 drawOrigin = new Vector2(bloomLine.Size().X / 2, 0);
                float rotation = MathHelper.PiOver2;
                Color drawColor = Color.White;
                drawColor.A = 0;
                drawColor *= 0.1375f;
                drawColor *= ExtraMath.Osc(0.95f, 1f);
                Vector2 drawPosition = new Vector2(Main.LocalPlayer.Center.X, singularityFallSystem.hoverPlatformY);
                drawPosition -= Main.screenPosition;
                drawPosition.Y += 48;
                Vector2 drawScale = new Vector2(1, 2);
                spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
                spriteBatch.Draw(bloomLine, drawPosition, null, drawColor, -rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            }
        }
        private void RenderBlackHurricaneRT(On_Main.orig_CheckMonoliths orig)
        {
            RenderToBlackHurricaneRT();
            RenderToReflectionRT();
            RenderToReflectionGradientRT();
            RenderToMagicGroundRT();
            orig();
        }

        private void DrawBlackHurricaneRTToScreen(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (!Main.gameMenu)
            {
                if (drawBlackSea)
                {
                    spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                    spriteBatch.End();
                    spriteBatch.Begin();

                    Color drawColor = Color.Lerp(Color.White, Color.Black, 0.35f);
                    spriteBatch.Draw(_blackHurricaneRT, Vector2.Zero, drawColor);
                    spriteBatch.End();


                    spriteBatch.Begin();
                    DrawHoveringPlatform(spriteBatch);
                    drawBlackSea = false;
                }

                if (miniOrbDrawPosition.HasValue)
                {
                    Effect featherEffect = FeatherShader.Instance.Effect;
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, Main.DefaultSamplerState, DepthStencilState.None, Main.Rasterizer,
                        featherEffect);

                    Vector2 positionToDrawOrbAt = miniOrbDrawPosition.Value;
                    Vector2 drawPosition = positionToDrawOrbAt - Main.screenPosition;
                    Texture2D hurricaneTexture = _blackHurricaneRT;
                    Vector2 drawOrigin = hurricaneTexture.Size() / 2f;
                    spriteBatch.Draw(hurricaneTexture, drawPosition, null, Color.White, 0, drawOrigin, miniOrbDrawScale, SpriteEffects.None, 0f);

                    spriteBatch.End();
                    spriteBatch.Begin();
                    miniOrbDrawPosition = null;
                }

            }

            orig(self, behindTiles);
        }


        private void ApplyReflection(On_OverlayManager.orig_Draw orig, OverlayManager self, SpriteBatch spriteBatch, RenderLayers layer, bool beginSpriteBatch)
        {
            if (layer == RenderLayers.ForegroundWater && !Main.gameMenu && NPC.AnyNPCs(ModContent.NPCType<E>()))
            {
                //  spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                spriteBatch.Draw(_magicGroundRT, Vector2.Zero, Color.White * 0.95f);

            }
            orig(self, spriteBatch, layer, beginSpriteBatch);
        }
        public Point GetScreenSize()
        {
            return new Point(Main.screenTarget.Width, Main.screenTarget.Height);
        }


        public override void PostUpdateNPCs()
        {
            base.PostUpdateNPCs();

            DrawHelper.UpdateFrame(ref _incresionDiskFrameBottom, 0.8f, 1, 40);
            DrawHelper.UpdateFrame(ref _incresionDiskFrameTop, 0.8f, 1, 76);
            _spinTimer++;
            _singularityRotation += 0.001f;
            _platformManager?.Update();
            _starParticleManager?.Update();
        }

        private float _incresionDiskFrameBottom;
        private float _incresionDiskFrameTop;
        private float _singularityRotation;
        private float _spinTimer;
        private string _rootTexturePath;
        public void DrawSingularity(Vector2 drawCenter, Vector2 screenPos)
        {
            Vector2 drawPosition = drawCenter - screenPos;
            _rootTexturePath = this.GetType().DirectoryHere() + "/BlackSingularity";
            Texture2D celestialRing = ModContent.Request<Texture2D>(_rootTexturePath + "_CelestialRing").Value;
            Vector2 ringDrawOrigin = celestialRing.Size() / 2f;
            Color ringDrawColor = Color.White;

            SpriteBatch spriteBatch = Main.spriteBatch;
            ringDrawColor *= 0.05f;
            ringDrawColor.A = 0;
            spriteBatch.Draw(celestialRing, drawPosition, null, ringDrawColor, _singularityRotation, ringDrawOrigin, 4, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(_rootTexturePath).Value;

            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = Vector2.One * 3;

            float spinRotOffset = _spinTimer * -0.01f;
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Main.DiscoColor;
            sparkyShader.Distortion = -0.15f;
            sparkyShader.Time = -Main.GlobalTimeWrappedHourly * 40;
            sparkyShader.Tiling = Vector2.One * 2;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: sparkyShader.Effect);


            var lightTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 lightDrawOrigin = lightTexture.Size() / 2f;

            float sparkyRot = _singularityRotation + spinRotOffset;
            float scaleOsc2 = ExtraMath.Osc(0.4f, 0.5f, speed: 1);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.White * 0.75f, sparkyRot, lightDrawOrigin, drawScale * 3 * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.Draw(lightTexture, drawPosition, null, Color.White * 0.25f, sparkyRot + 0.2f, lightDrawOrigin, drawScale * 8 * scaleOsc2, SpriteEffects.None, 0);


            var shader = SingularityShader.Instance;
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawPosition, null, Color.White, _singularityRotation, drawOrigin, drawScale * 1.5f * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();

            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Lerp(Color.White, Color.Cyan, 0.15f), ExtraMath.Osc(0f, 1f, speed: 2));
            diskDrawColor.A = 0;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.58f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, _singularityRotation, diskDrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, _singularityRotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc, SpriteEffects.None, 0);

            diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF2").Value;

            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.25f, _singularityRotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(3.5f, 0.2f), SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, _singularityRotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(7.5f, 0.2f), SpriteEffects.None, 0);


            Texture2D extra67 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_67").Value;
            Vector2 extra67DrawOrigin = extra67.Size() / 2f;
            Color extra67DrawColor = Color.Lerp(Color.White, Color.Cyan, ExtraMath.Osc(0f, 1f, speed: 2));
            extra67DrawColor.A = 0;
            spriteBatch.Draw(extra67, drawPosition, null, extra67DrawColor * 0.2f, _singularityRotation, extra67DrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            DrawIncresionDiskBottom(spriteBatch, drawCenter, screenPos, Color.White);
            DrawIncresionDiskTop(spriteBatch, drawCenter, screenPos, Color.White);
        }
        private void DrawIncresionDiskBottom(SpriteBatch spriteBatch, Vector2 drawCenter, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameBottom, columns: 5, frameWidth: 400, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(_rootTexturePath + "_Disk").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.15f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = drawCenter - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            float drawScale = 1.75f;
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, _singularityRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.Cyan;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, _singularityRotation, drawOrigin, drawScale * 1.5f, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.Purple;
            incresionDiskDrawColor *= 0.25f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, _singularityRotation, drawOrigin, drawScale * 2, SpriteEffects.None, 0);
        }


        private void DrawIncresionDiskTop(SpriteBatch spriteBatch, Vector2 drawCenter, Vector2 screenPos, Color drawColor)
        {
            //Draw Incresion Disk
            Rectangle incresionDiskRect = DrawHelper.FrameGrid(_incresionDiskFrameTop, columns: 4, frameWidth: 480, frameHeight: 200);
            Texture2D supernovaTopTexture = ModContent.Request<Texture2D>(_rootTexturePath + "_Top").Value;

            //Incresion Disk Draw Color
            Color incresionDiskDrawColor = Color.White;
            incresionDiskDrawColor *= 0.15f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = drawCenter - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;

            float drawScale = 3f;
            float drawRotation = _singularityRotation;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

    }
    public partial class E : ScarletBoss
    {
        private enum AIState
        {
            Intro_Idle,
            Intro_SwordHold,
            Intro_HeadTurn,
            Intro_HandOut,
            Intro_DomainExpansion,
            Intro_Finish,
            Idle,

            Despawn,

            ForwardSlash_Start,
            ForwardSlash_QuickStart,
            ForwardSlash_RePosition,
            ForwardSlash,
            ForwardSlash_End,

            RippingGeyser_Start,
            RippingGeyser_Dash,
            RippingGeyser_AuraFarm,
            RippingGeyser_End,

            Grab_Start,
            Grab_Walk,
            Grab_Dash,
            Grab_Punish,
            Grab_EatDirt,
            Grab_ThrowSword,
            Grab_End,

            Tornado_Start,
            Tornado_PreSpin,
            Tornado_Spin,
            Tornado_End,

            ScreenSlash_Start,
            ScreenSlash_PreSlash,
            ScreenSlash_Slash,
            ScreenSlash_SwordPoint,
            ScreenSlash_End,

            SwordStarPlosion_Start,
            SwordStarPlosion_Charge,
            SwordStarPlosion_Swing,
            SwordStarPlosion_End,

            BlackDashStart,
            BlackDashPreDash,
            BlackDashDash,
            BlackDashEnd,

            JevilScythes_Start,
            JevilScythes_Loop,
            JevilScythes_End,

            SingularBaseball_Start,
            SingularBaseball_SummonBall,
            SingularBaseball_HitBall,
            SingularBaseball_FindBall,
            SingularBaseball_End
        }

        private bool _intro;
        private bool _showNamePlate;
        private bool _contactDamage;

        private float _attackNumber;
        private float _hoverTimer;
        private ref float Timer => ref NPC.ai[0];
        private AIState State
        {
            get
            {
                return (AIState)NPC.ai[1];
            }
            set
            {
                NPC.ai[1] = (float)value;
            }
        }
        private Vector2 TargetVector
        {
            get
            {
                return new Vector2(NPC.ai[2], NPC.ai[3]);
            }
            set
            {
                NPC.ai[2] = value.X;
                NPC.ai[3] = value.Y;
            }
        }

        private PatternManager<AIState> _patternManagerBackingField;
        private PatternManager<AIState> PatternManager
        {
            get
            {
                if(_patternManagerBackingField == null)
                {
                    _patternManagerBackingField = new PatternManager<AIState>(new Tuple<AIState, float>(AIState.ForwardSlash_Start, 1.0f));
                }
                return _patternManagerBackingField;
            }
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && _contactDamage;
        }


        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }


        //Finally time to make a secret boss, this is going to be fun :)
        //Alright
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[NPC.type] = 34;
            NPCID.Sets.TrailCacheLength[NPC.type] = 16;
            NPCID.Sets.TrailingMode[Type] = 3;
            NPCID.Sets.MPAllowedEnemies[NPC.type] = true;
            NPCID.Sets.BossBestiaryPriority.Add(Type);
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 128;
            NPC.height = 200;
            NPC.damage = 100;
            NPC.defense = 20;
            NPC.lifeMax = 18000;
            NPC.scale = 1f;
            NPC.aiStyle = -1;

            NPC.value = Item.buyPrice(gold: 5);
            NPC.knockBackResist = 0f;
            NPC.boss = true;
            NPC.noGravity = true;
            NPC.noTileCollide = true;
            NPC.npcSlots = 30f;

            Music = MusicLoader.GetMusicSlot(Mod, "Assets/Music/EStyr");
            NPC.HitSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Hit") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
            NPC.DeathSound = new SoundStyle("Stellamod/Assets/Sounds/Gintze_Death") with { PitchVariance = 0.1f, Pitch = -0.5f, Volume = 0.2f };
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_forwardVector);
            writer.Write(_hoverTimer);
            writer.Write(_attackNumber);
        }
        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _forwardVector = reader.ReadVector2();  
            _hoverTimer = reader.ReadSingle();
            _attackNumber = reader.ReadSingle();
        }

        private void EnablePlatformArena()
        {
            SingularityFallSystem fallSystem = ModContent.GetInstance<SingularityFallSystem>();
            fallSystem.noWings = true;
            fallSystem.inSpace = true;
            fallSystem.hoveringPlatform = true;
            fallSystem.hoverPlatformY = 16000;
        }

        private void UpdateClient()
        {
            if (Main.netMode == NetmodeID.Server)
                return;
            if (_intro)
            {
                NPC.boss = true;
                BlackSea blackSea = ScreenShader.GetInstance<BlackSea>();
                blackSea.alpha = 1f;

                BlackSeaRenderingEdit blackseaRenderer = ModContent.GetInstance<BlackSeaRenderingEdit>();
                blackseaRenderer.drawBlackSea = true;
            }
            else
            {
                NPC.boss = false;
                for (int i = 0; i < Main.musicFade.Length; i++)
                {
                    Main.musicFade[i] = 0;
                }
            }
        }

        public override void AI()
        {
            base.AI();

            UpdateClient();
            _contactDamage = false;
            _isGrabbing = false;
            _hoverTimer++;
         

            if(State != AIState.Despawn && !NPC.HasValidTarget)
            {
                NPC.TargetClosest();
                if (!NPC.HasValidTarget)
                {
                    SwitchState(AIState.Despawn);
                }
            }
            _telegraphLineAlpha = 0;
            _telegraphLineRot = 0;
            TargetOutlineColor = Color.White;
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Intro_Idle:
                    AI_IntroIdle();
                    break;
                case AIState.Intro_SwordHold:
                    AI_IntroSwordHold();
                    break;
                case AIState.Intro_HeadTurn:
                    AI_IntroHeadTurn();
                    break;
                case AIState.Intro_HandOut:
                    AI_IntroHandOut();
                    break;
                case AIState.Intro_DomainExpansion:
                    AI_IntroDomainExpansion();
                    break;
                case AIState.Intro_Finish:
                    AI_IntroFinish();
                    break;

                case AIState.Despawn:
                    AI_Despawn();
                    break;

                case AIState.ForwardSlash_Start:
                    AI_ForwardSlashStart();
                    break;
                case AIState.ForwardSlash_QuickStart:
                    AI_ForwardSlashQuickStart();
                    break;
                case AIState.ForwardSlash:
                    AI_ForwardSlash();
                    break;
                case AIState.ForwardSlash_RePosition:
                    AI_ForwardSlashReposition();
                    break;
                case AIState.ForwardSlash_End:
                    AI_ForwardSlashEnd();
                    break;

                case AIState.RippingGeyser_Start:
                    AI_RippingGeysterStart();
                    break;
                case AIState.RippingGeyser_Dash:
                    AI_RippingGeyserDash();
                    break;
                case AIState.RippingGeyser_AuraFarm:
                    AI_RippingGeyserAuraFarm();
                    break;
                case AIState.RippingGeyser_End:
                    AI_RippingGeyserEnd();
                    break;

                case AIState.Grab_Start:
                    AI_GrabStart();
                    break;
                case AIState.Grab_Walk:
                    AI_GrabWalk();
                    break;
                case AIState.Grab_Dash:
                    AI_GrabDash();
                    break;
                case AIState.Grab_Punish:
                    AI_GrabDunk();
                    break;
                case AIState.Grab_EatDirt:
                    AI_GrabEatDirt();
                    break;
                case AIState.Grab_ThrowSword:
                    AI_GrabThrowSword();
                    break;
                case AIState.Grab_End:
                    AI_GrabEnd();
                    break;

                case AIState.Tornado_Start:
                    AI_TornadoStart();
                    break;
                case AIState.Tornado_PreSpin:
                    AI_TornadoPreSpin();
                    break;
                case AIState.Tornado_Spin:
                    AI_TornadoSpin();
                    break;
                case AIState.Tornado_End:
                    AI_TornadoEnd();
                    break;

                case AIState.ScreenSlash_Start:
                    AI_ScreenSlashStart();
                    break;
                case AIState.ScreenSlash_PreSlash:
                    AI_ScreenSlashPreSlash();
                    break;
                case AIState.ScreenSlash_Slash:
                    AI_ScreenSlashSlash();
                    break;
                case AIState.ScreenSlash_SwordPoint:
                    AI_ScreenSlashSwordPoint();
                    break;
                case AIState.ScreenSlash_End:
                    AI_ScreenSlashEnd();
                    break;

                case AIState.SwordStarPlosion_Start:
                    AI_SwordStarPlosionStart();
                    break;
                case AIState.SwordStarPlosion_Charge:
                    AI_SwordStarPlosionCharge();
                    break;
                case AIState.SwordStarPlosion_Swing:
                    AI_SwordStarPlosionSwing();
                    break;
                case AIState.SwordStarPlosion_End:
                    AI_SwordStarPlosion_End();
                    break;

                case AIState.BlackDashStart:
                    AI_BlackDashStart();
                    break;
                case AIState.BlackDashPreDash:
                    AI_BlackDashPreDash();
                    break;
                case AIState.BlackDashDash:
                    AI_BlackDashDash();
                    break;
                case AIState.BlackDashEnd:
                    AI_BlackDashEnd();
                    break;

                case AIState.JevilScythes_Start:
                    AI_JevilScythesStart();
                    break;
                case AIState.JevilScythes_Loop:
                    AI_JevilScythesLoop();
                    break;
                case AIState.JevilScythes_End:
                    AI_JevilScythesEnd();
                    break;

                case AIState.SingularBaseball_Start:
                    AI_SingularBaseballStart();
                    break;
                case AIState.SingularBaseball_SummonBall:
                    AI_SinuglarBaseballSummonBall();
                    break;
                case AIState.SingularBaseball_HitBall:
                    AI_SingularBaseballHitBall();
                    break;
                case AIState.SingularBaseball_FindBall:
                    AI_SingularBaseballFindBall();
                    break;
                case AIState.SingularBaseball_End:
                    AI_SingularBaseballEnd();
                    break;
            }
            _outlineColor = Color.Lerp(_outlineColor, TargetOutlineColor, 0.1f);
            for (int i = OldFrame.Length - 1; i > 0; i--)
            {
                OldFrame[i] = OldFrame[i - 1];
            }
            OldFrame[0] = NPC.frame;
            NPC.spriteDirection = NPC.direction;
        }

        private Vector2 CalculateHoverVelocity()
        {
            Vector2 hoverVelocity = Vector2.Zero;
            hoverVelocity.Y = MathF.Sin(_hoverTimer * 0.025f);
            return hoverVelocity;
        }

        private void AI_Idle()
        {
            _attackNumber = 0;
            Timer++;
            if(Timer >= 15)
            {
                ChooseAttack();
            }
        }

        private void AI_Despawn()
        {
            Timer++;
            if(Timer == 1)
            {
                ScreenShaderSystem screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Black, 1f, 160);
            }
            NPC.velocity.X *= 0.2f;
            NPC.velocity.Y -= 0.2f;
            Invert invert = ScreenShader.GetInstance<Invert>();
            invert.alpha = 1f;
            if(Timer >= 150)
            {
                NPC.active = false;
            }
        }

        private void ChooseAttack()
        {
            SwitchState(AIState.Grab_Start);
        }
    }
}
