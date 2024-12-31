using Stellamod.Common.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Visual.Particles
{
    internal class PurpleFlowerParticle : Particle
    {
        public int FrameWidth = 252;
        public int FrameHeight = 234;
        public int MaxFrameCount = 1;
        public override void OnSpawn()
        {
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.3f, 1.0f) * 0.1f;
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.99f;

            fadeIn++;
            if (fadeIn > 60)
                active = false;
        }
    }
}
