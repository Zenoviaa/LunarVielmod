using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class WigglerDetonator : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            foreach (var p in Main.ActiveProjectiles)
            {
                if (Main.myPlayer == Projectile.owner)
                {
                    if (p.type == ModContent.ProjectileType<WigglerStick>() ||
                            p.type == ModContent.ProjectileType<WigglerStick2>())
                    {
                        if (p.ai[2] == 0)
                        {
                            p.ai[1] = Main.rand.Next(30, 90);
                            p.ai[2] = 1;
                            p.netUpdate = true;
                        }
                    }
                }
            }
            Projectile.Kill();
        }
    }
}
