using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core.Particles;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Tundra.Abyss.BossesAB.VerlianSingularity.Projectiles
{
    public class BlackLightningBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.penetrate = -1;
            Projectile.usesLocalNPCImmunity = true;
            Projectile.localNPCHitCooldown = -1;
            Projectile.hostile = true;
            Projectile.timeLeft = 30;
            Projectile.tileCollide = false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.DeepSkyBlue, 1f).noGravity = true;
                }
                for (int i = 0; i < 7; i++)
                {
                    Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<TSmokeDust>(), (Vector2.One * Main.rand.Next(1, 5)).RotatedByRandom(19.0), 0, Color.LightSkyBlue, 1f).noGravity = true;
                }

                FXUtil.ShakeCamera(Projectile.Center, 1024, 32);
                float boomSize = Main.rand.NextFloat(0.045f, 0.08f);
                var part = FXUtil.GlowCircleBoom(Projectile.Center,
                    innerColor: Color.White,
                    glowColor: Color.LightBlue,
                    outerGlowColor: Color.Blue, duration: 25, baseSize: boomSize);
                part.Scale *= 3;
                for (float i = 0; i < 4; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleLongBoom(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.LightBlue,
                        outerGlowColor: Color.DarkBlue,
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                }
            }
        }
    }

    public class BlackLightning : VSProjectile
    {
        private ZappingTrail _lightningTrail;
        public override string Texture => TextureRegistry.EmptyTexture;
        private ref float BurstCounter => ref Projectile.ai[2];
        private Vector2 _targetCenter;
        private Vector2 _zapCenter;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 60;
            Projectile.hostile = false;
            Projectile.extraUpdates = 3;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_targetCenter);
            writer.WriteVector2(_zapCenter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _targetCenter = reader.ReadVector2();
            _zapCenter = reader.ReadVector2();
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if (Timer == 1)
            {
                SoundStyle summonSound = new SoundStyle("Stellamod/Assets/Sounds/SoftSummon");
                summonSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(summonSound, Projectile.position);

                SoundStyle zapSound = SoundID.DD2_LightningAuraZap;
                zapSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(zapSound, Projectile.position);

                var part = LegacyParticle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part.Scale *= 4;
                part.shrink = true;
                part.noStretch = true;
            }

            if (Timer == 1 && this.OwnedByLocalClient())
            {
                NPC parent = GetParentNPC();
                float distance = Main.rand.NextFloat(128, 1000);
                _targetCenter = parent.Center + Main.rand.NextVector2CircularEdge(distance, distance);
                Projectile.netUpdate = true;
            }

            if (Timer > 5 && Timer % 15 == 0)
            {
                if (this.OwnedByLocalClient())
                {
                    Vector2 vectorToTarget = (_targetCenter - Projectile.Center);
                    vectorToTarget = vectorToTarget.RotatedByRandom(MathHelper.ToRadians(180));
                    vectorToTarget *= 0.25f;
                    _zapCenter = Projectile.Center + vectorToTarget;
                    Projectile.netUpdate = true;
                }
                BurstCounter++;
            }

            if(Timer % 12 == 0)
            {
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlyphDust>(), Vector2.Zero, 
                    newColor: Color.White, 
                    Scale: Main.rand.NextFloat(0.15f, 0.5f));
            }

            //After a while we want the projectile to zoom to the target position;
            //This will be cool with the zapping around effect
            if (BurstCounter >= 12)
            {
                Vector2 targetVelocity = (_targetCenter - Projectile.Center) * 0.5f;
                Projectile.velocity = targetVelocity;
                float distanceToTarget = Vector2.Distance(Projectile.Center, _targetCenter);
                if (distanceToTarget <= 16)
                {
                    Projectile.Kill();
                }
            }
            else if (_zapCenter != Vector2.Zero)
            {
                Vector2 targetVelocity = (_zapCenter - Projectile.Center) * 0.5f;
                Projectile.velocity = targetVelocity;
                Projectile.rotation = Projectile.velocity.ToRotation();
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
            Vector2 telegraphPosition = _targetCenter - Main.screenPosition;
            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D attackTexture = ModContent.Request<Texture2D>("Stellamod/Assets/NoiseTextures/Extra_67").Value;
            Vector2 drawOrigin = attackTexture.Size() / 2f;
            Color drawColor = Color.Lerp(Color.Yellow, Color.DarkGoldenrod, ExtraMath.Osc(0f, 1f, speed: 32));
            drawColor.A = 0;
            Vector2 drawScale = Vector2.One * 0.5f * EasingFunction.QuadraticBump(BurstCounter / 12f);
            spriteBatch.Draw(attackTexture, telegraphPosition, null, drawColor, 0, drawOrigin, drawScale, SpriteEffects.None, 0);
            return false;
        }

        private float WidthFunction(float completionRatio)
        {
            return EasingFunction.QuadraticBump(completionRatio) * Main.rand.NextFloat(3f, 6f);
        }

        private Color ColorFunction(float p)
        {
            Color trailColor = Color.Lerp(Color.Cyan, Color.Blue, p);
            trailColor = Color.Lerp(trailColor, Color.White, ExtraMath.Osc(0f, 1f, speed: 64));
            trailColor = Color.Lerp(trailColor, Color.Black, ExtraMath.Osc(0f, 1f, speed: 64, offset: 8));
            return trailColor;
        }

        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            Projectile.NewProjectile(Projectile.GetSource_FromThis(), _targetCenter, Vector2.Zero, 
                ModContent.ProjectileType<BlackLightningBoom>(), Projectile.damage, Projectile.knockBack, Projectile.owner);
        }
    }
}
