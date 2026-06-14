using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class WindDebrisParticle : Particle<WindDebrisParticle>
    {
        private float _streakWidth;
        private float _streakHeight;
        private float _streakTimer;
        private float _streakRotation;
        public float dir;
        public float time;

        public const int FrameWidth = 16;
        public const int FrameHeight = 16;
        public override void OnSpawn()
        {
            _streakTimer = 0f;
            _streakHeight = Main.rand.NextFloat(16, 32f);
            _streakWidth = Main.rand.NextFloat(16, 80f);
            _streakRotation = Main.rand.NextFloat(-4f, 4f);
            time = Main.rand.NextFloat(30f, 60f);
            dir = Main.rand.NextBool(2) ? 1 : -1;
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(0, 4), FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.35f, 1);
        }

        public override void Update()
        {
            Rotation += 0.05f * dir;
            fadeIn++;
            if (fadeIn >= time)
                active = false;
            _streakTimer += 4;
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            float x = MathF.Sin(_streakTimer * 0.05f) * _streakWidth;
            float y = MathF.Cos(_streakTimer * 0.05f) * _streakHeight;
            Vector2 offset = new Vector2(x, y).RotatedBy(_streakRotation);
            Vector2 drawCenter = DrawPosition + offset;
            spriteBatch.Draw(GetTexture().Value, drawCenter, Frame, Color.White,
                Rotation + _streakRotation, Frame.Size() / 2f, Scale * EasingFunction.QuadraticBump(fadeIn / time), SpriteEffects.None, 0);
        }
    }
    public class WindStormParticle : Particle<WindStormParticle>
    {
        private Vector2 _streakPosition;
        private Vector2 _streakVelocity;

        private float _streakWidth;
        private float _streakHeight;
        private float _streakTimer;
        private float _streakRotation;
        public float dir;
        public Vector2[] trailCache;
        public float time;
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void OnSpawn()
        {
            _streakTimer = 0f;
            _streakVelocity = Velocity;
            _streakPosition = Vector2.Zero;
            _streakHeight = Main.rand.NextFloat(16, 32f);
            _streakWidth = Main.rand.NextFloat(16, 80f);
            _streakRotation = Main.rand.NextFloat(-4f, 4f);
            time = Main.rand.NextFloat(30f, 60f);
            dir = Main.rand.NextBool(2) ? 1 : -1;
            trailCache ??= new Vector2[64];
            for(int i = 0; i < trailCache.Length; i++)
            {
                trailCache[i] = Vector2.Zero;
            }
        }

        public override void Update()
        {
            fadeIn++;
            if (fadeIn >= time)
                active = false;
            for(int j = 0; j < 10; j++)
            {
                _streakTimer++;
                float x = MathF.Sin(_streakTimer * 0.05f) * _streakWidth;
                float y = MathF.Cos(_streakTimer * 0.05f) * _streakHeight;
                Vector2 offset = new Vector2(x, y).RotatedBy(_streakRotation);
                for (int i = trailCache.Length - 1; i > 0; i--)
                {
                    trailCache[i] = trailCache[i - 1];
                }
                if (trailCache.Length > 0)
                    trailCache[0] = offset;
            }
        }
        public Color GetTrailColor(float completionRatio)
        {
            return Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(completionRatio)) * MathHelper.SmoothStep(1f, 0f, fadeIn / time) * 0.5f;
        }

        public float GetTrailWidth(float completionRatio)
        {
            return MathHelper.Lerp(0f, 8, EasingFunction.QuadraticBump(completionRatio));
        }

        private void DrawTrail(GraphicsDevice graphicsDevice)
        {
            var windLineShader = BasicLaserAlphaShader.Instance;
            windLineShader.LaserTexture = TrailRegistry.LightningTrail2;
            TrailDrawer.Draw(Main.spriteBatch, trailCache, GetTrailColor, GetTrailWidth, windLineShader, Center);
        }

        public override void Draw(SpriteBatch spriteBatch)
        {
            base.Draw(spriteBatch);
            PixelationManager.QueuePrimitivesDrawAction(DrawTrail);
        }
    }
}
