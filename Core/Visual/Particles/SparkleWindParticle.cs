﻿using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Core.Visual.Particles
{
    internal class SparkleWindParticle : Particle
    {
        public int FrameWidth = 488;
        public int FrameHeight = 422;
        public int MaxHorizontalFrameCount = 5;
        public int MaxVerticalFrameCount = 11;
        public int FrameCounter = 0;
        public int TicksPerFrame = 4;
        public override void OnSpawn()
        {
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = Main.rand.NextFloat(0.04f, 0.2f);
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
            if (fadeIn > 60)
                active = false;
        }
    }
}
