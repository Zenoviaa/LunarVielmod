using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Gores;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class StarBeam : ScarletProjectile
    {
        private Vector2 _startPoint;
        private Vector2 _impactPoint;
        private bool _impactGround;
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 384;
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.hostile = true;
            Projectile.tileCollide = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180*4;
            Projectile.extraUpdates = 4;
        }

        public override void AI()
        {
            Timer++;

            if (Timer < TrailCacheLength)
            {
                base.AI();
            } else
            {
                Projectile.velocity *= 0f;
            }


            if(_impactPoint != Vector2.Zero)
            {
                if (Timer % 2 == 0)
                {
                    var part = FXUtil.GlowCircleDetailedBoom1(_impactPoint, Color.Yellow, Color.Orange, Color.DarkRed);
                    part.Scale *= 0.5f;
                    part.Rotation = Main.rand.NextFloat(-1f, 1f);
                }
            }
                ProjectileID.Sets.DrawScreenCheckFluff[Type] = 10000;
            if (Timer == 1)
            {
                _startPoint = Projectile.Center;
                SoundStyle railgun = AssetRegistry.Sounds.STARBOMBER.STARRAILGUN;
                railgun.PitchVariance = 0.3f;
                SoundEngine.PlaySound(railgun, Projectile.position);

                SoundStyle chargeSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_LAZER");
                SoundEngine.PlaySound(chargeSound, Projectile.position);

                FXUtil.ShakeCamera(Projectile.position, 1024, 18);
                FXUtil.GlowCircleBoom(Projectile.Center, Color.Pink, Color.Purple, Color.Black);
                for(float f = 0;f < 8; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink);
                }
                for (float f = 0; f < 8; f++)
                {
                    Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), velocity, newColor: Color.Pink);
                }
            }

        }

        private void ImpactEffect()
        {
            ShakeModSystem.Shake = 9;
            FXUtil.ShakeCamera(Projectile.position, 1024, 32);
            FXUtil.GlowCircleBoom(Projectile.Center, Color.Pink, Color.Purple, Color.Black);
            FXUtil.PunchCamera(Projectile.Center, Vector2.UnitY, 8, 8, 8);
            for (float f = 0; f < 8; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Pink);
            }
            for (float f = 0; f < 8; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowSparkleDust>(), velocity, newColor: Color.Pink);
            }
            for (float f = 0; f < 8; f++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Particle.NewParticle<ZapParticle>(Projectile.Center, velocity, Color.Pink);
            }

            for (int i = 0; i < 1; i++)
            {
                var source = Projectile.GetSource_FromThis();
                Vector2 rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));
                rvelocity *= 2;

                Gore.NewGore(source, Projectile.Center, rvelocity,
                    ModContent.GoreType<FableRock1>());

                rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                Gore.NewGore(source, Projectile.Center, rvelocity,
                    ModContent.GoreType<FableRock2>());

                rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                Gore.NewGore(source, Projectile.Center, rvelocity,
                    ModContent.GoreType<FableRock3>());

                rvelocity = -Vector2.UnitY * Main.rand.NextFloat(4, 8);
                rvelocity = rvelocity.RotatedByRandom(MathHelper.ToRadians(24));

                Gore.NewGore(source, Projectile.Center, rvelocity,
                    ModContent.GoreType<FableRock4>());
            }
            var sear = Particle.NewParticle<SearParticle>(Projectile.Center, Vector2.Zero);


            SoundStyle crush = AssetRegistry.Sounds.STARBOMBER.HeavyCrush;
            crush.PitchVariance = 0.3f;
            SoundEngine.PlaySound(crush, Projectile.position);


            var p = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.UnitY);
            p.Scale *= 5;
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (!_impactGround)
            {
                _impactPoint = Projectile.Center;
                ImpactEffect();
                     _impactGround = true;
            }
            Projectile.velocity.X = oldVelocity.X;
            Projectile.velocity.Y = 0;
            return false;
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(OldCenterPos, projHitbox, targetHitbox);
        }

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.White, Color.Black, 0.5f);
        }
        private float WidthFunction(float completionRatio)
        {
            float inEasing = EasingFunction.OutExpo(Timer / 60f);
            float outEasing = (float)Projectile.timeLeft / 60f;
            outEasing = EasingFunction.InOutSine(outEasing);
            return 80 * inEasing * outEasing;
        }
        private void DrawLaser()
        {
            SpriteBatch spriteBatch = Main.spriteBatch;
            BlackFireShader shader = BlackFireShader.Instance;
            shader.PrimaryTexture = TrailRegistry.DNATrail;
            shader.PrimaryTexture2 = TrailRegistry.StarTrail;
            shader.InnerColor = Color.Pink;
            shader.OuterColor = Color.Purple;
            shader.Distortion = 0;
            shader.Time = -Timer * 0.1f;
            TrailDrawer.Draw(spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
    //        TrailDrawer.Draw(spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);

        }

        private void DrawEndPoint()
        {
            Texture2D endPoint = ModContent.Request<Texture2D>(TextureRegistry.ZuiEffect).Value;
            Vector2 drawOrigin = endPoint.Size() / 2f;
            Vector2 drawPosition = Projectile.Center - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Color glowColor = Color.Pink;
            glowColor.A = 0;
            float outEasing = (float)Projectile.timeLeft / 60f;
            outEasing = EasingFunction.InOutSine(outEasing);
            for (float f = 0; f < 4; f++)
            {
                spriteBatch.Draw(endPoint, drawPosition, null, glowColor, f / 4f * MathHelper.TwoPi, drawOrigin, ExtraMath.Osc(0.5f, 1f, speed: 32, offset: f) * outEasing, SpriteEffects.None, 0);
                spriteBatch.Draw(endPoint, _startPoint - Main.screenPosition, null, glowColor, f / 4f * MathHelper.TwoPi, drawOrigin, ExtraMath.Osc(0.5f, 1f, speed: 32, offset: f) * 0.5f * outEasing, SpriteEffects.None, 0);
            }

        }

        public override bool PreDraw(ref Color lightColor)
        {
            DrawLaser();
            DrawEndPoint();
            return false;
        }
    }
}
