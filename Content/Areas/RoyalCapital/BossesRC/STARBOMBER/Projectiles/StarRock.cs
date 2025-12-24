using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class StarRock : ScarletProjectile,
        IDrawOutlines
    {
        private Vector2 _squish;
        private ref float Timer => ref Projectile.ai[0];
        private ref float BounceCount => ref Projectile.ai[1];
        private ref float KillTimer => ref Projectile.ai[2];
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.projFrames[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 32;
            Projectile.width = 32;
            Projectile.height = 32;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.tileCollide = true;
            Projectile.timeLeft = 300;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                _squish = new Vector2(0.5f, 1.5f);
                Projectile.frame = Main.rand.Next(0, Main.projFrames[Type]);
            }

            if (Timer % 5 == 0)
            {
                Dust.NewDust(Projectile.position, Projectile.width, Projectile.height,
                    ModContent.DustType<GlowSparkleDust>(), Scale: Main.rand.NextFloat(0.5f, 1f));
            }

            _squish = Vector2.Lerp(_squish, Vector2.One, 0.1f);

            //Gravity
            if (Projectile.velocity.Y < 12)
            {
                Projectile.velocity.Y += 0.25f;
            }
            if (BounceCount >= 1f)
            {
                KillTimer++;
                if(KillTimer >= 60f)
                {
                    Projectile.Kill();
                }
    
            }
            Projectile.velocity.X *= 0.94f;
            Projectile.rotation += Projectile.velocity.Length() * 0.0125f;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for(float f = 0; f  <16f; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                Dust.NewDustPerfect(Projectile.Center, DustID.Stone, vel, Scale: Main.rand.NextFloat(0.75f, 1.5f));
            }
            
            for(float f = 0; f < 8f; f++)
            {
                Vector2 vel = Main.rand.NextVector2Circular(4, 4);
                LegacyParticle.NewParticle<BlackSmokeParticle>(Projectile.Center, vel);

            }
            FXUtil.GlowCircleBoom(Projectile.Center, Color.White, Color.LightGray, Color.Black);

        }
        public override bool OnTileCollide(Vector2 oldVelocity)
        {
         
            if(Projectile.velocity.Y != oldVelocity.Y)
            {
                _squish = new Vector2(1.5f, 0.7f);
                SoundStyle rockSound = SoundID.DD2_MonkStaffGroundImpact;
                SoundEngine.PlaySound(rockSound, Projectile.position);
                Projectile.velocity.Y = -oldVelocity.Y * 0.6f;
                BounceCount++;
            }

            return false;
        }
        public override bool PreDraw(ref Color lightColor)
        {
            Rectangle drawFrame = Projectile.Frame();
            DrawAfterImageEffect(Main.spriteBatch, ModContent.Request<Texture2D>(Texture).Value, drawFrame, drawFrame.Size() / 2f, _squish, SpriteEffects.None, Color.White, 0.1f);
            this.DrawCentered(ref lightColor, _squish);
            return false;
        }

        public void DrawOutlines(SpriteBatch spriteBatch, Vector2 screenPos, Color lightColor)
        {
            this.OutlineNoRestart(Color.Red, ref lightColor, _squish);
        }
    }
}
