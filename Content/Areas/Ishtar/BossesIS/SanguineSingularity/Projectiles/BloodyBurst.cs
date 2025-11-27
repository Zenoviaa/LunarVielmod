using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;

/*

- Deer with a singularity for a head, in its spawn animation at first it looks like a normal deer before the head explodes and parts start orbiting it, ooo I know exactly how to code this

- The legs and everything are rigged, we’ll use forward kinematics to animate the boss, so we’ll have to make a run animation and idle animation

- Opens the fight with several exploding blood magic projectiles that loosely track the player

- Winds up a charge and then runs directly at the player really fast, and explodes into bloody bits before merging itself back together elsewhere

- Runs up into the sky and rains down acidic blood

- Walks slowly around the player as bloody boils explode from its body and then home back towards you

- Cracks form in its body and it violently erupts into multiple bloody geysers

- Winds up a charge and then keeps running at you while swerving around and trying to juke you out
 
- In phase 2 every attack gets more deadlier, triggers at under 50% health
 */
namespace Stellamod.Content.Areas.Ishtar.BossesIS.SanguineSingularity.Projectiles
{
    public class BloodyBurst : ScarletProjectile,
        IDrawSanguineBlood
    {
        private float _trailWidth;
        public override string Texture => TextureRegistry.EmptyTexture;

        private ref float Timer => ref Projectile.ai[0];
        private ref float Version => ref Projectile.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 80;
            Projectile.width = 16;
            Projectile.height = 16;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.hostile = true;
            Projectile.friendly = false;

            Projectile.extraUpdates = 1;
        }

        private void AI_Gravity()
        {
            if (Projectile.velocity.Y < 25)
            {
                Projectile.velocity.Y += 0.25f;
            }


        }

        private void AI_Homing()
        {
            Projectile.extraUpdates = 2;
            var closest = PlayerHelper.FindClosestPlayer(Projectile.position, 2048);
            if(closest != null)
            {
                if(Timer < 100)
                {
                    float degreesToRotate = MathHelper.Lerp(0.1f, 6f, EasingFunction.InOutSine(Timer / 100f));
                    Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(Projectile, closest.Center, degreesToRotate);
                    Projectile.velocity = homingVelocity;
                }
               
            }
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            switch (Version)
            {
                case 0:
                    AI_Gravity();
                    break;
                case 1:
                    AI_Homing();
                    break;
            }
            if (Timer >= 240f)
            {
                _trailWidth = MathHelper.Lerp(_trailWidth, 0f, 0.1f);
                Projectile.velocity *= 0.9f;
                if (Projectile.velocity.Length() <= 1f)
                {
                    Projectile.Kill();
                }
            }
            else
            {
                _trailWidth = MathHelper.Lerp(_trailWidth, 1f, 0.1f);
            }

            if (Timer % 7 == 0)
            {
                Particle.NewBlackParticle<BloodSparkleParticle>(Projectile.Center, Vector2.Zero, Color.Red, Scale: Main.rand.NextFloat(0.5f, 1f));
            }
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);

        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.White;
        }

        private float WidthFunction(float completionRatio)
        {
            return MathHelper.SmoothStep(56, 0, completionRatio) * _trailWidth * MathF.Sin(completionRatio * 4);
        }

        public void DrawToSanguineMask(SpriteBatch spriteBatch)
        {
            var flamingTrailShader = BasicLaserAlphaShader.Instance;
            flamingTrailShader.Tiling = Vector2.One * 1;
            flamingTrailShader.LaserTexture = TrailRegistry.LightningTrail2;
            flamingTrailShader.BlendState = BlendState.AlphaBlend;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, flamingTrailShader);
       //     spriteBatch.Draw(ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value, Projectile.Center - Main.screenPosition, Color.White);
        }
    }
}
