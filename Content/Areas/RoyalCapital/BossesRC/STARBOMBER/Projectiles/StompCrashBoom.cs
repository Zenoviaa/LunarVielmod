using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class StompCrashBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 128;
            Projectile.height = 128;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 15;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
        }
    }
}
