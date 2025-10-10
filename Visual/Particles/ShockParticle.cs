using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;

namespace Stellamod.Visual.Particles
{
    public class ShockParticle : Particle
    {
        public int FrameWidth = 143;
        public int FrameHeight = 143;
        private float _timer;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);

        }
        public override void Update()
        {
            _timer++;
            Scale *= 1.2f;
            color *= 0.95f;
            fadeIn++;
            if (fadeIn > 60)
                active = false;
        }
    }
}
