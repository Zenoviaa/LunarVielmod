using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DreadMire
{
    internal class DreadSineSkull : ModProjectile
    {
        private Vector2 initialSpeed;
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 22;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 360;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                initialSpeed = Projectile.velocity;
            }

            float distance = 8;
            float frequency = 4;
            Vector2 offset = initialSpeed.RotatedBy(Math.PI / 2);
            offset.Normalize();
            offset *= (float)(Math.Cos((Timer * frequency) * (Math.PI / 180)) * (distance));
            Projectile.velocity = initialSpeed + offset;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Lighting.AddLight(Projectile.Center, Color.DarkRed.ToVector3() * 2.25f * Main.essScale);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Red, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }
    }
}
