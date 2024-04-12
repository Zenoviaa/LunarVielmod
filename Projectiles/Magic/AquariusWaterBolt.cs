using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Magic
{
    internal class AquariusWaterBolt : ModProjectile
    {
        private float Timer
        {
            get => Projectile.ai[0];
            set => Projectile.ai[0] = value;
        }

        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 24;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.friendly = true;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 3600;
            Projectile.extraUpdates = 3;
        }

        public override void AI()
        {
            Timer++;

            if (Timer % 6 == 0)
            {
                for (int i = 0; i < 1; i++)
                {
                    Vector2 position = Projectile.Center + Main.rand.NextVector2Circular(2, 2);
                    Vector2 velocity = Main.rand.NextVector2Circular(1, 1);
                    float scale = Main.rand.NextFloat(0.2f, 0.4f);
                    ParticleManager.NewParticle(position, velocity, ParticleManager.NewInstance<BubbleParticle>(),
                        Color.White, scale);
                }
            }

            if(Timer > 90)
            {
                NPC npc = NPCHelper.FindClosestNPC(Projectile.position, 512);
                if(npc != null)
                {
                    Vector2 targetVelocity = (npc.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 4;
                    // If found, change the velocity of the projectile and turn it in the direction of the target
                    // Use the SafeNormalize extension method to avoid NaNs returned by Vector2.Normalize when the vector is zero
                    Projectile.velocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.2f); 
                    Projectile.rotation = Projectile.velocity.ToRotation();
                }
            }
            else
            {
                Projectile.velocity.Y += 0.02f;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 8; i++)
            {
                Vector2 velocity = Main.rand.NextVector2CircularEdge(4f, 4f);
                float scale = Main.rand.NextFloat(0.2f, 0.4f);
                ParticleManager.NewParticle(Projectile.Center, velocity, ParticleManager.NewInstance<BubbleParticle>(),
                    Color.White, scale);
            }
        }


        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(ColorFunctions.Niivin, Color.Black, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.CausticTrail);
            return base.PreDraw(ref lightColor);
        }
    }
}
