using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Helpers;
using System;

namespace Stellamod.Particles
{
    internal class ZeeParticle : Particle
    {
        private int frameCount;
        private int frameTick;
        private const int Frame_Count = 5;
        private const int Frame_Duration = 8;
        private int FrameDuration => 30;
        public override void SetDefaults()
        {
            width = 32;
            height = 32;
            Scale = 1f;
            timeLeft = Frame_Count * FrameDuration;

            layer = Layer.BeforeNPCs;
        }
        public override void AI()
        {
            float lifeTime = timeLeft;
            velocity.X = MathF.Sin(lifeTime / 16);
            velocity.Y *= 1.01f;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            Vector2 origin = this.OriginCenter();
            spriteBatch.Draw(texture, screenPos,
                texture.AnimationFrame(ref frameCount, ref frameTick, FrameDuration, Frame_Count, true), Color.White,
                rotation,
               origin,
                1.35f * scale, SpriteEffects.None, 0f);

            return false;
        }
    }
}
