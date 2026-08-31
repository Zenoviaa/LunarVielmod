using Microsoft.Xna.Framework;
using Stellamod.Content.Dusts;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    public class Meatball4 : ModProjectile
    {
        public float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }
        private ref float SwordRotation => ref Projectile.ai[1];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Balls");
            Main.projFrames[Projectile.type] = 32;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = true;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 92;
            Projectile.height = 92;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 32;
            Projectile.scale = 1f;

        }

        public override void AI()
        {

            Timer++;
            Player player = Main.player[Projectile.owner];
            if (player.noItems || player.CCed || player.dead || !player.active)
                Projectile.Kill();

            Projectile.Center = Main.player[Projectile.owner].Center;
        }

        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            if (++Projectile.frameCounter >= 1)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 32)
                {
                    Projectile.frame = 0;
                }
            }





            if (Main.rand.NextBool(8))
            {
                int dust = Dust.NewDust(Projectile.position + Projectile.velocity, Projectile.width, Projectile.height, ModContent.DustType<PaintBlob2>(), 0f, 0f);
                Main.dust[dust].scale = 1f;
            }
 
            return true;
        }

    }

}
