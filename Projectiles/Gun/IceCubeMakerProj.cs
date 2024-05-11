using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Gun
{
    internal class IceCubeMakerProj : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            Main.projFrames[Type] = 7;
            ProjectileID.Sets.TrailCacheLength[Type] = 12;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = Projectile.height = 36;
            Projectile.friendly = true;
            Projectile.penetrate = 12;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 600;
            Projectile.light = 0.78f;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 60;
        }

        public override void AI()
        {
            Timer++;
            Projectile.velocity *= 0.98f;
            Projectile.rotation += Projectile.velocity.Length() * 0.05f;
            AI_Collide();
            Visuals();
        }

        private void AI_Collide()
        {
            if (Timer < 30)
                return;
            Rectangle myRect = Projectile.getRect();
            for (int i = 0; i < Main.maxProjectiles; i++)
            {
                Projectile p = Main.projectile[i];
                if (!p.active)
                    continue;
                if (p.type != Projectile.type)
                    continue;
                if (p == Projectile)
                    continue;
                Rectangle otherRect = p.getRect();
                if (Projectile.Colliding(myRect, otherRect))
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                    float scale = Main.rand.NextFloat(0.3f, 0.5f);
                    ParticleManager.NewParticle<SnowFlakeParticle>(Projectile.Center, velocity, Color.White, scale);
                    
                    SoundStyle soundStyle = SoundID.NPCHit11;
                    soundStyle.Pitch = 0.5f;
                    soundStyle.PitchVariance = 0.15f;
                    SoundEngine.PlaySound(soundStyle, Projectile.position);

                    Vector2 directionToProjectile = Projectile.Center.DirectionTo(p.Center);
                    p.velocity = directionToProjectile * 16;
                    p.ai[0] = 20;

                    Vector2 bounceVelocity = -Projectile.velocity * 1.5f;
                    Projectile.velocity = bounceVelocity.RotatedByRandom(MathHelper.PiOver4 / 4);
                    Projectile.damage += 5;
                }
            }
        }

        private void Visuals()
        {
            //Animations
            Projectile.frameCounter++;

            int frameSpeed = 7;
            if (Projectile.frameCounter >= frameSpeed)
            {
                Projectile.frameCounter = 0;
                Projectile.frame++;

                if (Projectile.frame >= Main.projFrames[Projectile.type])
                {
                    Projectile.frame = Main.projFrames[Projectile.type] - 1;
                }
            }
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.LightCyan, Color.Transparent, completionRatio) * 0.2f;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.StarTrail);
            return base.PreDraw(ref lightColor);
        }

        public override void OnHitNPC(NPC target, NPC.HitInfo hit, int damageDone)
        {
            Vector2 bounceVelocity = -Projectile.velocity * 1.5f;
            Projectile.velocity = bounceVelocity.RotatedByRandom(MathHelper.PiOver4 / 16);
            Projectile.velocity += -Vector2.UnitY * 8;
            Projectile.damage += 5;
            if (Main.rand.NextBool(8))
            {
                target.AddBuff(BuffID.Frostburn, 120);
            }
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            Projectile.penetrate--;
            return false;
        }

        public override void OnKill(int timeLeft)
        {
            SoundEngine.PlaySound(SoundID.Item27, Projectile.position);
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(16, 16);
                float scale = Main.rand.NextFloat(0.3f, 0.5f);
                ParticleManager.NewParticle<SnowFlakeParticle>(Projectile.Center, velocity, Color.White, scale);
                if (Main.rand.NextBool(2))
                {
                    Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Snow);
                }
            }
        }
    }
}
