using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class StarMissileBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 5;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                var boom = FXUtil.GlowCircleBoom(Projectile.Center, Color.Pink, Color.Purple, Color.Blue);
                boom.Scale *= 2f;
                FXUtil.GlowCircleDetailedBoom1(Projectile.Center, Color.Pink, Color.Purple, Color.DarkBlue);
                var part2 = FXUtil.GlowCircleDetailedBoom1(Projectile.Center, Color.Pink, Color.Purple, Color.DarkBlue);
                part2.Scale *= 0.5f;
                for (float f = 0; f < 16; f++)
                {
                    Vector2 randVelocity = Main.rand.NextVector2Circular(16, 16);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), randVelocity,
                        newColor: Color.Pink, Scale: Main.rand.NextFloat(0.2f, 0.8f));
                }
                FXUtil.ShakeCamera(Projectile.position, 1024, 16);
                SoundStyle explosionSound = new SoundStyle("Stellamod/Assets/Sounds/STARGROP");
                explosionSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(explosionSound, Projectile.position);
            }
        }
    }
}
