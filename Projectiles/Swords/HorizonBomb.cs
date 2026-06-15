using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.Graphics.Shaders;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    public class HorizonBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
            // DisplayName.SetDefault("Abyssal Charge");
        }

        public override void SetDefaults()
        {
            Projectile.tileCollide = true;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.DamageType = DamageClass.Magic;
            Projectile.penetrate = 51;
            Projectile.timeLeft = 240;
            Projectile.height = 48;
            Projectile.width = 48;
            AIType = ProjectileID.Bullet;
            Projectile.extraUpdates = 1;
        }

        int counter = 9;
        float alphaCounter = 9;
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }


        public override bool PreDraw(ref Color lightColor)
        {

            return false;
        }

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
                //float x = Projectile.Center.X - Projectile.velocity.X / 10f * i;
                //float y = Projectile.Center.Y - Projectile.velocity.Y / 10f * i;
            }
        }

        public override void OnKill(int timeLeft)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(base.Projectile.Center, 1212f, 62f);
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(19.0), 0, default(Color), 4f).noGravity = true;
            }

            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(10.0), 0, default(Color), 1f).noGravity = false;
            }

            for (int i = 0; i < 40; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 6f).noGravity = true;
            }

            for (int i = 0; i < 20; i++)
            {
                Dust.NewDustPerfect(base.Projectile.Center, DustID.BlueTorch, (Vector2.One * Main.rand.Next(1, 12)).RotatedByRandom(25.0), 0, default(Color), 2f).noGravity = false;
            }

            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
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