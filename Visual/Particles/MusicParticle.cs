using Stellamod.Common.Particles;
using Microsoft.Xna.Framework;
using Terraria;

namespace Stellamod.Visual.Particles
{
    internal class MusicParticle : Particle
    {
        public int FrameWidth = 216;
        public int FrameHeight = 222;
        public int MaxHorizontalFrameCount = 5;
        public int MaxVerticalFrameCount = 12;
        public int FrameCounter = 0;
        public int TicksPerFrame = 2;
        public override void OnSpawn()
        {
            Scale = Main.rand.NextFloat(0.75f, 1.0f) * 0.3f;
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.997f;
    
            FrameCounter++;
            if (FrameCounter >= TicksPerFrame)
            {
                if(Frame.X < FrameWidth * MaxHorizontalFrameCount)
                {
                    Frame.X += FrameWidth;
                  
                }

                if(Frame.X >= Frame.Width * MaxHorizontalFrameCount)
                {
                    Frame.Y += FrameHeight;
                }

                if(Frame.Y >= Frame.Height * MaxVerticalFrameCount)
                {
                    active = false;
                }
                FrameCounter = 0;
            }
        }
    }
}
