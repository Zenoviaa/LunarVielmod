using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Gores;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Visual
{
    public class SplashProj : ModProjectile
    {
        public Color SplashColor;
        ref float Timer => ref Projectile.ai[0];
        ref float Color => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 6;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 2;
            Projectile.height = 2;
            Projectile.friendly = false;
            Projectile.hostile = false;
        }

        public override void AI()
        {
            Timer++;
            if (Timer == 1)
            {
                Color = Main.rand.Next(6);
                switch (Color)
                {
                    default:
                    case 0:
                        //Yellow
                        SplashColor = new Color(255, 205, 0);
                        break;
                    case 1:
                        //Red
                        SplashColor = new Color(255, 0, 0);
                        break;
                    case 2:
                        //Orange
                        SplashColor = new Color(255, 113, 0);
                        break;
                    case 3:
                        //Green
                        SplashColor = new Color(0, 255, 14);
                        break;
                    case 4:
                        //Blue
                        SplashColor = new Color(0, 135, 255);
                        break;
                    case 5:
                        //Black
                        SplashColor = new Color(0, 0, 0);
                        break;
                }
            }

            Projectile.velocity.Y += 0.2f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            var source = Projectile.GetSource_FromThis();
            Gore g;
            switch (Color)
            {
                default:
                case 0:
                    //Yellow
                    g = Gore.NewGorePerfect(source, Projectile.Center, oldVelocity, GoreHelper.TypeSplashYellow);
                    break;
                case 1:
                    //Red
                    g = Gore.NewGorePerfect(source, Projectile.Center, oldVelocity, GoreHelper.TypeSplashRed);
                    break;
                case 2:
                    //Orange
                    g = Gore.NewGorePerfect(source, Projectile.Center, oldVelocity, GoreHelper.TypeSplashOrange);
                    break;
                case 3:
                    //Green
                    g = Gore.NewGorePerfect(source, Projectile.Center, oldVelocity, GoreHelper.TypeSplashGreen);
                    break;
                case 4:
                    //Blue
                    g = Gore.NewGorePerfect(source, Projectile.Center, oldVelocity, GoreHelper.TypeSplashBlue);
                    break;
                case 5:
                    //Black
                    g = Gore.NewGorePerfect(source, Projectile.Center, oldVelocity, GoreHelper.TypeSplashBlack);
                    break;
            }

            g.position += oldVelocity * 1.3f;
            g.rotation = oldVelocity.ToRotation() - MathHelper.PiOver2;
            return base.OnTileCollide(oldVelocity);
        }
    }
}
