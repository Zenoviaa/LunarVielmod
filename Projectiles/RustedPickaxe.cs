using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class RustedPickaxe : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 42;
            Projectile.height = 42;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            //This makes it slow down
            Projectile.velocity.X *= 0.99f;

            //This makes it fall down
            Projectile.velocity.Y += 0.15f;

            //This makes the rotation effect scale with the velocity
            Projectile.rotation += Projectile.velocity.Length() * 0.02f;
            Visuals();
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Electric);
            }

            Lighting.AddLight(Projectile.position, Color.White.ToVector3() * 0.78f * Main.essScale);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.Electric, speed);
                d.noGravity = true;
            }

            SoundEngine.PlaySound(SoundID.Dig, Projectile.position);   
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Cyan, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
