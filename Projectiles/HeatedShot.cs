using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Utilities;
using Stellamod.Projectiles.Magic;
using Stellamod.Trails;
using Terraria;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class HeatedShot : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("SalfaCirle");
            Main.projFrames[Projectile.type] = 21;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 2;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.width = 216;
            Projectile.height = 120;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            Projectile.knockBack = 12.9f;
            Projectile.aiStyle = 1;
            AIType = ProjectileID.Bullet;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.tileCollide = false;

        }
        public override bool PreAI()
        {

            Projectile.tileCollide = false;

            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }

        public float nigga = 0;
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation();
            Vector3 RGB = new(1.00f, 0.37f, 0.30f);
            // The multiplication here wasn't doing anything
            Lighting.AddLight(Projectile.position, RGB.X, RGB.Y, RGB.Z);

            int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Smoke, 0f, 0f);
            Main.dust[dust].scale = 0.6f;


            nigga++;

            Projectile.velocity *= 1.04f;
            if (nigga < 2)
            {
                ShakeScreenPosition.Shake = 5;
            }


            if (++Projectile.frameCounter >= 2)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 21)
                {
                    Projectile.frame = 0;
                }
            }


        }

     
        public override bool PreDraw(ref Color lightColor)
        {

            return true;
        }


        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.NextBool(3))
            {
                target.AddBuff(BuffID.OnFire, 180);
            }
            var EntitySource = Projectile.GetSource_Death();

        }

    }
}



