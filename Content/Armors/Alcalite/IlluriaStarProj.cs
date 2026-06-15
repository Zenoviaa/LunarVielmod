using Stellamod.Assets;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Armors.Alcalite
{
    public abstract class IlluriaStarProj : ModProjectile
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
            if (Timer % 16 == 0)
            {
                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.scaleRange *= 0.5f;
                spawnParams.gravity = 0;
                spawnParams.outerColor = StarColor;
                DustParticle ds = DustParticle.Spawn(Projectile.Center, Vector2.Zero, spawnParams);
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
            if (Timer == 0)
            {
                Projectile.scale = 0.0001f;
            }
            Projectile.scale = MathHelper.Lerp(Projectile.scale, 1f, 0.2f);
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
            if (Projectile.timeLeft < 90)
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
            for (float f = 0; f < 4; f++)
            {
                Vector2 fireVelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero);
                fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                fireVelocity *= Main.rand.NextFloat(3f, 8f);

                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = StarColor;
                spawnParams.scaleRange *= 0.5f;
                DustParticle.Spawn(Projectile.Center, fireVelocity, spawnParams);
            }
        }
    }

    public class IlluriaStarProjBlue : IlluriaStarProj
    {
        public override Color StarColor => ColorFunctions.Niivin;
    }


    public class IlluriaStarProjYellow : IlluriaStarProj
    {
        public override Color StarColor => Color.Yellow;
    }


    public class IlluriaStarProjCyan : IlluriaStarProj
    {
        public override Color StarColor => Color.LightCyan;
    }
}
