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
    internal class ExampleParticle : Particle
    {
        public int FrameWidth = 270;
        public int FrameHeight = 249;
        public int MaxFrameCount = 5;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, FrameHeight * Main.rand.Next(MaxFrameCount), FrameWidth, FrameHeight);
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Rotation += 0.01f;
            Scale *= 0.997f;
            color *= 0.99f;

            fadeIn++;
            if (fadeIn > 60)
                active = false;
        }
    }
}
