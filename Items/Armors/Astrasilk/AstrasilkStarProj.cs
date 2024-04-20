using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Astrasilk
{
    internal class AstrasilkStarProj : ModProjectile
    {
        private static float _orbitingOffset;
        private Player Owner => Main.player[Projectile.owner];
     
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        private int Index
        {
            get => (int)Projectile.ai[1];
            set => Projectile.ai[1] = value;
        }
        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = false;
            Projectile.hostile = false;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = int.MaxValue;
        }

        public override void AI()
        {
            _orbitingOffset += MathHelper.PiOver4 / 64;
            Projectile.rotation += MathHelper.PiOver4 / 16;

            Vector2 targetCirclePosition = CalculateCirclePosition();
            Projectile.Center = Vector2.Lerp(Projectile.Center, targetCirclePosition, 0.075f);
        
            Timer++;
            if (Timer == 1)
            {
                Index = SummonHelper.GetProjectileIndex(Projectile);
                //Spawn effect ig
                for (int i = 0; i < 8; i++)
                {
                    int num1 = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, 0f, -2f, 0, default, .8f);
                    Main.dust[num1].noGravity = true;
                    Main.dust[num1].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num1].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num1].position != Projectile.Center)
                        Main.dust[num1].velocity = Projectile.DirectionTo(Main.dust[num1].position) * 6f;
                    int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.WhiteTorch, 0f, -2f, 0, default, .8f);
                    Main.dust[num].noGravity = true;
                    Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    Main.dust[num].position.Y += Main.rand.Next(-50, 51) * .05f - 1.5f;
                    if (Main.dust[num].position != Projectile.Center)
                        Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }

        private Vector2 CalculateCirclePosition()
        {
            //Get the index of this minion
            int index = Index;

            //Now we can calculate the circle position	
            int count = Owner.ownedProjectileCounts[Type];
            float degreesBetween = MathHelper.TwoPi / 5;
            float degrees = degreesBetween * index;
            float circleDistance = 64;
            Vector2 circleCenter = Owner.Center;
            Vector2 circleOffset = new Vector2(circleDistance, 0);
            Vector2 rotatedCirclePosition = circleCenter + circleOffset.RotatedBy(degrees + _orbitingOffset);
            return rotatedCirclePosition;
        }
    }
}
