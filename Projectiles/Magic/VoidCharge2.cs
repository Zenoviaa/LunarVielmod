using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class VoidCharge2 : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            // DisplayName.SetDefault("Abyssal Charge");
        }

        public override void SetDefaults()
        {
            Projectile.hide = true;
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 2;
            Projectile.height = 32;
            Projectile.width = 32;
            AIType = ProjectileID.Bullet;
            Projectile.extraUpdates = 1;
        }

        int counter = 6;
        public override void AI()
        {
            Projectile.rotation += 0.09f;
            Projectile.velocity *= 0.99f;
            if (counter >= 1440)
            {
                counter = -1440;
            }
            for (int i = 0; i < 4; i++)
            {
                float x = Projectile.Center.X - Projectile.velocity.X / 10f * i;
                float y = Projectile.Center.Y - Projectile.velocity.Y / 10f * i;
            }
        }

        public override void OnKill(int timeLeft)
        {
            int offsetRandom = Main.rand.Next(0, 50);
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.CopperCoin, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
            }
            float spread = 45f * 0.0174f;
            double startAngle = Math.Atan2(1, 0) - spread / 2;
            double deltaAngle = spread / 8f;
            double offsetAngle;
            for (int i = 0; i < 4; i++)
            {
                offsetAngle = (startAngle + deltaAngle * (i + i * i) / 2f) + 32f * i + offsetRandom;
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(Math.Sin(offsetAngle) * 9f), (float)(Math.Cos(offsetAngle) * 9f), 
                    ModContent.ProjectileType<VoidCharge>(), 39, 0, Projectile.owner);
                Projectile.NewProjectile(Projectile.GetSource_FromAI(), Projectile.Center.X, Projectile.Center.Y, (float)(-Math.Sin(offsetAngle) * 9f), (float)(-Math.Cos(offsetAngle) * 9f), 
                    ModContent.ProjectileType<VoidCharge>(), 39, 0, Projectile.owner);

            }
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/TOTS2"), Projectile.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/TOTS1"), Projectile.position);
            }
            for (int i = 0; i < 10; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BlueCrystalShard, 0f, -2f, 0, default, 1.1f);
                Main.dust[num].noGravity = true;
                Dust expr_62_cp_0 = Main.dust[num];
                expr_62_cp_0.position.X = expr_62_cp_0.position.X + (Main.rand.Next(-30, 31) / 20 - 1.5f);
                Dust expr_92_cp_0 = Main.dust[num];
                expr_92_cp_0.position.Y = expr_92_cp_0.position.Y + (Main.rand.Next(-30, 31) / 20 - 1.5f);
                if (Main.dust[num].position != Projectile.Center)
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }
    }
}