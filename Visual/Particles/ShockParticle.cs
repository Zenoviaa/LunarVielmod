using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;

namespace Stellamod.Visual.Particles
{
    public class ShockParticle : LegacyParticle
    {
        public int FrameWidth = 143;
        public int FrameHeight = 143;
        public float alpha;
        private float _timer;
        public override void OnSpawn()
        {
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
            alpha = 1f;
        }
        public override void Update()
        {
            _timer++;
            Scale *= 1.2f;
            color = Color.Lerp(Color.White, Color.Black, fadeIn / 60f);
            color *= alpha;
            fadeIn++;
            if (fadeIn > 60)
                active = false;
        }
    }
}
