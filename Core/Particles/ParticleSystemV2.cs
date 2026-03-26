using Stellamod.Common.Shaders;
using Stellamod.Core.Pixelation;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.Particles
{
    /// <summary>
    /// Manages drawing all particles to the screen
    /// </summary>
    [Autoload(Side = ModSide.Client)]
    public class ParticleSystemV2 : ModSystem
    {
        private GameTime _lastUpdateUiGameTime;
        public static int MaxParticleCount => 500;

        private static List<BaseParticle> AdditiveParticles = new(MaxParticleCount);
        private static List<BaseParticle> AlphaBlendedParticles = new(MaxParticleCount);
        private static List<BaseParticle> UIParticles = new(MaxParticleCount);
        public override void Load()
        {
            base.Load();
            On_Main.DrawDust += DrawMainParticles;
        }

        public override void Unload()
        {
            On_Main.DrawDust -= DrawMainParticles;
            AdditiveParticles.Clear();
            AdditiveParticles = null;

            AlphaBlendedParticles.Clear();
            AlphaBlendedParticles = null;

            UIParticles.Clear();
            UIParticles = null;
        }


        public override void PostUpdateDusts()
        {
            UpdateParticle();
        }

        private static void UpdateParticleList(IEnumerable<BaseParticle> particles)
        {
            foreach(BaseParticle particle in particles)
            {
                if (particle == null)
                    continue;
                if (!particle.active)
                    continue;

                particle.Update();
                particle.Center += particle.Velocity;
                if (particle.parent != null)
                {
                    Vector2 parentMovement = particle.parent.position - particle.parent.oldPosition;
                    particle.Center += parentMovement;
                    particle.hasParent = true;
                }

                bool shouldKill = particle.hasParent && !particle.parent.active;
                shouldKill |= particle.Scale < 0.001f;
                shouldKill |= particle.fadeIn > 1000;
                if (shouldKill)
                {
                    particle.active = false;
                }
            }

        }
        private bool _drawBehind;
        public static void UpdateParticle()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            UpdateParticleList(AdditiveParticles);
            UpdateParticleList(AlphaBlendedParticles);
            UpdateParticleList(UIParticles);

            AdditiveParticles.RemoveAll(p => p == null || !p.active);
            AlphaBlendedParticles.RemoveAll(p => p == null || !p.active);
            UIParticles.RemoveAll(p => p == null || !p.active);
        }

        private void DrawMainParticles(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            PixelationManager.QueueSpritebatchDrawAction(DrawAlphaPixelParticlesBehind, DrawLayer.BehindNPCsWithOutline);
            PixelationManager.QueueSpritebatchDrawAction(DrawAlphaPixelParticles, DrawLayer.OverNPCsWithOutline);
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelParticles, DrawLayer.OverNPCsAdditive);
        }

        private void RenderAlphaParticles(SpriteBatch spriteBatch)
        {
            BaseShader myCustomShader = null;
            for (int i = 0; i < AlphaBlendedParticles.Count; i++)
            {
                var particle = AlphaBlendedParticles[i];
                if (particle == null || !particle.active)
                    continue;
                if (particle.behindLayer && !_drawBehind)
                    continue;

                if (!ParticleUtils.OnScreen(particle.Center - Main.screenPosition))
                    continue;

                if (particle.customShader != myCustomShader)
                {
                    spriteBatch.End();
                    myCustomShader = particle.customShader;
                    if (myCustomShader == null)
                        spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                            myCustomShader.Effect, Main.GameViewMatrix.TransformationMatrix);
                    }
                }
                particle.Draw(spriteBatch);
            }
        }
        private void DrawAlphaPixelParticlesBehind(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            _drawBehind = true;
            RenderAlphaParticles(spriteBatch);
            _drawBehind = false;
        }

        private void DrawAlphaPixelParticles(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            RenderAlphaParticles(spriteBatch);
        }

        private void DrawPixelParticles(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            RenderAdditiveParticles(spriteBatch);
        }

        private void RenderAdditiveParticles(SpriteBatch spriteBatch)
        {
            BaseShader myCustomShader = null;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);

            for (int i = 0; i < AdditiveParticles.Count; i++)
            {
                var particle = AdditiveParticles[i];
                if (particle == null || !particle.active)
                    continue;

                if (!ParticleUtils.OnScreen(particle.Center - Main.screenPosition))
                    continue;

                if (particle.customShader != myCustomShader || spriteBatch.GraphicsDevice.BlendState != BlendState.Additive)
                {
                    spriteBatch.End();
                    myCustomShader = particle.customShader;
                    if (myCustomShader == null)
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, default, null, Main.GameViewMatrix.TransformationMatrix);
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                            myCustomShader.Effect, Main.GameViewMatrix.TransformationMatrix);
                    }
                }

                particle.Draw(spriteBatch);
            }
        }

        public override void UpdateUI(GameTime gameTime)
        {
            base.UpdateUI(gameTime);
            _lastUpdateUiGameTime = gameTime;
        }

        public override void ModifyInterfaceLayers(List<GameInterfaceLayer> layers)
        {
            int mouseTextIndex = layers.FindIndex(layer => layer.Name.Equals("Vanilla: Mouse Text"));
            if (mouseTextIndex != -1)
            {
                layers.Insert(mouseTextIndex, new LegacyGameInterfaceLayer(
                    "Scarlet Sun:UI Particles",
                    delegate
                    {
                        if (_lastUpdateUiGameTime != null && UIParticles.Count > 0)
                        {
                            SpriteBatch spriteBatch = Main.spriteBatch;
                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
                            BaseShader myCustomShader = null;
                            for (int i = 0; i < UIParticles.Count; i++)
                            {
                                var particle = UIParticles[i];
                                if (particle == null || !particle.active)
                                    continue;

                                if (particle.customShader != myCustomShader)
                                {
                                    spriteBatch.End();
                                    myCustomShader = particle.customShader;
                                    if (myCustomShader == null)
                                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, RasterizerState.CullCounterClockwise, null, Main.UIScaleMatrix);
                                    else
                                    {
                                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullCounterClockwise,
                                            myCustomShader.Effect, Main.UIScaleMatrix);
                                    }
                                }

                                particle.Draw(spriteBatch);
                            }

                            spriteBatch.End();
                            spriteBatch.Begin(SpriteSortMode.Deferred, BlendState.AlphaBlend, SamplerState.PointClamp, DepthStencilState.None, Main.Rasterizer, null, Main.UIScaleMatrix);
                        }
                        return true;
                    },
                    InterfaceScaleType.UI));
            }
        }

        public static void AddParticle<T>(T p) where T : BaseParticle
        {
            AdditiveParticles.Add(p);
        }

        public static void AddAlphaBlendedParticle<T>(T p) where T : BaseParticle
        {
            AlphaBlendedParticles.Add(p);
        }

        public static void AddUIParticle<T>(T p) where T : BaseParticle, new()
        {
            UIParticles.Add(p);
        }
    }
}
