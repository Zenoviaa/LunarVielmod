using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles
{
    internal class CinderFireball2 : ModProjectile
    {
        private ref float ai_Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 16;
            Projectile.width = 16;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            ai_Timer++;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            target.AddBuff(BuffID.OnFire, 180);
        }

        private void Visuals()
        {
            float radius = 1/6f;
            for(int i = 0; i < 2; i++)
            {
                float speedX = Main.rand.NextFloat(-radius, radius);
                float speedY = Main.rand.NextFloat(-radius, radius);
                float scale = Main.rand.NextFloat(0.66f, 1f);
                int d = Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork,
                    speedX, speedY, Scale: scale);
                Main.dust[d].noGravity = true;
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.InfernoFork, speed, Scale: 3f);
                d.noGravity = true;
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkOrange, Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.WhispyTrail);
            return base.PreDraw(ref lightColor);
        }
    }
}
