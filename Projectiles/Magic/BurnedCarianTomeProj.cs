using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class BurnedCarianTomeProj : ModProjectile
    {
        private ref float ai_Counter => ref Projectile.ai[0];
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 12;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 3;
            Projectile.timeLeft = 600;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 30;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player

            float distX = targetCenter.X - Projectile.Center.X;
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
            }

            //Accelerate to being above the player.
            float distY = targetCenter.Y - Projectile.Center.Y;
            if (Projectile.Center.Y < targetCenter.Y && Projectile.velocity.Y < moveSpeed)
            {
                Projectile.velocity.Y += accel;
            }
            else if (Projectile.Center.Y > targetCenter.Y && Projectile.velocity.Y > -moveSpeed)
            {
                Projectile.velocity.Y -= accel;
            }
        }

        public override void AI()
        {
            Visuals();
            ai_Counter++;
            Player owner = Main.player[Projectile.owner];
            SummonHelper.SearchForTargets(owner, Projectile, 
                out bool foundTarget, out float distanceFromTarget, out Vector2 targetCenter);


            if(foundTarget)
            {
                AI_Movement(targetCenter, 15);
            }
            else
            {
                AI_Movement(owner.Center, 7);
            }
        }

        public override void OnKill(int timeLeft)
        {
            //Charged Sound thingy
            for (int i = 0; i < 32; i++)
            {
                Vector2 position = Projectile.Center;
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
                    Color.White, 0.5f);
                p.layer = Particle.Layer.BeforeProjectiles;
            }
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
            DrawHelper.DrawAdditiveAfterImage(Projectile, ColorFunctions.MiracleVoid, Color.Transparent, ref lightColor);
            return true;
        }

        private void Visuals()
        {
            if(ai_Counter == 0)
            {
                //Charged Sound thingy
                for (int i = 0; i < 8; i++)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
                        Color.White, 0.5f);
                    p.layer = Particle.Layer.BeforeProjectiles;
                }
            }

            if(ai_Counter % 5 == 0)
            {
                for (int i = 0; i < 2; i++)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<VoidParticle>(),
                        Color.White, 0.5f);
                    p.layer = Particle.Layer.BeforeProjectiles;
                }
            }

            if(ai_Counter % 15 == 0)
            {
                Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(1f, 1f);
                Dust dust = Dust.NewDustPerfect(position, DustID.GemAmethyst, Scale: Main.rand.NextFloat(0.5f, 1f));
                dust.noGravity = true;
            }

            // So it will lean slightly towards the direction it's moving
            float rotation = MathHelper.ToRadians(ai_Counter * 5);
            Projectile.rotation = rotation;
            DrawHelper.AnimateTopToBottom(Projectile, 5);

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }
    }
}
