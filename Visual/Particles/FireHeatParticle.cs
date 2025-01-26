using Microsoft.Xna.Framework;
using Stellamod.Common.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    internal class FireHeatParticle : Particle
    {
        public int FrameWidth = 80;
        public int FrameHeight = 82;
        public int MaxHorizontalFrameCount = 5;
        public int MaxVerticalFrameCount = 21;
        public int FrameCounter = 0;
        public int TicksPerFrame = 4;
        public override void OnSpawn()
        {
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.75f, 1.0f) * 0.8f;
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.96f;

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
