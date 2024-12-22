using Microsoft.Xna.Framework;
using Stellamod.Buffs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Bow
{
    internal class CloudArrow : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.friendly = true;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer % 6 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, Projectile.velocity * 0.1f, 0, Color.White, Main.rand.NextFloat(0.5f, 1f)).noGravity = true;
            }
            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(90);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            base.OnHitNPC(target, hit, damageDone);
            if (Main.rand.NextBool(2))
                target.AddBuff(ModContent.BuffType<Clouded>(), 240);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 4; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Cloud, Projectile.velocity.RotatedByRandom(MathHelper.PiOver4) * 0.1f, 0, Color.White, Main.rand.NextFloat(1f, 2f)).noGravity = true;
            }
        }
    }
}
