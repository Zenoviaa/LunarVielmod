using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.NPCs.Bosses.Gustbeak.Projectiles
{
    internal class WindBoom : BaseWindProjectile
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.timeLeft = 30;
            Projectile.light = 0.2f;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            if (Timer == 1)
            {
                SoundStyle windHit = new SoundStyle($"Stellamod/Assets/Sounds/WindHit", variantSuffixesStart: 1, numVariants: 2);
                windHit.PitchVariance = 0.15f;
                SoundEngine.PlaySound(windHit, Projectile.position);

                for (float f = 0; f < 12; f++)
                {
                    float rot = (f / 12f) * MathHelper.TwoPi;
                    Vector2 velOffset = rot.ToRotationVector2() * 6;
                    Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GemDiamond, velOffset, Scale: 1f);
                    d.noGravity = true;
                }
            }

            float progress = Timer / 30f;
            float easedProgress = Easing.OutCirc(progress);
            float spikingEasedProgress = Easing.OutCirc(progress);
            if (Timer < 10)
            {
                Vector2 offset = (Timer / 10f).ToRotationVector2() * 4;
                float rotation = offset.ToRotation();
                rotation += Main.rand.NextFloat(-1f, 1f);
                offset -= Projectile.Size / 2f;
                var slash = Wind.NewSlash(offset, rotation);
                slash.duration *= 1.5f;
            }

            ShadowScale = 0f;
            Wind.ExpandMultiplier = MathHelper.Lerp(0f, 2f, easedProgress);
            Wind.WidthMultiplier = MathHelper.Lerp(1f, 0f, progress);
        }
    }
}
