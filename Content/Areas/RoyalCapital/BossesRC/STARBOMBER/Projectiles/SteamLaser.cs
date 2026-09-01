using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Assets;
using Stellamod.Common.Particles;
using Stellamod.Common.Shaders;
using Stellamod.Content.Dusts;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.RoyalCapital.BossesRC.STARBOMBER.Projectiles
{
    public class SteamLaser : ModProjectile
    {
        private Vector2 _startPoint;
        private Vector2[] _laserPoints;
        private Vector2[] LaserPoints
        {
            get
            {
                _laserPoints ??= new Vector2[64];
                for(int  i = 0; i < _laserPoints.Length; i++)
                {
                    float f = i;
                    float numPoints = _laserPoints.Length;
                    float completionRatio = f / numPoints;
                    Vector2 point = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, completionRatio);
                    _laserPoints[i] = point;
                }
                return _laserPoints;
            }
        }

        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 1;
            Projectile.height = 1;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 60;
        }

        private void CreateParticlesMini(Vector2 position)
        {
            for (float f = 0; f < 16; f++)
            {
                Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = position,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Blue.ToVector4(),
                    velocity = Main.rand.NextVector2Circular(14, 14),
                    timeLeft = Main.rand.Next(60, 180)
                });
            }

            for (float f = 0; f < 16; f++)
            {
                Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = position,
                    innerColor = Color.Pink.ToVector4(),
                    outerColor = Color.Purple.ToVector4(),
                    velocity = Main.rand.NextVector2Circular(18, 18),
                    timeLeft = Main.rand.Next(60, 180)
                });
            }

            for (float f = 0; f < 16; f++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = position,
                    innerColor = Color.LightBlue.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    velocity = Main.rand.NextVector2Circular(32, 32),
                    scale = new Vector2(Main.rand.NextFloat(0.5f, 1.5f)),
                    timeLeft = Main.rand.Next(60, 360)
                });
            }
        }

        private void CreateParticles(Vector2 position)
        {
            for (float f = 0; f < 12; f++)
            {
                Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = position,
                    innerColor = Color.White.ToVector4(),
                    outerColor = Color.Blue.ToVector4(),
                    velocity = Main.rand.NextVector2Circular(32, 32),
                    timeLeft = Main.rand.Next(60, 180)
                });
            }

            for (float f = 0; f < 12; f++)
            {
                Particles.BitDust.Spawn(BitDustFactory.SlowingOverTime with
                {
                    position = position,
                    innerColor = Color.Pink.ToVector4(),
                    outerColor = Color.Purple.ToVector4(),
                    velocity = Main.rand.NextVector2Circular(48, 48),
                    timeLeft = Main.rand.Next(60, 180)
                });
            }

            for (float f = 0; f < 32; f++)
            {
                Particles.SwirlingFlameDust.Spawn(BitDustFactory.Default with
                {
                    position = position,
                    innerColor = Color.LightBlue.ToVector4(),
                    outerColor = Color.DarkBlue.ToVector4(),
                    velocity = Main.rand.NextVector2Circular(48, 48),
                    scale = new Vector2(Main.rand.NextFloat(0.5f, 1.2f)),
                    timeLeft = Main.rand.Next(60, 360)
                });
            }
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer >= 30)
            {
                Projectile.hostile = false;
            }
            if (Timer == 1)
            {
                _startPoint = Projectile.Center;
                SoundStyle railgun = AssetRegistry.Sounds.STARBOMBER.STARRAILGUN;
                railgun.PitchVariance = 0.3f;
                SoundEngine.PlaySound(railgun, Projectile.position);


                CreateParticlesMini(Projectile.Center);

                Vector2 direction = Projectile.velocity.SafeNormalize(Vector2.Zero);
                Vector2 explosionCenter = Projectile.Center + Projectile.velocity;
                FXUtil.ShakeCamera(Projectile.Center, 512, 8);
                CreateParticles(explosionCenter);
                if (Main.myPlayer == Projectile.owner)
                {
                    Projectile.NewProjectile(Projectile.GetSource_FromThis(), explosionCenter, Vector2.Zero, ModContent.ProjectileType<SiriusBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
                }


                ShakeScreenPosition.Shake = 3;
                SoundEngine.PlaySound(SoundID.DD2_ExplosiveTrapExplode, explosionCenter);
    

                SoundStyle morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/MorrowExp");
                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, explosionCenter);

                switch (Main.rand.Next(3))
                {
                    case 0:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
                        break;
                    case 1:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower1");
                        break;
                    case 2:
                        morrowExp = new SoundStyle($"Stellamod/Assets/Sounds/StarFlower3");
                        break;
                }

                morrowExp.PitchVariance = 0.3f;
                SoundEngine.PlaySound(morrowExp, explosionCenter);

                FXUtil.ShakeCamera(explosionCenter, 1024, 24);
                var b = FXUtil.GlowCircleBoom(explosionCenter,
                    innerColor: Color.White,
                    glowColor: Color.Pink,
                    outerGlowColor: Color.Blue, duration: 60, baseSize: 0.24f);
                b.Scale *= 2.4f;
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(explosionCenter,
                        innerColor: Color.White,
                        glowColor: Color.Pink,
                        outerGlowColor: Color.Blue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }

                for (float f = 0; f < 24; f++)
                {
                    float progress = f / 24f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 velocity = rot.ToRotationVector2() * Main.rand.NextFloat(4f, 64f);
                    var particle = FXUtil.GlowStretch(explosionCenter, velocity);
                    particle.InnerColor = Color.White;
                    particle.GlowColor = Color.HotPink;
                    particle.OuterGlowColor = Color.Black;
                    particle.Duration = Main.rand.NextFloat(25, 50);
                    particle.BaseSize = Main.rand.NextFloat(0.09f, 0.18f);
                    particle.VectorScale *= 0.5f;

                }
                for (int i = 0; i < 8; i++)
                {
                    Dust.NewDustPerfect(explosionCenter, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DarkGray, 1f).noGravity = true;
                }
            }
         
      
        }
        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(LaserPoints, projHitbox, targetHitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }
        

        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.DarkGray, Color.Black, completionRatio);
        }
        private float EaseOut => MathHelper.SmoothStep(1f, 0f, EasingFunction.OutSine(Timer / 60f));
        private float WidthFunction(float completionRatio)
        {
            return 32 * EaseOut;
        }
        private void DrawPixelatedBeam(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.Pink;
            shader.InnerColor = Color.Lerp(Color.LightPink, Color.Blue, 0.75f);
            shader.OuterColor = Color.Violet;
            shader.LaserTexture = TrailRegistry.BeamTrail;
            shader.BloomTexture = TrailRegistry.CrystalTrail;

            TrailDrawer.Draw(Main.spriteBatch, LaserPoints, ColorFunction, WidthFunction, shader, Projectile.Size / 2f);
        }

        public void DrawPixelatedMuzzleFlash(SpriteBatch spriteBatch, Vector2 screenPos)
        {
            Asset<Texture2D> muzzleFlashTexture = ModContent.Request<Texture2D>("Stellamod/Assets/LaserTextures/MuzzleFlash");
            Vector2 drawOrigin = muzzleFlashTexture.Size() / 2f;
            Vector2 drawCenter = Projectile.Center - screenPos;
            Color drawColor = Color.Pink;
            drawColor.A = 0;

            float outWidth = EaseOut;
            float scale = outWidth;
            Vector2 flashScale = Vector2.One;
            flashScale.X *= 1.5f;
            flashScale.Y *= 1.2f;
            flashScale *= scale;
            spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale, SpriteEffects.None, 0);

            drawColor = Color.White;
            drawColor.A = 0;
            spriteBatch.Draw(muzzleFlashTexture.Value, drawCenter, null, drawColor, Projectile.velocity.ToRotation(), drawOrigin, flashScale * 0.6f, SpriteEffects.None, 0);

            Asset<Texture2D> impactTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/ZuiEffect");
            drawOrigin = impactTexture.Size() / 2f;

            Vector2 impactPoint = _startPoint;
            scale *= ExtraMath.Osc(0.66f, 1f, speed: 32);

            drawCenter = impactPoint - screenPos;
            drawColor = Color.Pink;
            drawColor.A = 0;

            float rot = Main.GlobalTimeWrappedHourly;
            spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, rot, drawOrigin, scale * 1.2f, SpriteEffects.None, 0);

            drawColor = Color.White;
            drawColor.A = 0;
            spriteBatch.Draw(impactTexture.Value, drawCenter, null, drawColor, rot, drawOrigin, scale * 0.8f, SpriteEffects.None, 0);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawPixelatedMuzzleFlash);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelatedBeam);
            return false;
        }
    }
}
