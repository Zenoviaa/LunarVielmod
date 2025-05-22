using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Brooches
{
    public class VerliaBroochP2 : ModProjectile
    {

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
            Lighting.AddLight(Projectile.Center, Color.LightSkyBlue.ToVector3() * 1.25f * Main.essScale);
            Projectile.rotation = Projectile.velocity.X / 2f;
            return true;
        }
        float alphaCounter;
        public override void AI()
        {
            alphaCounter += 0.04f;
            Projectile.rotation += 0.5f;

        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 255f);
        }


    }
}
