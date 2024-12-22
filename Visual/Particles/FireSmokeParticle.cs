using Microsoft.Xna.Framework;
using Stellamod.Common.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    internal class FireSmokeParticle : Particle
    {
        public int FrameWidth = 58;
        public int FrameHeight = 55;
        public int MaxHorizontalFrameCount = 5;
        public int MaxVerticalFrameCount = 19;
        public int FrameCounter = 0;
        public int TicksPerFrame = 4;
        public override void OnSpawn()
        {
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.75f, 1.0f) * 0.9f;
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.96f;
            //  color *= 0.99f;

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
        }
    }
}
