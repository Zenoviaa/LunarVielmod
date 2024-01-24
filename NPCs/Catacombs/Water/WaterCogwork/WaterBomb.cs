using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Projectiles;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Catacombs.Water.WaterCogwork
{
    internal class WaterBomb : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 18;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.timeLeft = 300;
            Projectile.friendly = false;
            Projectile.hostile = true;
        }

        public override void AI()
        {
            Visuals();
        }

        //Trails
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 1.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Aqua * 0.8f, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            //Draw The Body
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            DrawHelper.DrawAdditiveAfterImage(Projectile, Color.Aqua * 0.8f, Color.Transparent, ref lightColor);
            return base.PreDraw(ref lightColor);
        }

        private void Visuals()
        {
            for (int i = 0; i < Main.rand.Next(2, 4); i++)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(4, 4);
                float size = Main.rand.NextFloat(0.25f, 0.33f);
                Particle p = ParticleManager.NewParticle(position, Vector2.Zero, ParticleManager.NewInstance<WaterParticle>(),
                    default(Color), size);
                p.layer = Particle.Layer.BeforeProjectiles;
            }

            if (Main.rand.NextBool(8))
            {
                Dust.NewDust(Projectile.Center, 2, 2, ModContent.DustType<BubbleDust>());
            }
        }

        public override void OnKill(int timeLeft)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero,
                ModContent.ProjectileType<WataBoom>(), Projectile.damage, 6, Projectile.owner);
            int count = 32;
            for (int i = 0; i < count; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                Particle p = ParticleManager.NewParticle(Projectile.Center, speed, ParticleManager.NewInstance<WaterParticle>(),
                    default(Color), 1 / 2f);
            }

            for (int i = 0; i < count / 2; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<BubbleDust>(), speed);
            }
        }
    }
}
