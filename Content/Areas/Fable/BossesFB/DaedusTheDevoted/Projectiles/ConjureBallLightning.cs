using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted.Projectiles
{
    public class ConjureBallLightning : ModProjectile
    {
        private float _scale;
        private float _width;
        private float _stretchX;
        private float _originalSpeed;
        private bool _canDie;
        private Vector2 _zapVelocity;

        private ref float Timer => ref Projectile.ai[0];
        private ref float Charge => ref Projectile.ai[1];

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            ProjectileID.Sets.TrailCacheLength[Type] = 16;
            ProjectileID.Sets.TrailingMode[Type] = 2;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            _width = 1;
            Projectile.width = 49;
            Projectile.height = 49;
            Projectile.friendly = false;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.usesIDStaticNPCImmunity = true;
            Projectile.idStaticNPCHitCooldown = 8;
            Projectile.timeLeft = 600;
            Projectile.light = 0.48f;
            Projectile.tileCollide = false;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            Color drawColor = Color.White;
            float drawRotation = Projectile.rotation;
            float drawScale = _scale;
            Vector2 scale = Vector2.One * drawScale;
            scale.X *= _stretchX;
            SpriteBatch spriteBatch = Main.spriteBatch;
            var shader = TeslaOrbShader.Instance;
            if (Main.rand.NextBool(3))
            {
 
            }
            Vector2 drawPos = Projectile.Center - Main.screenPosition;
            spriteBatch.Restart(blendState: BlendState.Additive, effect: shader.Effect);
     
            spriteBatch.Draw(texture, drawPos, null, drawColor, Projectile.rotation, drawOrigin, scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void AI()
        {
            base.AI();
            if(_originalSpeed == 0)
            {
                _originalSpeed = Projectile.velocity.Length();
            }
            if (Collision.CanHitLine(Projectile.Top, 1, 1, Projectile.Top + Vector2.UnitY * 50, 1, 1))
            {
                _canDie = true;
            }

            if (_zapVelocity != Vector2.Zero)
            {
                Projectile.velocity += _zapVelocity;
                _zapVelocity *= 0.5f;
            }

            if(Projectile.velocity.Length() >= _originalSpeed)
            {
                Projectile.velocity *= 0.8f;
            }
          //  Projectile.velocity.Y += MathF.Sin(Timer * 0.05f) * 0.1f;
            if(Timer % 60 == 0)
            {
                _zapVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero) * 25;
                _zapVelocity = _zapVelocity.RotatedByRandom(0.3f);
                SoundStyle zap = SoundID.DD2_LightningBugZap;
                zap.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zap, Projectile.position);

                FXUtil.GlowCircleBoom(Projectile.Center,
                                  innerColor: Color.White,
                                  glowColor: Color.Yellow,
                                  outerGlowColor: Color.Blue, duration: 7, baseSize: 0.15f);
            }
            float targetX = MathHelper.Lerp(1f, 2f, _zapVelocity.Length() / 10f);
            _stretchX = MathHelper.Lerp(_stretchX, targetX, 0.1f);
            Projectile.rotation = Projectile.velocity.ToRotation();
            Projectile.tileCollide = _canDie;
            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Wave");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, Projectile.position);
            }

            if (Timer % 6 == 0)
            {
                for (float f = 0; f < 1; f++)
                {
                    Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<SparkParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }
            }

            if(Timer % 4 == 0)
            {
                Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = Particle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.Scale *= 0.5f;
                spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
            }
            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center, DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer <= 15)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(0.5f, 1f) + MathHelper.Lerp(0.1f, 0.3f, Charge), Easing.InCubic(Timer / 15f));
            }

            if (Timer >= 90)
            {
                Projectile.tileCollide = true;
            }

          
        }

        public override bool OnTileCollide(Vector2 oldVelocity)
        {
            if (Projectile.velocity.X != oldVelocity.X)
                Projectile.velocity.X = -oldVelocity.X;
            if (Projectile.velocity.Y != oldVelocity.Y)
                Projectile.velocity.Y = -oldVelocity.Y;

            return false;
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            SoundStyle zapSound = SoundID.DD2_LightningBugZap;
            zapSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(zapSound, target.Center);
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            for (int i = 0; i < 16; i++)
            {
                float progress = i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust d = Dust.NewDustPerfect(Projectile.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }


            var part = FXUtil.GlowCircleBoom(Projectile.Center,
                innerColor: Color.White,
                glowColor: Color.Yellow,
                outerGlowColor: Color.Blue, duration: 12, baseSize: 0.14f);
            part.Scale *= 2;
            for (float f = 0; f < 32; f++)
            {
                Dust.NewDustPerfect(Projectile.Center, DustID.Torch,
                    (Vector2.One * Main.rand.NextFloat(0.2f, 5f)).RotatedByRandom(19.0), 0, Color.White, Main.rand.NextFloat(1f, 3f)).noGravity = true;
            }


            for (float i = 0; i < 15; i++)
            {
                float rot = rot = -Projectile.oldVelocity.ToRotation();
                rot += Main.rand.NextFloat(-0.5f, 0.5f);

                Vector2 offset = rot.ToRotationVector2() * Main.rand.NextFloat(32, 64);
                Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(2, 15);
                var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center + offset,
                    innerColor: Color.White,
                    glowColor: Color.Yellow,
                    outerGlowColor: Color.Blue,
                    baseSize: Main.rand.NextFloat(0.03f, 0.1f),
                    duration: Main.rand.NextFloat(5, 25));
                particle.Velocity = velocity;
                particle.Scale *= 0.35f;
                particle.Rotation = rot;
            }


            //EXPLODE
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Projectile.velocity.SafeNormalize(Vector2.Zero),
                ModContent.ProjectileType<ConjureBallExplosion>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
