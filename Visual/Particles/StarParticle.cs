using Microsoft.Xna.Framework;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Terraria;

namespace Stellamod.Visual.Particles
{
    public class StarParticle : LegacyParticle
    {
        public int FrameWidth = 64;
        public int FrameHeight = 64;
        public bool fast;
        public override void OnSpawn()
        {
            Scale = Main.rand.NextFloat(0.13f, 0.7f);
            Rotation += Main.rand.NextFloat(-MathHelper.TwoPi, MathHelper.TwoPi);
            Frame = new Rectangle(0, 0, FrameWidth, FrameHeight);
        }

        public override void Update()
        {
            Velocity *= 0.98f;
            Velocity.Y += 0.0125f;
            Velocity = Velocity.RotatedByRandom(0.09f);
            Rotation += 0.01f;
            //   Scale *= 0.997f;c

            float denom = 180f;
            if (fast)
                denom *= 0.25f;
            color = Color.Lerp(Color.Transparent, Color.White, EasingFunction.QuadraticBump(fadeIn / denom));

            fadeIn++;
            if (fadeIn > denom)
                active = false;

        }
    }
}
