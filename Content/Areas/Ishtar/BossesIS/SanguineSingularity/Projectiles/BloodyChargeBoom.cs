using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Content.Dusts;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.ModLoader;

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
    public class BloodyChargeBoom : ModProjectile,
        IDrawSanguineBlood
    {
        private ref float Timer => ref Projectile.ai[0];
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.friendly = false;
            Projectile.penetrate = -1;
            Projectile.tileCollide = false;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 30;
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                var p = FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkBlue, Color.Black);
                p.Scale *= 3f;

                var p2 = FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkBlue, Color.Black);
                p2.Scale *= 2f;

                ShakeScreenPosition.Shake = 16;
                FXUtil.ShakeCamera(Projectile.Center, 1024, 8);
                for (float f = 0; f < 16f; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(24, 24);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity,
                        newColor: Color.Red,
                        Scale: Main.rand.NextFloat(0.5f, 3f));
                }

                for (float f = 0; f < Main.rand.NextFloat(10f, 16f); f++)
                {
                    Color color = Color.White;
                    Vector2 velocity = Main.rand.NextVector2Circular(24, 24);
                    LegacyParticle.NewBlackParticle<BloodSparkleParticle>(Projectile.Center, velocity, color, Scale: Main.rand.NextFloat(0.5f, 3f));
                }
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        public void DrawToSanguineMask(SpriteBatch spriteBatch)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            float drawScale = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / 30f));
            spriteBatch.Draw(texture, drawCenter, null, Color.White, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
        }
    }
}
