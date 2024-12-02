using Stellamod.Common.Particles;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Visual.Particles
{
    internal class Strike2Particle : Particle
    {
        public int FrameWidth = 451;
        public int FrameHeight = 451;
        public int MaxHorizontalFrameCount = 5;
        public int MaxVerticalFrameCount = 5;
        public int FrameCounter = 0;
        public int TicksPerFrame = 2;
        public override void OnSpawn()
        {
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale *= 0.2f;
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

            fadeIn++;
            if (fadeIn > 60 || color.A < 10)
                active = false;
        }
    }
}
