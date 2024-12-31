using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.StarrVeriplant.Projectiles
{
    internal class StompShockwave : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Vector2 _targetVelocity;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 30;
            ProjectileID.Sets.TrailCacheLength[Type] = 5;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }
        public override void SetDefaults()
        {
            Projectile.width = 12;
            Projectile.height = 12;
            Projectile.penetrate = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 60;
            Projectile.tileCollide = false;
            Projectile.light = 0.78f;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                _targetVelocity = Projectile.velocity;
                Projectile.velocity = _targetVelocity.SafeNormalize(Vector2.Zero);
            }

            Projectile.velocity = Vector2.Lerp(Projectile.velocity, _targetVelocity, 0.1f);
            DrawHelper.AnimateTopToBottom(Projectile, 2);
            if(Timer > 50)
            {
                Projectile.hostile = false;
            }
        }
    }
}
