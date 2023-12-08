using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords.Ripper
{
    internal class RipperSwordProj : ModProjectile
    {
        private const int Delay = 15;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 24;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.tileCollide = false;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.timeLeft = 82-50;
        }

        public override void AI()
        {
            ref float ai_Counter = ref Projectile.ai[0];
            ai_Counter++;
            if (ai_Counter == Delay)
            {
                //I made the projectile just move super slow when it spawned, so gotta do this to return to normal speed.
                Projectile.velocity *= 100;
            }

            Projectile.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            Visuals();
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(60, 0, 118, 175), Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(60, 0, 118), Color.Black, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            if (Main.rand.NextBool(20))
            {
                Dust.NewDust(Projectile.Center, 2, 2, DustID.GemAmethyst);
            }

            Lighting.AddLight(Projectile.Center, Color.Pink.ToVector3() * 0.28f);
        }

        public override void OnKill(int timeLeft)
        {
            var proj = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
              ModContent.ProjectileType<RipperSlashProjBig>(), 0, 0f, Projectile.owner);
            RipperSlashProjBig ripperSlash = proj.ModProjectile as RipperSlashProjBig;
            ripperSlash.randomRotation = false;
            proj.rotation = Projectile.velocity.ToRotation() + MathHelper.ToRadians(45);
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.GemAmethyst, speed, Scale: 3f);
                d.noGravity = true;
            }
        }
    }
}
