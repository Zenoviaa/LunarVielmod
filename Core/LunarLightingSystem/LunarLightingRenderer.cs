using Microsoft.Xna.Framework.Input;
using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.Foggy;
using Stellamod.Core.Rendering;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Tiles;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using Terraria;
using Terraria.GameContent.Drawing;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.Utilities;

namespace Stellamod.Core.LunarLightingSystem
{
    public class SSAOShader : CrystalShader<SSAOShader>
    {
        public Vector2 StepSize
        {
            set
            {
                Effect.Parameters["stepSize"].SetValue(value);
            }
        }

        public Vector2[] Offsets
        {
            set
            {
                Effect.Parameters["offsets"].SetValue(value);
            }
        }
    }

    public class LuminanceShader : CrystalShader<LuminanceShader>
    {
        private EffectParameter _thresholdParam;
        public float Threshold
        {
            set
            {
                _thresholdParam ??= Effect.Parameters["threshold"];
                _thresholdParam.SetValue(value);
            }
        }

    }

    //TODO: Rewrite this and try implementing Radiance Cascades instead, might be really cool
    //I'll make a prototype elsewhere first though
    public class LightingPreDrawEdit : GlobalTile
    {
        public static bool DontRenderPreDraw;
        public override bool PreDraw(int i, int j, int type, SpriteBatch spriteBatch)
        {
            if (type == TileID.FogMachine && DontRenderPreDraw)
                return false;
            if (type == TileID.FogMachine && NPC.AnyDanger())
                return false;
            return base.PreDraw(i, j, type, spriteBatch);
        }
    }

    public class SunLightShader : CrystalShader<SunLightShader>
    {
        public Vector2 StepSize
        {
            set
            {
                Effect.Parameters["stepSize"].SetValue(value);
            }
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class LunarLightingRenderer : ModSystem,
        IPostProcessingPass
    {
        //We're using 255 lights because that's how many values are in the alpha channel in color
        //We're using the alpha channel to mask which shadows it checks for in the shadow map
        //We could make a custom vertex structure if we need to have more
        public const int MAX_POINT_LIGHTS = 255;
        public const float POINT_LIGHT_DIAMETER = 800;
        public int PostProcessPriority => 15;

        private PointLights _pointLights;
        private ShadowMap _shadowMap;

        private Dictionary<Point, Fog> _fogIndex = new();
        private List<Fog> _fogsToRemove = new();
        public bool renderFog;
        private Color _backLightColor;
        private Vector2 _previousScreenSize;

        private bool _isLoaded;
        private RenderTargetProvider _lightsRT = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);
        private RenderTargetProvider _tileRenderTarget = new RenderTargetProvider(RenderTargetParameters.DefaultScreenTargetCreationFunc);

        private RenderTarget2D _tileBlurRT;
        private RenderTarget2D _tileSunShadowRT;
   

        private List<ILightEmitter> _emitters;
        private List<IBackLightModifier> _backLightModifiers;

        public Color SmoothedBackLightColor;
        public Color BackLightColor;
        public Color SunColor;
        public Vector3 AmbientLight;
        public bool leviathanDarken;

        public bool IsLightingEnabled => ModContent.GetInstance<LunarVeilClientConfig>().BeamingLights;
        public override void Load()
        {
            _pointLights = new PointLights(MAX_POINT_LIGHTS);
            _shadowMap = new ShadowMap(MAX_POINT_LIGHTS, 64);
            _backLightModifiers = new List<IBackLightModifier>();
            _emitters = new List<ILightEmitter>();

            On_FilterManager.EndCapture += ApplyLighting;
            On_Main.CheckMonoliths += RenderToLightMaps;
            On_Main.DrawCachedNPCs += DrawShadowsBehindTiles;
            On_Main.DoDraw_Tiles_Solid += ApplySSAO;
        }

        private void ApplySSAO(On_Main.orig_DoDraw_Tiles_Solid orig, Main self)
        {
            orig(self);
            ApplySSAO();
        }

        private void ApplySSAO()
        {
            SSAOShader ssaoShader = ShaderContent.GetInstance<SSAOShader>();
            ssaoShader.StepSize = Vector2.One / new Vector2(Main.instance.tileTarget.Width, Main.instance.tileTarget.Height) * 16;

            List<Vector2> offsets = new List<Vector2>(16);
            UnifiedRandom random = new UnifiedRandom(1337);
            for (int i = 0; i < 16; i++)
            {
                offsets.Add(random.NextVector2Circular(16, 16));
            }

            ssaoShader.Offsets = offsets.ToArray();
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone,
                ssaoShader.Effect,
                Main.GameViewMatrix.TransformationMatrix);

           
            
            spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);

            spriteBatch.End();
        }

        private void RenderSolidTileMask()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            TileDrawing tilesRenderer = Main.instance.TilesRenderer;


            graphicsDevice.SetRenderTarget(_tileBlurRT);
            graphicsDevice.Clear(Color.Transparent);
            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend);

            Main.screenPosition += new Vector2(Main.offScreenRange);
            LightingPreDrawEdit.DontRenderPreDraw = true;
            tilesRenderer.Draw(true, true, true);
            spriteBatch.End();
            LightingPreDrawEdit.DontRenderPreDraw = false;
            Main.screenPosition -= new Vector2(Main.offScreenRange);

            graphicsDevice.SetRenderTarget(_tileRenderTarget);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone, null);

            Vector2 drawPosition = Vector2.Zero;
            spriteBatch.Draw(_tileBlurRT, drawPosition, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }


        public Color GetSunColor()
        {
            Color[] sunColors = new Color[]
            {
                new Color(8, 79, 126).Towards(Color.White, 0.5f),
              Color.SkyBlue,
      
                new Color(255, 173, 63),
                   new Color(255, 173, 63),
                            new Color(255, 173, 63),
                                     new Color(255, 173, 63),
                                        new Color(255, 173, 63),

              

                Color.White,
               Color.White,
                    Color.White,
                         Color.White,
                              Color.White,
                Color.White,
               Color.White,
                    Color.White,
                         Color.White,
                              Color.White,

                new Color(255, 173, 63),
               new Color(255, 173, 63),
                        new Color(255, 173, 63),
                                 new Color(255, 173, 63),
                                    new Color(255, 173, 63),
                 Color.SkyBlue,
                new Color(8, 79, 126).Towards(Color.White, 0.5f),
            };

            float dayProgress = Main.dayTime ? (float)Main.time / (float)Main.dayLength : (float)Main.time / (float)Main.nightLength;
            Color interpolatedColor = DrawUtilities.InterpolateColorArray(dayProgress, sunColors);
            if (!Main.dayTime)
                interpolatedColor = sunColors[0];
            if (!Main.LocalPlayer.ZoneOverworldHeight && !Main.LocalPlayer.ZoneSkyHeight)
                interpolatedColor = SmoothedBackLightColor;
            if (ModContent.GetInstance<DomainExpansionManager>().hoveringPlatform)
                interpolatedColor = Color.White;
            return interpolatedColor;
        }
        private void RenderSunLight()
        {
            Vector2 stepSize = Vector2.One / new Vector2(Main.screenWidth, Main.screenHeight);
            stepSize *= 4 * -SunLightManager.ShadowDirection;

            var shader = ShaderContent.GetInstance<SunLightShader>();
            shader.StepSize = stepSize;

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp,
                DepthStencilState.None, RasterizerState.CullNone, shader.Effect, Main.GameViewMatrix.TransformationMatrix);


            Vector2 drawPosition = Vector2.Zero;
            spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, null, 
               SunColor, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);

            spriteBatch.End();
        }

        private void ApplyLighting(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (!Main.gameMenu && IsLightingEnabled)
            {
                var glowMaskBloomShader = ShaderContent.GetInstance<LuminanceShader>();
                // glowMaskBloomShader.Threshold = 0.5f;
                glowMaskBloomShader.Threshold = 1F;


                //First we're going to draw the screen target to the lights RT while calculating which colors are bright so we don't kill glow masks and whatnot
                GraphicsDevice gDevice = Main.graphics.GraphicsDevice;
                SpriteBatch sb = Main.spriteBatch;
                gDevice.SetRenderTarget(_lightsRT);
                sb.Begin(SpriteSortMode.Deferred, CustomBlendStates.Brightest, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    glowMaskBloomShader.Effect);
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                sb.End();


                //Take the screen target again and multiple the final light RT over it, to apply the lighting
                gDevice.SetRenderTarget(Main.screenTargetSwap);
                gDevice.Clear(Color.White);

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                    null);
                sb.Draw(Main.screenTarget, Vector2.Zero, Color.White);
                sb.End();

                sb.Begin(SpriteSortMode.Immediate, blendState: CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                sb.Draw(_lightsRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1f, SpriteEffects.None, 0);
                sb.End();

                //Put it back and let the rest of post processing take over
                gDevice.SetRenderTarget(Main.screenTarget);
                gDevice.Clear(Color.Transparent);

                sb.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
                sb.Draw(Main.screenTargetSwap, Vector2.Zero, Color.White);
                sb.End();

            }
            orig(self, finalTexture, screenTarget1, screenTarget2, clearColor);
        }


        public override void Unload()
        {
            base.Unload();
            Main.QueueMainThreadAction(UnloadRenderTargets);
            On_FilterManager.EndCapture -= ApplyLighting;
            On_Main.CheckMonoliths -= RenderToLightMaps;
            On_Main.DrawCachedNPCs -= DrawShadowsBehindTiles;
        }

        private static bool IsActive
        {
            get
            {
                DomainExpansionManager domainExpansionManager = ModContent.GetInstance<DomainExpansionManager>();
                return !domainExpansionManager.inSpace;
            }
        }

        #region Light Render Loop
        private void RenderToLightsRT()
        {
            if (Keyboard.GetState().IsKeyDown(Keys.K))
            {
                Main.time += 128;
            }
            if (Main.gameMenu)
                return;

            //     RenderTileLight();
            if (!IsLightingEnabled)
                return;

            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            int resolution = 64;
            switch (config.ShadowQuality)
            {
                case ShadowQuality.Ultra_Low:
                    resolution = 16;
                    break;
                default:
                case ShadowQuality.Low:
                    resolution = 32;
                    break;
                case ShadowQuality.Medium:
                    resolution = 64;
                    break;
                case ShadowQuality.High:
                    resolution = 128;
                    break;
                case ShadowQuality.Very_High:
                    resolution = 256;
                    break;
            }
            if (_shadowMap.Resolution != resolution)
            {
                _shadowMap.Dispose();
                _shadowMap = new ShadowMap(MAX_POINT_LIGHTS, resolution);
            }
            _shadowMap.Clear();
            _pointLights.Clear();
            _pointLights.GatherLights();

            FastParallel.For(0, _pointLights.UsedLightCount, delegate (int start, int end, object context)
            {
                for (int j = start; j < end; j++)
                {
                    Light light = _pointLights[j];

                    //For now all lights will have the same radius
                    //I think we need a custom vertex structure to have difference radiuses
                    _shadowMap.RayMarch(j, light.position, light.diameter);
                }
            });

            //      FastParallel.For(0, _pointLights.UsedLightCount, (int start, int end, ))
            //_shadowMap.RayMarch(0, Main.LocalPlayer.Center, PointLightSize);
            //RenderSolidTileMask();
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_lightsRT);
            graphicsDevice.Clear(_backLightColor);

            //Render Sun
            RenderSunLight();

            //SunLightManager.RenderSunLight();
            _emitters.Clear();
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.ModProjectile is ILightEmitter emitter)
                {
                    _emitters.Add(emitter);
                }
            }

            if (_emitters.Count > 0)
            {
                SpriteBatch spriteBatch = Main.spriteBatch;
                //Draw additional lights
                foreach (ILightEmitter emitter in _emitters)
                {
                    emitter.RenderLight(spriteBatch);
                }
            }

            VertexPositionColorTexture[] vertices = new VertexPositionColorTexture[_pointLights.UsedLightCount * 4];

            //Prepare the index buffer, we need to draw all the lights in the same batch
            int[] indices = new int[_pointLights.UsedLightCount * 6];
            int connectIndex = 0;
            for (int i = 0; i < indices.Length; i += 6)
            {
                indices[i] = connectIndex + 0;
                indices[i + 1] = connectIndex + 2;
                indices[i + 2] = connectIndex + 3;
                indices[i + 3] = connectIndex + 0;
                indices[i + 4] = connectIndex + 1;
                indices[i + 5] = connectIndex + 3;
                connectIndex += 4;
            }


            for (int i = 0; i < _pointLights.UsedLightCount; i++)
            {
                Light light = _pointLights[i];
                float r = light.diameter;
                r /= 2;
                Vector2 topLeftOffset = new Vector2(-r, -r);
                Vector2 bottomLeftOffset = new Vector2(-r, r);
                Vector2 topRightOffset = new Vector2(r, -r);
                Vector2 bottomRightOffset = new Vector2(r, r);

                Vector2 center = light.position;
                Vector2 topLeft = center + topLeftOffset;
                Vector2 bottomLeft = center + bottomLeftOffset;
                Vector2 topRight = center + topRightOffset;
                Vector2 bottomRight = center + bottomRightOffset;

                //Rotate around the center pivot
                int startIndex = i * 4;
                Color lightColor = light.color;
                vertices[startIndex + 0] = new VertexPositionColorTexture(new Vector3(topLeft, 0), lightColor, new Vector2(0, 0));
                vertices[startIndex + 1] = new VertexPositionColorTexture(new Vector3(topRight, 0), lightColor, new Vector2(1, 0));
                vertices[startIndex + 2] = new VertexPositionColorTexture(new Vector3(bottomLeft, 0), lightColor, new Vector2(0, 1));
                vertices[startIndex + 3] = new VertexPositionColorTexture(new Vector3(bottomRight, 0), lightColor, new Vector2(1, 1));
            }

            if (vertices.Length <= 0 || indices.Length <= 0)
                return;

            //Get the shadow map texture
            _shadowMap.Output();

            //We have to use a blend state that takes the brightest color otherwies shadows would be able to blend over other
            //Lights
            //Actually not sure if we need that with this specific implementation


            var shadow2 = LightingShader.Instance;
            shadow2.ShadowMap = _shadowMap.Texture;
            shadow2.TransformMatrix = TrailDrawer.WorldViewPoint2;


            //Using the max color state gives a really nice look on colors
            //Additive seems to just lerp towards white which looks kinda bland
            graphicsDevice.BlendState = CustomBlendStates.Brightest;
            graphicsDevice.RasterizerState = RasterizerState.CullNone;


            int primitiveCount = vertices.Length / 2;
            shadow2.ApplyPasses();
            graphicsDevice.RasterizerState = RasterizerState.CullNone;
            graphicsDevice.DrawUserIndexedPrimitives(
                PrimitiveType.TriangleList, vertices, 0, vertices.Length, indices, 0, primitiveCount);


        }

        #endregion
        private void RenderToLightMaps(On_Main.orig_CheckMonoliths orig)
        {
            RenderToLightsRT();
            if (IsActive && _isLoaded)
            {
                if (DrawSunShadows2())
                {
                    RenderShadows();
                }
            }

            orig();
        }

        private void DrawShadowsBehindTiles(On_Main.orig_DrawCachedNPCs orig, Main self, List<int> npcCache, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (behindTiles && DrawSunShadows2() && IsActive && _isLoaded)
            {      
                spriteBatch.Draw(_tileSunShadowRT, Vector2.Zero, Color.White);
            }

            orig(self, npcCache, behindTiles);
        }

        public override void OnModLoad()
        {
            base.OnModLoad();
            PostProcessingRenderer.AddPass(this);
        }

        public override void ClearWorld()
        {
            base.ClearWorld();
            ClearLightingData();
        }

        private void ClearLightingData()
        {
            _emitters.Clear();
            _backLightModifiers.Clear();
        }

        private void DrawToScreen()
        {
            if (!ShouldRender())
                return;
            if (!_isLoaded)
                return;


            RenderFog();
        }

        public override void PostUpdateWorld()
        {
            base.PostUpdateWorld();
            UpdateFog();
        }

        public Fog SetupFog(Point position, Action<Fog> createFogFunc)
        {
            if (_fogIndex.ContainsKey(position))
                return _fogIndex[position];
            else
            {
                Fog fog = new Fog();
                fog.tilePosition = position;
                fog.position = new Vector2(position.X * 16, position.Y * 16);
                createFogFunc?.Invoke(fog);
                _fogIndex.Add(position, fog);
                return fog;
            }
        }

        private void UpdateFog()
        {
            foreach (var kvp in _fogIndex)
            {
                Fog fog = kvp.Value;
                fog.Update();
                float dist = Vector2.Distance(fog.position, Main.LocalPlayer.position);
                if (dist > 2000)
                {
                    _fogsToRemove.Add(fog);
                }
            }

            for (int i = 0; i < _fogsToRemove.Count; i++)
            {
                Fog fog = _fogsToRemove[i];
                _fogIndex.Remove(fog.tilePosition);
            }
            _fogsToRemove.Clear();
        }

        private void RenderFog()
        {
            DomainExpansionManager domainExpansionManager = ModContent.GetInstance<DomainExpansionManager>();
            if (domainExpansionManager.inSpace)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (_fogIndex.Count <= 0)
                return;

            var texture = TextureRegistry.Clouds6;
            //Apply Fog Shader
            var fogShader = FogShader.Instance;
            fogShader.FogTexture = texture;
            fogShader.ProgressPower = 0.75f;
            fogShader.EdgePower = 1f;
            fogShader.Speed = 1f;
            fogShader.Apply();
            var currentTexture = texture;
            var blendState = BlendState.AlphaBlend;
            BaseShader currentShader = fogShader;



            spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                currentShader.Effect, Main.GameViewMatrix.TransformationMatrix);

            foreach (var kvp in _fogIndex)
            {
                var fog = kvp.Value;
                if (config.FocusMode && fog.disableWithFocus)
                    continue;

                BaseShader newShader = null;
                if (fog.shaderFunc != null)
                {
                    newShader = fog.shaderFunc();
                }

                if (blendState != fog.blendState || newShader != currentShader)
                {
                    currentTexture = fog.texture;
                    currentShader = newShader;
                    blendState = fog.blendState;

                    Effect effect = null;
                    if (currentShader != null)
                        effect = currentShader.Effect;
                    spriteBatch.End();
                    spriteBatch.Begin(SpriteSortMode.Immediate, blendState, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer,
                        effect, Main.GameViewMatrix.TransformationMatrix);
                }

                Vector2 center = fog.position - Main.screenPosition;
                Vector2 scale = Vector2.One * fog.scale;
                Vector2 origin = fog.texture.Size() / 2;
                spriteBatch.Draw(currentTexture.Value, center, null, fog.color, fog.rotation, origin, scale, SpriteEffects.None, 0f);
            }

            spriteBatch.End();

        }


        private static bool DrawSunShadows2()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            return config.SunShadows2;
        }


        private void RenderShadows()
        {
            if (Main.gameMenu)
                return;

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;

            graphicsDevice.SetRenderTarget(_tileBlurRT);
            graphicsDevice.Clear(Color.Transparent);

            Effect effect = GameShaders.Misc["LunarVeil:SunShadow"].Shader;
            effect.Parameters["mipBias"].SetValue(0.1f);

            Vector2 sunDirection = SunLightManager.ShadowDirection.SafeNormalize(Vector2.Zero);
            effect.Parameters["sunDirection"].SetValue(-sunDirection * 1400);
            effect.Parameters["falloff"].SetValue(0.1f);
            effect.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, effect);
            spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, null, Color.Black * 0.9f, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();


            graphicsDevice.SetRenderTarget(_tileSunShadowRT);
            graphicsDevice.Clear(Color.Transparent);
            Effect blurEffect = GameShaders.Misc["LunarVeil:SunBlur"].Shader;
            blurEffect.Parameters["mipBias"].SetValue(12);
            blurEffect.Parameters["uScreenResolution"].SetValue(Main.ScreenSize.ToVector2());
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, RasterizerState.CullNone, blurEffect);
            spriteBatch.Draw(_tileBlurRT, Vector2.Zero, null, Color.White, 0f, Vector2.Zero, 1, SpriteEffects.None, 0f);
            spriteBatch.End();
        }

        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
            leviathanDarken = false;
        }

        public override void PostUpdateTime()
        {
            base.PostUpdateTime();
            AmbientLight = Color.White.ToVector3();
            BackLightColor = Color.Black;
            if (Main.LocalPlayer.ZoneUnderworldHeight)
            {
                BackLightColor = Color.White * 0.8f;
            }
            if ((Main.LocalPlayer.ZoneSnow ||
                Main.LocalPlayer.GetModPlayer<BiomePlayer>().ZoneMoonspiralTower)
                && !Main.dayTime && Main.LocalPlayer.ZoneOverworldHeight)
            {
                Color color1 = Color.Lerp(Color.LightGreen, Color.LightPink, ExtraMath.Osc(0f, 1f, speed: 0.4f));
                Color color2 = Color.Lerp(Color.Cyan, color1, ExtraMath.Osc(0f, 1f, offset: 1, speed: 0.4f));
                Color finalColor = Color.Lerp(Color.White, color2, ExtraMath.Osc(0f, 1f, offset: 2, speed: 0.4f) * 0.5f);
                BackLightColor = finalColor * 0.8f;
            }

            BiomePlayer biomePlayer = Main.LocalPlayer.GetModPlayer<BiomePlayer>();
            MyPlayer myPlayer = Main.LocalPlayer.GetModPlayer<MyPlayer>();
            foreach (var backLightModifier in _backLightModifiers)
            {
                backLightModifier.ModifyBackLight(ref BackLightColor);
            }

            _backLightColor = Color.Lerp(_backLightColor, BackLightColor, 0.1f);
            SmoothedBackLightColor = _backLightColor;
            SunColor = Color.Lerp(SunColor, GetSunColor(), 0.1f);
        }

        public void AddBackLight(IBackLightModifier backLightModifier)
        {
            _backLightModifiers.Add(backLightModifier);
        }

        public void RemoveBackLight(IBackLightModifier backLightModifier)
        {
            _backLightModifiers.Remove(backLightModifier);
        }

        public override void PostUpdateEverything()
        {
            ResizeRenderTarget(false);
        }

        private static bool ShouldRender()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            if (!config.BeamingLights)
                return false;
            if (Main.gameMenu)
                return false;
            if (!IsActive)
                return false;
            return true;
        }


        private void UnloadRenderTargets()
        {
            _tileBlurRT?.Dispose();
            _tileSunShadowRT?.Dispose();

            _tileBlurRT = null;
            _tileSunShadowRT = null;
            _isLoaded = false;
        }

        private void ResizeRenderTargets()
        {
            if (_tileBlurRT != null && !_tileBlurRT.IsDisposed)
                _tileBlurRT.Dispose();
            if (_tileSunShadowRT != null && !_tileSunShadowRT.IsDisposed)
                _tileSunShadowRT.Dispose();

            _tileSunShadowRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);
            _tileBlurRT = new RenderTarget2D(Main.graphics.GraphicsDevice, Main.screenWidth, Main.screenHeight);      
            _isLoaded = true;
        }

        private void ResizeRenderTarget(bool load)
        {
            if (Main.gameMenu)
                return;
            if (Main.netMode == NetmodeID.Server)
                return;
            Vector2 currentScreenSize = new(Main.screenWidth, Main.screenHeight);
            if (currentScreenSize == _previousScreenSize)
                return;
            Main.QueueMainThreadAction(ResizeRenderTargets);
            _previousScreenSize = currentScreenSize;
        }

        public void RenderToScreen()
        {
            DrawToScreen();
        }
    }
}
