using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Altride
{
    internal class MooningProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        bool Moved;
        Vector2 StartVelocity;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gladiator Spear");
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 15;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 1;
        }
        public override void SetDefaults()
        {
            Projectile.DamageType = DamageClass.Melee;
            Projectile.penetrate = 2;
            Projectile.width = 30;
            Projectile.height = 30;
            Projectile.timeLeft = 700;
            Projectile.alpha = 255;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
        }
        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                StartVelocity = Projectile.velocity;
            }

            if (Timer == 2)
            {
                Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
                Projectile.velocity = -Projectile.velocity;
            }

            if (Timer == 5 || Timer == 10 || Timer == 15 || Timer == 20)
            {
                var EntitySource = Projectile.GetSource_FromThis();
                if (Main.rand.NextBool(8) && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + Main.rand.Next(-50, 50), Projectile.Center.Y + Main.rand.Next(-50, 50), StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                }

                if (Main.rand.NextBool(3) && Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + Main.rand.Next(-50, 50), Projectile.Center.Y + Main.rand.Next(-50, 50), StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                }
                else
                {
                    if (Main.myPlayer == Projectile.owner)
                    {
                        Projectile.NewProjectile(EntitySource, Projectile.Center.X + Main.rand.Next(-50, 50), Projectile.Center.Y + Main.rand.Next(-50, 50), StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                    }

                }
            }
            if (Timer >= 0 && Timer <= 20)
            {
                Projectile.velocity *= .86f;

            }
            if (Timer == 20)
            {
                Projectile.velocity = -Projectile.velocity;
            }
            if (Timer >= 21 && Timer <= 60)
            {
                Projectile.velocity /= .90f;

            }
            if (Timer == 60)
            {
                Projectile.tileCollide = true;
            }
        }

        public override void OnKill(int timeLeft)
        {

        }

        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }

        public override void PostDraw(Color lightColor)
        {

        }
    }
}