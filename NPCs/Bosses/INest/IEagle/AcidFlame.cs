using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.INest.IEagle
{
    public class AcidFlame : ModProjectile
    {
        public int Time = 1;
        public float Set = 0.99f;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Acid Flame");
            Main.projFrames[Projectile.type] = 5;
        }
        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 100;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
            Projectile.scale = 1.2f;
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
                if (Projectile.frame >= 5)
                    Projectile.frame = 0;

            }
            return true;
        }
        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.DD2_BetsyFireballImpact, Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 2048f, 16f);
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;

        public override void AI()
        {

            Projectile.localAI[0] += 1f;
            Projectile.ai[0]++;
            if (Projectile.ai[0] == 1)
            {
            }
            Projectile.spriteDirection = Projectile.direction;
            Projectile.rotation = Projectile.velocity.ToRotation() + 1.57f + 3.14f;

            Projectile.velocity *= 1.01f;

            if (Projectile.timeLeft < 50)
            {
                Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 256f, 16f);
                if (Main.netMode != NetmodeID.Server)
                {
                    Dust dust = Dust.NewDustDirect(Projectile.Center, Projectile.width, Projectile.height, DustID.CursedTorch);
                    dust.velocity *= -1f;
                    dust.scale *= .8f;
                    dust.noGravity = true;
                    Vector2 vector2_1 = new Vector2(Main.rand.Next(-80, 81), Main.rand.Next(-80, 81));
                    vector2_1.Normalize();
                    Vector2 vector2_2 = vector2_1 * (Main.rand.Next(50, 100) * 0.04f);
                    dust.velocity = vector2_2;
                    vector2_2.Normalize();
                    Vector2 vector2_3 = vector2_2 * 34f;
                    dust.position = Projectile.Center - vector2_3;
                    Projectile.netUpdate = true;
                }
            }
            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.CursedTorch, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }

        }

    }
}