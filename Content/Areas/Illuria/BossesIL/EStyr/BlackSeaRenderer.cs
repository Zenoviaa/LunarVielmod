using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.MoonWaters;
using Stellamod.Core.Shaders;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Graphics.Effects;
using Terraria.Graphics.Shaders;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    [Autoload(Side = ModSide.Client)]
    public class BlackSeaRenderer : ModSystem
    {
        private BlackSeaPlatformManager _platformManager;
        private LittleStarParticleManager _starParticleManager;
        private ManagedRenderTarget _pixelRT;
        private ManagedRenderTarget _blackHurricaneRT;
        private ManagedRenderTarget _reflectionGradientRT;
        private ManagedRenderTarget _reflectionRT;
        private ManagedRenderTarget _magicGroundRT;
        public bool drawBlackSea;
        public bool renderBlackSea;
        public Vector2? miniOrbDrawPosition;
        public float miniOrbDrawScale;
        public float alpha;
        public override void Load()
        {
            base.Load();
            On_Main.CheckMonoliths += RenderBlackHurricaneRT;
            On_Main.DrawNPCs += DrawBlackHurricaneRTToScreen;
            On_OverlayManager.Draw += ApplyReflection;
           On_Main.DrawCachedNPCs += DrawBlackHurricaneRTToScreen;
        }


        public override void Unload()
        {
            base.Unload();
            On_Main.CheckMonoliths -= RenderBlackHurricaneRT;
            On_Main.DrawNPCs -= DrawBlackHurricaneRTToScreen;
            On_OverlayManager.Draw -= ApplyReflection;
            On_Main.DrawCachedNPCs -= DrawBlackHurricaneRTToScreen;
        }

        private void DrawBlackHurricaneRTToScreen(On_Main.orig_DrawCachedNPCs orig, Main self, List<int> npcCache, bool behindTiles)
        {

            orig(self, npcCache, behindTiles);
        }

        public override void OnModLoad()
        {
            base.OnModLoad();
            _pixelRT = ManagedRenderTarget.New(GetScreenSize, 8);
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

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.AnisotropicClamp, DepthStencilState.None, Main.Rasterizer);
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

            DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
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

            spriteBatch.Draw(_reflectionRT, Vector2.Zero, Color.White * alpha);

            spriteBatch.End();
            graphicsDevice.SetRenderTarget(null);
        }

        private void RenderToPixelRT()
        {

            SpriteBatch spriteBatch = Main.spriteBatch;
            GraphicsDevice graphicsDevice = Main.graphics.GraphicsDevice;
            graphicsDevice.SetRenderTarget(_pixelRT);
            graphicsDevice.Clear(Color.Transparent);

            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer);
            spriteBatch.Draw(_blackHurricaneRT, Vector2.Zero, null, Color.White, 0, Vector2.Zero, 1/8f, SpriteEffects.None, 0);
            spriteBatch.End();

            graphicsDevice.SetRenderTarget(null);
        }
        private void DrawHoveringPlatform(SpriteBatch spriteBatch)
        {
            DomainExpansionManager singularityFallSystem = ModContent.GetInstance<DomainExpansionManager>();
            if (singularityFallSystem.hoveringPlatform)
            {
                Texture2D bloomLine = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/BloomLine").Value;
                Vector2 drawOrigin = new Vector2(bloomLine.Size().X / 2, 0);
                float rotation = MathHelper.PiOver2;

                Color drawColor = Color.White;
                drawColor.A = 0;
                drawColor *= 0.1375f;
                drawColor *= ExtraMath.Osc(0.95f, 1f);
                drawColor *= alpha;

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
            if (renderBlackSea)
            {
                RenderToBlackHurricaneRT();
                RenderToReflectionRT();
                RenderToReflectionGradientRT();
                RenderToMagicGroundRT();
                RenderToPixelRT();
            }

            orig();
        }

        private void DrawBlackHurricaneRTToScreen(On_Main.orig_DrawNPCs orig, Main self, bool behindTiles)
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            if (!Main.gameMenu && behindTiles)
            {

                if (drawBlackSea)
                {

                    spriteBatch.GraphicsDevice.Clear(Color.Transparent);
                    spriteBatch.End();
                    spriteBatch.Begin();

                    Color drawColor = Color.Lerp(Color.White, Color.Black, 0.35f);
                    spriteBatch.Draw(_blackHurricaneRT, Vector2.Zero, null, drawColor, 0, Vector2.Zero, 1, SpriteEffects.None, 0);
                    spriteBatch.End();


                    spriteBatch.Begin();


                }
                DrawHoveringPlatform(spriteBatch);
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
            if (layer == RenderLayers.ForegroundWater && !Main.gameMenu && NPC.AnyNPCs(ModContent.NPCType<E>()) && drawBlackSea)
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

        public override void PreUpdateNPCs()
        {
            base.PreUpdateNPCs();
            drawBlackSea = false;
            renderBlackSea = false;
            darkenedSingularity = false;
        }

        public override void PostUpdateNPCs()
        {
            base.PostUpdateNPCs();
            if (renderBlackSea)
            {
                DrawHelper.UpdateFrame(ref _incresionDiskFrameBottom, 0.8f, 1, 40);
                DrawHelper.UpdateFrame(ref _incresionDiskFrameTop, 0.8f, 1, 76);
                _spinTimer++;
                _singularityRotation += 0.001f;

                _platformManager?.Update();
                _starParticleManager?.Update(Main.Camera.Center);

                alpha += 0.02f;
                if (alpha >= 1f)
                    alpha = 1f;
            }
            else if (alpha > 0)
            {
                alpha -= 0.02f;
                if (alpha < 0)
                    alpha = 0;
            }

     
        }

        private float _incresionDiskFrameBottom;
        private float _incresionDiskFrameTop;
        private float _singularityRotation;
        private float _spinTimer;
        private string _rootTexturePath;

        public bool darkenedSingularity;
        public void DrawSingularity(Vector2 drawCenter, Vector2 screenPos)
        {

            Vector2 drawPosition = drawCenter - screenPos;
            _rootTexturePath = this.GetType().DirectoryHere() + "/BlackSingularity";
            Texture2D celestialRing = ModContent.Request<Texture2D>(_rootTexturePath + "_CelestialRing").Value;
            Vector2 ringDrawOrigin = celestialRing.Size() / 2f;
            Color ringDrawColor = Color.DarkGray;

            SpriteBatch spriteBatch = Main.spriteBatch;
            ringDrawColor *= 0.05f;
            ringDrawColor.A = 0;
            if (darkenedSingularity)
                ringDrawColor *= 0.5f;
            spriteBatch.Draw(celestialRing, drawPosition, null, ringDrawColor, _singularityRotation, ringDrawOrigin, 4, SpriteEffects.None, 0);

            Texture2D texture = ModContent.Request<Texture2D>(_rootTexturePath).Value;

            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawScale = Vector2.One * 3;

            float spinRotOffset = _spinTimer * -0.01f;
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.White;
            sparkyShader.OuterColor = Color.Gray;
            sparkyShader.Distortion = -0.15f;
            sparkyShader.Time = -Main.GlobalTimeWrappedHourly * 40;
            sparkyShader.Tiling = Vector2.One * 2;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: sparkyShader.Effect);


            var lightTexture = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 lightDrawOrigin = lightTexture.Size() / 2f;

            float sparkyRot = _singularityRotation + spinRotOffset;
            float scaleOsc2 = ExtraMath.Osc(0.4f, 0.5f, speed: 1);

            Color sparkyColor = Color.White * 0.75f;
            if (darkenedSingularity)
                sparkyColor *= 0.5f;
            spriteBatch.Draw(lightTexture, drawPosition, null, sparkyColor, sparkyRot, lightDrawOrigin, drawScale * 3 * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.Draw(lightTexture, drawPosition, null, sparkyColor * 0.25f, sparkyRot + 0.2f, lightDrawOrigin, drawScale * 8 * scaleOsc2, SpriteEffects.None, 0);


            var shader = SingularityShader.Instance;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.White;
            spriteBatch.Restart(effect: shader.Effect);

            Color singularityColor = Color.White;
            if (darkenedSingularity)
                singularityColor *= 0.5f;
            spriteBatch.Draw(texture, drawPosition, null, singularityColor, _singularityRotation, drawOrigin, drawScale * 1.5f * scaleOsc2, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();

            Texture2D diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SFGrey").Value;
            Vector2 diskDrawOrigin = diskTexture.Size() / 2f;
            Color diskDrawColor = Color.Lerp(Color.White, Color.Gray, ExtraMath.Osc(0f, 1f, speed: 2));
            if (darkenedSingularity)
                diskDrawColor *= 0.5f;
            diskDrawColor.A = 0;

            float scaleOsc = ExtraMath.Osc(0.5f, 0.58f, speed: 1);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, _singularityRotation, diskDrawOrigin, drawScale * 0.8f * scaleOsc, SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor, _singularityRotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc, SpriteEffects.None, 0);

            diskTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/SF2Grey").Value;

            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.25f, _singularityRotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(3.5f, 0.2f), SpriteEffects.None, 0);
            spriteBatch.Draw(diskTexture, drawPosition, null, diskDrawColor * 0.5f, _singularityRotation, diskDrawOrigin, drawScale * 0.7f * scaleOsc * new Vector2(7.5f, 0.2f), SpriteEffects.None, 0);


            Texture2D extra67 = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_67").Value;
            Vector2 extra67DrawOrigin = extra67.Size() / 2f;
            Color extra67DrawColor = Color.Lerp(Color.White, Color.Gray, ExtraMath.Osc(0f, 1f, speed: 2));
            extra67DrawColor.A = 0;
            if (darkenedSingularity)
                extra67DrawColor *= 0.5f;
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
            if (darkenedSingularity)
                incresionDiskDrawColor *= 0.5f;
            incresionDiskDrawColor.A = 0;

            Vector2 drawPos = drawCenter - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;
            float drawScale = 1.75f;
            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, _singularityRotation, drawOrigin, drawScale, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.Gray;
            incresionDiskDrawColor *= 0.25f;
            if (darkenedSingularity)
                incresionDiskDrawColor *= 0.5f;
            incresionDiskDrawColor.A = 0;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, _singularityRotation, drawOrigin, drawScale * 1.5f, SpriteEffects.None, 0);

            incresionDiskDrawColor = Color.DarkGray;
            incresionDiskDrawColor *= 0.25f;
            if (darkenedSingularity)
                incresionDiskDrawColor *= 0.5f;
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
            if (darkenedSingularity)
                incresionDiskDrawColor *= 0.5f;
            Vector2 drawPos = drawCenter - screenPos;
            Vector2 drawOrigin = incresionDiskRect.Size() / 2;

            float drawScale = 3f;
            float drawRotation = _singularityRotation;

            spriteBatch.Draw(supernovaTopTexture, drawPos, incresionDiskRect, incresionDiskDrawColor, drawRotation, drawOrigin, drawScale, SpriteEffects.None, 0);
        }

    }
}
