using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class RuneLineParticle : Particle<RuneLineParticle>
    {
        public Vector2[] trailCache;
        public Color bloomColor;
        public float time;
        public override void OnSpawn()
        {
            time = 30;
            bloomColor = Color.Green;
        }

        public override void Update()
        {
            fadeIn++;
            if (fadeIn >= time)
            {
                active = false;
            }
        }

        private void DrawRuneLinePrims(GraphicsDevice graphicsDevice)
        {
            //Just in case, I don't know how it'd be null though.
            if (trailCache == null)
                return;
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Lerp(color, bloomColor, ExtraMath.Osc(0f, 1f, speed: 32));
            shader.OuterColor = bloomColor;
            shader.LaserTexture = AssetManager.LaserTextures.Lightning2;
            TrailDrawer.Draw(Main.spriteBatch, trailCache, ColorFunction, WidthFunction, shader, Main.screenPosition);

            shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
            shader.LaserColor = bloomColor * 0.2f;
            shader.InnerColor = Color.Lerp(color, bloomColor, ExtraMath.Osc(0f, 1f, speed: 32)) * 0.2f;
            shader.OuterColor = bloomColor * 0.2f;
            TrailDrawer.Draw(Main.spriteBatch, trailCache, ColorFunction, WidthFunction2, shader, Main.screenPosition);
        }

        public float WidthFunction(float completionRatio)
        {
            float osc = MathF.Sin(completionRatio * 384) * 0.5f + 0.5f;
            return MathHelper.SmoothStep(7, 2, completionRatio) * MathHelper.Lerp(1f, 0f, osc) * EasingFunction.QuadraticBump(fadeIn / time);
        }

        public float WidthFunction2(float completionRatio)
        {
            return WidthFunction(completionRatio) * 2;
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(color, Color.White, ExtraMath.Osc(0f, 1f, speed: 32));
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            //base.Draw(spriteBatch);
            PixelationManager.QueuePrimitivesDrawAction(DrawRuneLinePrims);
        }
    }
}
