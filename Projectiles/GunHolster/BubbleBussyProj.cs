using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.GunHolster
{
    internal class BubbleBussyProj : ModProjectile
    {
        public override void SetStaticDefaults()
        {
            Main.projFrames[Projectile.type] = 7;
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 8;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 20;
            Projectile.friendly = true;
        }

        public override void AI()
        {
            Projectile.velocity.Y += 0.4f;
            Projectile.rotation = Projectile.velocity.ToRotation();
            Visuals();
        }

        private void Visuals()
        {
            DrawHelper.AnimateTopToBottom(Projectile, 5);
            if (Main.rand.NextBool(3))
            {
                if (Main.rand.NextBool(12))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.BoneTorch);
                }
  
                Vector2 speed = Main.rand.NextVector2Circular(0.5f, 0.5f);
                ParticleManager.NewParticle(Projectile.Center, speed * 1, ParticleManager.NewInstance<VoidParticle>(), Color.RosyBrown, Main.rand.NextFloat(0.2f, 0.8f));
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Black * 0.25f, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.VortexTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            int targetNpc = target.whoAmI;
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity,
                ModContent.ProjectileType<BubbleBussyStickyProj>(), Projectile.damage, Projectile.knockBack, Projectile.owner, ai0: targetNpc);
        }


        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.position + oldVelocity, Projectile.velocity,
              ModContent.ProjectileType<BubbleBussyStickyProj2>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
            return base.OnTileCollide(oldVelocity);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 16; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(2f, 2f);
                var d = Dust.NewDustPerfect(Projectile.Center, DustID.BoneTorch, speed * 4);
                d.noGravity = true;
            }
        }
    }
}
