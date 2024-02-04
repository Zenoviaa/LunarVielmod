using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class CinderFlameball : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 4;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.height = 16;
            Projectile.width = 16;
            Projectile.hostile = false;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 120;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.33f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            float radius = 1 / 6f;
            if (Main.rand.NextBool(12))
            {
                for (int i = 0; i < 2; i++)
                {
                    float speedX = Main.rand.NextFloat(-radius, radius);
                    float speedY = Main.rand.NextFloat(-radius, radius);
                    float scale = Main.rand.NextFloat(0.66f, 1f);
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.InfernoFork,
                        speedX, speedY, Scale: scale);
                }
            }
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            target.AddBuff(BuffID.OnFire3, 180);
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

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.InfernoFork, speed);
                d.noGravity = true;
            }
        }
    }
}
