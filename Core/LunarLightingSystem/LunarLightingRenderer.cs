using Microsoft.Xna.Framework.Input;
using ReLogic.Threading;
using Stellamod.Common.Shaders;
using Stellamod.Content.Biomes;
using Stellamod.Core.Foggy;
using Stellamod.Core.Rendering;
using System;
using System.Collections.Generic;
using Terraria;
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
        public float ShadowAlpha
        {
            set
            {
                Effect.Parameters["shadowAlpha"].SetValue(value);
            }
        }

        public Vector2 StepSize
        {
            set
            {
                Effect.Parameters["stepSize"].SetValue(value);
            }
        }
    }

    [Autoload(Side = ModSide.Client)]
    public partial class LunarLightingRenderer : ModSystem,
        IPostProcessingPass
    {

        private Vector2[] _offsets;
        private Vector2[] Offsets
        {
            get
            {
                if(_offsets == null)
                {
                    List<Vector2> offsets = new List<Vector2>(16);
                    UnifiedRandom random = new UnifiedRandom(1337);
                    for (int i = 0; i < 16; i++)
                    {
                        offsets.Add(random.NextVector2Circular(16, 16));
                    }
                    _offsets = offsets.ToArray();
                }
                return _offsets;
            }
        }
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
            if (!Lighting.UsingNewLighting)
                return;
            SSAOShader ssaoShader = ShaderContent.GetInstance<SSAOShader>();
            ssaoShader.StepSize = Vector2.One / new Vector2(Main.instance.tileTarget.Width, Main.instance.tileTarget.Height) * 16;


            ssaoShader.Offsets = Offsets;
            SpriteBatch spriteBatch = Main.spriteBatch;
            spriteBatch.Begin(
                SpriteSortMode.Deferred, 
                BlendState.AlphaBlend,
                SamplerState.AnisotropicClamp,
                DepthStencilState.None, 
                RasterizerState.CullNone,
                ssaoShader.Effect,
                Main.GameViewMatrix.TransformationMatrix);
            spriteBatch.Draw(Main.instance.tileTarget, Main.sceneTilePos - Main.screenPosition, Color.White);
            spriteBatch.End();
        }

        private void ApplyLighting(On_FilterManager.orig_EndCapture orig, FilterManager self, RenderTarget2D finalTexture, RenderTarget2D screenTarget1, RenderTarget2D screenTarget2, Color clearColor)
        {
            if (!Main.gameMenu && IsLightingEnabled && Lighting.UsingNewLighting)
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

                sb.Begin(SpriteSortMode.Deferred, blendState: CustomBlendStates.Multiply, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone);
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

        private void RenderToLightMaps(On_Main.orig_CheckMonoliths orig)
        {
            if (Lighting.UsingNewLighting)
            {
                RenderToLightsRT();
                if (IsActive && _isLoaded)
                {
                    if (DrawSunShadows2())
                    {
                        RenderShadows();
                    }
                }
            }


            orig();
        }

        private void DrawShadowsBehindTiles(On_Main.orig_DrawCachedNPCs orig, Main self, List<int> npcCache, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (behindTiles && DrawSunShadows2() && IsActive && _isLoaded && Lighting.UsingNewLighting)
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

        private static bool DrawSunShadows2()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            return config.SunShadows2;
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
