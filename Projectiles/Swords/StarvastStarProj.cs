using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Swords
{
    internal class StarvastStarProj : ModProjectile,
        IPixelPrimitiveDrawer
    {
        private ref float ai_Counter => ref Projectile.ai[0];
        ref float Timer => ref Projectile.ai[1];
        bool foundTarget;
        internal PrimitiveTrail BeamDrawer;
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Spragald");
            // Sets the amount of frames this minion has on its spritesheet
            ProjectileID.Sets.TrailCacheLength[Projectile.type] = 7;
            ProjectileID.Sets.TrailingMode[Projectile.type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 22;
            Projectile.height = 22;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.aiStyle = -1;
            Projectile.penetrate = 2;
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
                out foundTarget, out float distanceFromTarget, out Vector2 targetCenter);
            if (foundTarget)
            {
                AI_Movement(targetCenter, 15);
            }
            else
            {
                Timer += 0.02f;
                Vector2 orbitCenter = MovementHelper.OrbitAround(owner.Center, Vector2.UnitY, 64, Timer);
                Projectile.Center = Vector2.Lerp(Projectile.Center, orbitCenter, 0.8f);
            }
        }

        public override void OnKill(int timeLeft)
        {
            //Charged Sound thingy
            for (int i = 0; i < 32; i++)
            {
                Vector2 position = Projectile.Center;
                Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<StarParticle2>(),
                    Color.White, 0.5f);
                p.layer = Particle.Layer.BeforeProjectiles;
            }
        }


        public float WidthFunction(float completionRatio)
        {
            return Projectile.scale * 2f;
        }


        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(new Color(44, 84, 94), Color.Transparent, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawAdditiveAfterImage(Projectile, new Color(44, 84, 94), Color.Transparent, ref lightColor);
            return true;
        }

        private void Visuals()
        {
            if (ai_Counter == 0)
            {
                //Charged Sound thingy
                for (int i = 0; i < 8; i++)
                {
                    Vector2 position = Projectile.Center;
                    Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                    Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<StarParticle2>(),
                        Color.White, 0.5f);
                    p.layer = Particle.Layer.BeforeProjectiles;
                }
            }

            if (foundTarget)
            {
                if (ai_Counter % 8 == 0)
                {
                    for (int i = 0; i < 2; i++)
                    {
                        Vector2 position = Projectile.Center;
                        Vector2 speed = Main.rand.NextVector2CircularEdge(1f, 1f);
                        Particle p = ParticleManager.NewParticle(position, speed, ParticleManager.NewInstance<StarParticle2>(),
                            Color.White, 0.5f);
                        p.layer = Particle.Layer.BeforeProjectiles;
                    }
                }

                if (ai_Counter % 15 == 0)
                {
                    Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(1f, 1f);
                    Dust dust = Dust.NewDustPerfect(position, ModContent.DustType<GunFlash>(), Scale: Main.rand.NextFloat(0.5f, 1f));
                    dust.noGravity = true;
                }

            }



            // So it will lean slightly towards the direction it's moving
            float rotation = MathHelper.ToRadians(ai_Counter * 5);
            Projectile.rotation = rotation;
            DrawHelper.AnimateTopToBottom(Projectile, 5);

            // Some visuals here
            Lighting.AddLight(Projectile.Center, Color.White.ToVector3() * 0.78f);
        }


        public void DrawPixelPrimitives(SpriteBatch spriteBatch)
        {
            BeamDrawer ??= new PrimitiveTrail(WidthFunction, ColorFunction, null, true);
            BeamDrawer.DrawPixelated(Projectile.oldPos, -Main.screenPosition, Projectile.oldPos.Length);
            Main.spriteBatch.ExitShaderRegion();
        }
    }
}
