using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using System;

namespace Stellamod.Visual.Particles
{
    public class SleepParticle : LegacyParticle
    {
        public int FrameWidth = 14;
        public int FrameHeight = 18;
        private float _timer;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);

        }
        public override void Update()
        {
            _timer++;
            Velocity.X = MathF.Sin(_timer * 0.02f);
            Scale *= 0.995f;

            fadeIn++;
            if (fadeIn > 60)
                active = false;
        }
    }
}
