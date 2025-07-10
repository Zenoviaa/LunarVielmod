using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Core.Visual.Particles
{
    internal class SpiralBoomParticle : Particle
    {
        public int FrameWidth = 301;
        public int FrameHeight = 276;
        public int MaxHorizontalFrameCount = 1;
        public int MaxVerticalFrameCount = 10;
        public int FrameCounter = 0;
        public int TicksPerFrame = 2;
        public override void OnSpawn()
        {
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale *= Main.rand.NextFloat(0.5f, 0.7f);
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.997f;
            color *= 0.99f;

            FrameCounter++;
            if (FrameCounter >= TicksPerFrame)
            {
                if (Frame.X < FrameWidth * MaxHorizontalFrameCount)
                {
                    Frame.X += FrameWidth;

                }

                if (Frame.X >= Frame.Width * MaxHorizontalFrameCount)
                {
                    Frame.Y += FrameHeight;
                }

                if (Frame.Y >= Frame.Height * MaxVerticalFrameCount)
                {
                    active = false;
                }
                FrameCounter = 0;
            }

            fadeIn++;
            if (fadeIn > 60 || color.A < 10)
                active = false;
        }
    }
}
