
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core.Particles
{
    /// <summary>
    /// Manages drawing all particles to the screen
    /// </summary>
    public class ParticleSystemV2 : ModSystem
    {
        public static int MaxParticleCount => 500;
        public static List<BaseParticle> AdditiveParticles = new(MaxParticleCount);
        public static List<BaseParticle> AlphaBlendedParticles = new(MaxParticleCount);
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
        }


        public override void PostUpdateDusts()
        {
            UpdateParticle();
        }

        public static void UpdateParticle()
        {
            if (Main.netMode == NetmodeID.Server)
                return;

            for (int i = 0; i < AdditiveParticles.Count; i++)
            {
                BaseParticle particle = AdditiveParticles[i];

                if (particle == null)
                    continue;

                particle.Update();
                particle.Center += particle.Velocity;

                if (particle.shouldKilledOutScreen && !ParticleUtils.OnScreen(particle.Center - Main.screenPosition))
                    particle.active = false;

                if (particle.Scale < 0.001f)
                    particle.active = false;

                if (particle.fadeIn > 1000)
                    particle.active = false;
            }


            for (int i = 0; i < AlphaBlendedParticles.Count; i++)
            {
                BaseParticle particle = AlphaBlendedParticles[i];

                if (particle == null)
                    continue;

                particle.Update();
                particle.Center += particle.Velocity;

                if (particle.shouldKilledOutScreen && !ParticleUtils.OnScreen(particle.Center - Main.screenPosition))
                    particle.active = false;

                if (particle.Scale < 0.001f)
                    particle.active = false;

                if (particle.fadeIn > 1000)
                    particle.active = false;
            }

            AdditiveParticles.RemoveAll(p => p == null || !p.active);
            AlphaBlendedParticles.RemoveAll(p => p == null || !p.active);
        }

        private void DrawMainParticles(On_Main.orig_DrawDust orig, Main self)
        {
            orig(self);
            PixelationManager.QueueSpritebatchDrawAction(DrawAlphaPixelParticles, DrawLayer.OverNPCsWithOutline);
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelParticles, DrawLayer.OverNPCsAdditive);
        }

        public void DrawAlphaBlendedParticles(SpriteBatch spriteBatch)
        {
            BaseShader myCustomShader = null;
            for (int i = 0; i < AlphaBlendedParticles.Count; i++)
            {
                var particle = AlphaBlendedParticles[i];
                if (particle == null || !particle.active)
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

        public void DrawAlphaPixelParticles(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            DrawAlphaBlendedParticles(spriteBatch);
        }

        public void DrawPixelParticles(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            DrawParticles(spriteBatch);
        }

        public void DrawParticles(SpriteBatch spriteBatch)
        {
            BaseShader myCustomShader = null;
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
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointWrap, default, default, default, Main.GameViewMatrix.TransformationMatrix);
                    else
                    {
                        spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, SamplerState.PointClamp, DepthStencilState.None, RasterizerState.CullNone,
                            myCustomShader.Effect, Main.GameViewMatrix.TransformationMatrix);
                    }
                }
                particle.Draw(spriteBatch);
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
    }
}
