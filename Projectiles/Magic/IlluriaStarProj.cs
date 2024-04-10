using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
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
    internal abstract class IlluriaStarProj : ModProjectile
    {
        const float Lifetime = 180;
        public virtual Color StarColor { get; }


        ref float Timer => ref Projectile.ai[0];
        ref float OrbitDistance => ref Projectile.ai[1];
        ref float OrbitOffset => ref Projectile.ai[2];
        float WhiteTimer;

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 8;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 18;
            Projectile.height = 18;
            Projectile.friendly = true;
            Projectile.hostile = false;
            Projectile.penetrate = 2;
            Projectile.timeLeft = (int)Lifetime;
            Projectile.tileCollide = false;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = 15;
        }

        private void AI_Movement(Vector2 targetCenter, float moveSpeed, float accel = 1f)
        {
            //This code should give quite interesting movement
            //Accelerate to being on top of the player
            if (Projectile.Center.X < targetCenter.X && Projectile.velocity.X < moveSpeed)
            {
                Projectile.velocity.X += accel;
            }
            else if (Projectile.Center.X > targetCenter.X && Projectile.velocity.X > -moveSpeed)
            {
                Projectile.velocity.X -= accel;
            }

            //Accelerate to being above the player.
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
            if(Projectile.timeLeft  == Lifetime / 2)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, StarColor, 0.5f);
            }

            if (Projectile.timeLeft > Lifetime / 2)
            {
                WhiteTimer++;
                Orbit();
            }
            else
            {
                NPC npc = NPCHelper.FindClosestNPC(Projectile.position, 700);
                if (npc != null)
                {
                    if (Main.rand.NextBool(16))
                    {
                        Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                        ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, StarColor, 0.5f);
                    }

                    AI_Movement(npc.Center, 15);
                }
                else
                {
                    Orbit();
                }
            }

            Projectile.rotation += (1f - (Projectile.timeLeft / Lifetime)) * 0.05f;
        }

        private void Orbit()
        {
            Timer++;
            if (Timer == 1 && Main.myPlayer == Projectile.owner)
            {
                OrbitDistance = Main.rand.Next(32, 64);
                OrbitOffset = Main.rand.NextFloat(0, MathHelper.TwoPi);
                Projectile.netUpdate = true;
            }

            Player owner = Main.player[Projectile.owner];
            Vector2 startOrbit = Vector2.UnitY.RotatedBy(OrbitOffset);
            Vector2 orbitCenter = MovementHelper.OrbitAround(owner.Center, startOrbit, OrbitDistance, Timer * 0.01f);
            Vector2 targetVelocity = VectorHelper.VelocitySlowdownTo(Projectile.Center, orbitCenter, 8);
            Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.9f);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(StarColor, Color.Transparent, completionRatio);
        }

        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width * 0.5f;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        //Visual Stuffs
        public override bool PreDraw(ref Color lightColor)
        {
            if(Projectile.timeLeft < 90)
            {
                DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.CausticTrail);
                DrawHelper.DrawAdditiveAfterImage(Projectile, StarColor, Color.Transparent, ref lightColor);
            }

            return true;
        }

        public override void PostDraw(Color lightColor)
        {
            string glowTexture = "Stellamod/Projectiles/Magic/IlluriaStarProjWhite";
            Texture2D whiteTexture = ModContent.Request<Texture2D>(glowTexture).Value;

            Vector2 textureSize = new Vector2(18, 18);
            Vector2 drawOrigin = textureSize / 2;

            //Lerping
            float whiteTime = (Lifetime / 2);
            float progress = WhiteTimer / whiteTime;
            Color drawColor = Color.Lerp(Color.White, Color.Transparent, progress);
            Vector2 drawPosition = Projectile.position - Main.screenPosition + drawOrigin;


            Main.spriteBatch.Draw(whiteTexture, drawPosition, null, drawColor, Projectile.rotation, drawOrigin, Projectile.scale, SpriteEffects.None, 0f);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 4; i++)
            {
                Vector2 speed = Main.rand.NextVector2CircularEdge(4f, 4f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, speed, StarColor, 0.5f);
            }
        }
    }

    internal class IlluriaStarProjBlue : IlluriaStarProj
    {
        public override Color StarColor => ColorFunctions.Niivin;
    }


    internal class IlluriaStarProjYellow : IlluriaStarProj
    {
        public override Color StarColor => Color.Yellow;
    }


    internal class IlluriaStarProjCyan : IlluriaStarProj
    {
        public override Color StarColor => Color.LightCyan;
    }
}
