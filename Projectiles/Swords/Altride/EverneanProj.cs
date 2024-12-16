using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Altride
{
    internal class EverneanProj : ModProjectile
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
            base.Projectile.penetrate = -1;
            base.Projectile.width = 30;
            base.Projectile.height = 30;
            base.Projectile.timeLeft = 700;
            base.Projectile.alpha = 255;
            base.Projectile.friendly = true;
            base.Projectile.hostile = false;
            base.Projectile.ignoreWater = true;
            base.Projectile.tileCollide = false;
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
                    int randy = Main.rand.Next(-50, 50);
                    int randx = Main.rand.Next(-50, 50);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 4, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial2>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);
                }

                if (Main.rand.NextBool(3) && Main.myPlayer == Projectile.owner)
                {
                    int randy = Main.rand.Next(-50, 50);
                    int randx = Main.rand.Next(-50, 50);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride4>(), Projectile.damage * 2, 1, Projectile.owner, 0, 0);
                    Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial2>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);

                }
                else
                {
                    if(Main.myPlayer == Projectile.owner)
                    {
                        int randy = Main.rand.Next(-75, 75);
                        int randx = Main.rand.Next(-75, 75);
                        Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Altride5>(), Projectile.damage, 1, Projectile.owner, 0, 0);
                        Projectile.NewProjectile(EntitySource, Projectile.Center.X + randx, Projectile.Center.Y + randy, StartVelocity.X, StartVelocity.Y, ModContent.ProjectileType<Radial>(), Projectile.damage * 0, 1, Projectile.owner, 0, 0);
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