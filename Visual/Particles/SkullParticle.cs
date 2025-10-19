using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class SkullParticle : Particle
    {
        public int FrameWidth = 128;
        public int FrameHeight = 128;
        public int MaxFrameCount = 1;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            Scale = 1;
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Scale *= 0.97f;
            color *= 0.99f;

            fadeIn++;
            if (fadeIn > 60)
                active = false;
        }
    }
}
