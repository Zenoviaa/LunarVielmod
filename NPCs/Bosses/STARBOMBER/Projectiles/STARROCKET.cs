using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Particles;
using Stellamod.Trails;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Bosses.STARBOMBER.Projectiles
{
    internal class STARROCKET : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        private Vector2 InitialVelocity;
        public override void SetStaticDefaults()
        {
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.light = 0.75f;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            Timer++;
            if(Timer == 1)
            {
                InitialVelocity = Projectile.velocity;
            }

            if(Timer % 8 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height, DustID.Firework_Pink);
            }

            float maxDetectDistance = 2000;
            Player closestPlayer = PlayerHelper.FindClosestPlayer(Projectile.position, maxDetectDistance);
            if(closestPlayer != null && Timer < 150)
            {
                Vector2 targetDirection = Projectile.Center.DirectionTo(closestPlayer.Center);
                Vector2 targetVelocity = targetDirection * InitialVelocity.Length();
                Vector2 homingVelocity = Vector2.Lerp(Projectile.velocity, targetVelocity, 0.1f);
                Projectile.velocity = homingVelocity;
            }

            Projectile.rotation = Projectile.velocity.ToRotation();
        }
        
        public float WidthFunction(float completionRatio)
        {
            float baseWidth = Projectile.scale * Projectile.width;
            return MathHelper.SmoothStep(baseWidth, 3.5f, completionRatio);
        }

        public Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Pink * 0.3f, Color.Transparent, completionRatio);
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawHelper.DrawSimpleTrail(Projectile, WidthFunction, ColorFunction, TrailRegistry.StarTrail, 
                frameSize: new Vector2(32, 22));
            return base.PreDraw(ref lightColor);
        }

        public override void OnKill(int timeLeft)
        {
            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.OrangeRed, 1f).noGravity = true;
            }

            for (int i = 0; i < 14; i++)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
            }

            for (int i = 0; i < 8; i++)
            {
                //Get a random velocity
                Vector2 velocity = Main.rand.NextVector2Circular(4, 4);

                //Get a random
                float randScale = Main.rand.NextFloat(0.5f, 1.5f);
                ParticleManager.NewParticle<StarParticle2>(Projectile.Center, velocity, Color.White, randScale);
            }


            SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, Projectile.position);
            Main.LocalPlayer.GetModPlayer<MyPlayer>().ShakeAtPosition(Projectile.Center, 1024f, 32f);
        }
    }
}
