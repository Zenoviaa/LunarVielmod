
using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;

namespace Stellamod.Visual.Particles
{
    public class CircleStepParticle : Particle
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 5;
        public int FrameCounter = 0;
        public int TicksPerFrame = 3;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
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
                Frame.Y += FrameHeight;
                if (Frame.Y >= FrameHeight * MaxFrameCount)
                {
                    Frame.Y = 0;
                    active = false;
                }
                FrameCounter = 0;
            }
        }
    }
}
