using Stellamod.Trails;
using Stellamod.Effects;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.ModLoader;
using Terraria.Graphics.Shaders;
using Terraria.GameContent;
using Terraria.ID;
using Stellamod.Projectiles.Swords;

namespace Stellamod.Projectiles.Magic
{
    public class OFlame : ModProjectile
    {
        public float VELY = 1;
        public float VEL = 1;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("OFlame");
            Main.projFrames[Projectile.type] = 4;
        }
        public override void SetDefaults()
        {
            Projectile.width = 14;
            Projectile.height = 14;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 10;
            Projectile.timeLeft = 900;
            Projectile.tileCollide = true;
            Projectile.aiStyle = -1;
        }

        public override bool PreAI()
        {
            Projectile.alpha -= 40;
            if (Projectile.alpha < 0)
                Projectile.alpha = 0;

            Projectile.spriteDirection = Projectile.direction;
            Projectile.frameCounter++;
            if (Projectile.frameCounter >= 3)
            {
                Projectile.frame++;
                Projectile.frameCounter = 0;
                if (Projectile.frame >= 4)
                    Projectile.frame = 0;

            }
            return true;
        }
        public override void Kill(int timeLeft)
        {
            for (int i = 0; i < 20; i++)
            {
                Dust.NewDust(Projectile.Center, Projectile.width, Projectile.height, DustID.Firework_Yellow, 0, 60, 133);
            }
        }
        public override Color? GetAlpha(Color lightColor) => Color.White;
        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            if (Main.rand.Next(1) == 0)
                target.AddBuff(BuffID.OnFire, 180);
        }
        public override void AI()
        {
            Lighting.AddLight(Projectile.Center, Color.Orange.ToVector3() * 1.75f * Main.essScale);
            Projectile.rotation = Projectile.velocity.X * 0.05f;
            Projectile.ai[0]++;
            Projectile.ai[1]++;
            Player player;
            if ((player = VectorHelper.GetNearestPlayerDirect(Projectile.position, Alive: true)) != null)
            {
                if (Projectile.position.X <= player.position.X)
                {
                    Projectile.velocity.X += 0.1f;
                }
                else
                {
                    Projectile.velocity.X -= 0.1f;
                }
            }
            if(Projectile.ai[0] <= 40)
            {
                Projectile.velocity *= 0.97f;
            }
            else
            {
                Projectile.velocity.X += VEL;
                Projectile.velocity.Y += VELY;
            }

            if (Projectile.ai[1] >= 40)
            {
                VEL = Main.rand.NextFloat(-0.15f, 0.15f);
                VELY = Main.rand.NextFloat(-0.15f, 0.15f);
                Projectile.ai[1] = 0;
            }



            if (Main.rand.NextBool(29))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust].noGravity = true;
                Main.dust[dust].scale = 1.5f;
            }

            if (Main.rand.NextBool(29))
            {
                int dust3 = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, DustID.FlameBurst, Projectile.velocity.X * 0.5f, Projectile.velocity.Y * 0.5f);
                Main.dust[dust3].noGravity = true;
                Main.dust[dust3].scale = 1.5f;
            }

        }

    }
}