using Microsoft.Xna.Framework;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.DreadMire
{
    public class DreadMireMagic : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 19;
        }

        public override void SetDefaults()
        {
            Projectile.friendly = false;
            Projectile.DamageType = DamageClass.Ranged;
            Projectile.width = 256;
            Projectile.height = 256;
            Projectile.penetrate = -1;
            Projectile.scale = 1f;
            DrawOriginOffsetY = 0;
            Projectile.damage = 0;
            Projectile.timeLeft = 72;
        }
        
        public override bool PreAI()
        {
            Projectile.tileCollide = false;
            return true;
        }

        public override Color? GetAlpha(Color lightColor)
        {
            return new Color(255, 255, 255, 0) * (1f - Projectile.alpha / 50f);
        }
        public override void AI()
        {
            Projectile.rotation = 30;
            if (++Projectile.frameCounter >= 6)
            {
                Projectile.frameCounter = 0;
                if (++Projectile.frame >= 19)
                {
                    Projectile.frame = 0;
                }
            }
        }
    }
}



