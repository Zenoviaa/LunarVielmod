using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    public class Nado : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Tornado");
            Main.projFrames[Projectile.type] = 3;
        }

        public override void SetDefaults()
        {
            Projectile.width = 45;
            Projectile.height = 45;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 1;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
            Projectile.damage = 15;
            Projectile.aiStyle = -1;
        }
        public override bool PreAI()
        {
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 4)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 3)
                    Projectile.frame = 0;

            }
            return true;
        }
        public override Color? GetAlpha(Color lightColor)
        {
            return Color.White;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item14, Projectile.position);
            for (int i = 0; i < 40; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, 0f, -2f, 0, default, 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].scale = 1.9f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
            for (int i = 0; i < 40; i++)
            {
                int num = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Smoke, 0f, -2f, 0, default, 1.5f);
                Main.dust[num].noGravity = true;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                Main.dust[num].position.X += Main.rand.Next(-50, 51) * .05f - 1.5f;
                {
                    Main.dust[num].velocity = Projectile.DirectionTo(Main.dust[num].position) * 6f;
                }
            }
        }
        public override void AI()
        {
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;
            Projectile.localAI[0] += 1f;
            if (Projectile.localAI[0] == 16f)
            {
                Projectile.localAI[0] = 0f;
                for (int j = 0; j < 10; j++)
                {
                    Vector2 vector2 = Vector2.UnitX * -Projectile.width / 2f;
                    vector2 += -Vector2.UnitY.RotatedBy(j * 3.141591734f / 6f, default) * new Vector2(8f, 16f);
                    vector2 = vector2.RotatedBy(Projectile.rotation - 1.57079637f, default);
                    int num8 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.Smoke, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                    Main.dust[num8].scale = 1.3f;
                    Main.dust[num8].noGravity = true;
                    Main.dust[num8].position = Projectile.Center + vector2;
                    Main.dust[num8].velocity = Projectile.velocity * 0.1f;
                    Main.dust[num8].noLight = true;
                    Main.dust[num8].velocity = Vector2.Normalize(Projectile.Center - Projectile.velocity * 3f - Main.dust[num8].position) * 1.25f;
                }
            }
            int num1222 = 74;
            for (int k = 0; k < 2; k++)
            {
                int index2 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.UnusedWhiteBluePurple, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[index2].position = Projectile.Center - Projectile.velocity / num1222 * k;
                Main.dust[index2].scale = .95f;
                Main.dust[index2].velocity *= 0f;
                Main.dust[index2].noGravity = true;
                Main.dust[index2].noLight = false;
            }

        }
    }
}