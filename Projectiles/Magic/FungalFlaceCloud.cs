using Microsoft.Xna.Framework;
using Stellamod.Dusts;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Projectiles.Magic
{
    internal class FungalFlaceCloud : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Fungal Flace Cloud");
        }

        public override void SetDefaults()
        {

            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 210;
            Projectile.tileCollide = false;
            Projectile.height = 35;
            Projectile.width = 35;
            Projectile.penetrate = 10;
            AIType = ProjectileID.Bullet;
            Projectile.extraUpdates = 1;
        }

        public override bool PreAI()
        {
            Projectile.alpha++;

            float num = 1f - Projectile.alpha / 255f;
            Projectile.velocity *= .98f;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f;
            num *= Projectile.scale;
            Lighting.AddLight(Projectile.Center, 0.3f * num, 0.2f * num, 0.1f * num);
            Projectile.rotation = Projectile.velocity.X / 2f;
            return true;
          
        }
        float alphaCounter;

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }
        public override void AI()
        {
            alphaCounter += 0.04f;
            Projectile.rotation += 0.3f;
            if (Main.rand.NextBool(40))
            {
                for (int i = 0; i < 2; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(-2, 2)).RotatedByRandom(19.0), 0, Color.Pink, 0.4f).noGravity = true;
                }
            }
        }
    }
}
