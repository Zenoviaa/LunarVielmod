using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Fable.BossesFB.DaedusTheDevoted.Projectiles
{
    public class ElectricSingularity : ModNPC
    {
        private Vector2 _zapVelocity;

        private float _scale;
        private ref float Timer => ref NPC.ai[0];
        private ref float AttackTimer => ref NPC.ai[1];
        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.width = 49;
            NPC.height = 49;
            NPC.lifeMax = 1000;
            NPC.damage = 10;
            NPC.noTileCollide = true;
            NPC.dontTakeDamage = true;
            NPC.noGravity = true;
        }

        public override bool PreDraw(SpriteBatch spriteBatch, Vector2 screenPos, Color drawColor)
        {
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawOrigin = texture.Size() / 2f;
            float drawRotation = NPC.rotation;
            float drawScale = _scale;
            Vector2 stretchScale = Vector2.One;
            stretchScale.X *= 2;
            Vector2 drawPos = NPC.Center - Main.screenPosition;
            SparkyShader sparkyShader = SparkyShader.Instance;
            sparkyShader.InnerColor = Color.Lerp(Color.DarkGoldenrod, Color.White, AttackTimer / 60f);
            sparkyShader.OuterColor = Color.Lerp(Color.DarkRed, Color.Yellow, AttackTimer / 60f);
            spriteBatch.Restart(effect: sparkyShader.Effect, blendState: BlendState.Additive);
            spriteBatch.Draw(texture, drawPos, null, Color.White, NPC.rotation, drawOrigin, drawScale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White * 0.25f, NPC.rotation - Main.GlobalTimeWrappedHourly, drawOrigin, stretchScale * 1.5f * ExtraMath.Osc(0.5f, 1f, speed: 6, offset: 3), SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawPos, null, Color.White * 0.25f, NPC.rotation + Main.GlobalTimeWrappedHourly, drawOrigin, stretchScale * 1.5f * ExtraMath.Osc(0.5f, 1f, speed: 6), SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        public override void AI()
        {
            base.AI();
            NPC.rotation += Main.rand.NextFloat(0.01f, 0.02f);
            NPC.TargetClosest();
            Player target = Main.player[NPC.target];

            AttackTimer++;
            if (AttackTimer % 4 == 0)
            {
                Vector2 dustSpawnPoint = NPC.Center + Main.rand.NextVector2CircularEdge(64, 64);
                Vector2 dustVelocity = (NPC.Center - dustSpawnPoint).SafeNormalize(Vector2.Zero);
                dustVelocity *= 4;
                float progress = AttackTimer / 80f;

                Dust d = Dust.NewDustPerfect(dustSpawnPoint, DustID.GoldCoin, Velocity: dustVelocity, Scale: progress * 1f);
                d.noGravity = true;
            }


            if (AttackTimer >= 60)
            {
                var part = Particle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity * 0.15f);
                part.innerColor = Color.White;
                part.outerColor = Color.Yellow;
                part.fadeToColor = Color.Goldenrod;
                part.Scale *= 0.05f;
                part.Rotation = NPC.velocity.ToRotation();

                _scale *= 0.5f;
                if (target.active)
                {
                    Vector2 velToPlayer = (target.Center - NPC.Center).SafeNormalize(Vector2.Zero);
                    velToPlayer *= 9;
                    if (StellaMultiplayer.IsHost)
                    {
                        Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, velToPlayer,
                            ModContent.ProjectileType<ConjureBallLightningMini>(), 21, 1, Owner: Main.myPlayer);
                    }
                }

                AttackTimer = 0;
            }

            //Some interesting movement code for the singularity
            if (target != null)
            {
                float diffX = target.Center.X - NPC.Center.X;
                NPC.velocity.X = diffX * 0.03f;
            }

            NPC.velocity.Y = MathF.Sin(Timer * 0.05f) * 2;

            Timer++;
            if (Timer == 1)
            {
                SoundStyle soundStyle = new SoundStyle("Stellamod/Assets/Sounds/StormDragon_Enrage");
                soundStyle.PitchVariance = 0.15f;
                SoundEngine.PlaySound(soundStyle, NPC.position);

                //Spawn Dust Circle
                for (int i = 0; i < 32; i++)
                {
                    float progress = i / 32f;
                    float rot = progress * MathHelper.TwoPi;
                    Vector2 vel = rot.ToRotationVector2() * 8;
                    Dust.NewDustPerfect(NPC.Center, DustID.GoldCoin, vel);
                }
            }

            if (Timer % 12 == 0)
            {
                for (float f = 0; f < 1; f++)
                {
                    Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<SparkParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }
            }
            if (Timer % 60 == 0)
            {
                _zapVelocity = NPC.velocity.SafeNormalize(Vector2.Zero) * 25;
                _zapVelocity = _zapVelocity.RotatedByRandom(0.3f);
                SoundStyle zap = SoundID.DD2_LightningBugZap;
                zap.PitchVariance = 0.3f;
                SoundEngine.PlaySound(zap, NPC.position);

                FXUtil.GlowCircleBoom(NPC.Center,
                                  innerColor: Color.White,
                                  glowColor: Color.Yellow,
                                  outerGlowColor: Color.Blue, duration: 7, baseSize: 0.15f);
            }
            if (_zapVelocity != Vector2.Zero)
            {
                NPC.velocity += _zapVelocity;
                _zapVelocity *= 0.5f;
            }

            if (Timer % 8 == 0)
            {
                Vector2 pVelocity = -Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = Particle.NewParticle<ZapParticle>(NPC.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                spark.Scale *= 0.5f;
                spark.Rotation = Main.rand.NextFloat(0f, 3.14f);
            }

            if (Timer % 12 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(NPC.Center, DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer % 6 == 0)
            {
                Vector2 vel = Vector2.Zero;
                Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }

            if (Timer <= 15)
            {
                _scale = MathHelper.Lerp(0f, Main.rand.NextFloat(1f, 1.4f), Easing.InCubic(Timer / 15f));
            }

            if (Timer > 400)
            {
                _scale *= 0.98f;
            } else
            {
                _scale = MathHelper.Lerp(_scale, 1f, 0.02f);
            }

            if (Timer >= 440)
            {
                NPC.Kill();
            }
        }

        public override void OnHitPlayer(Player target, Player.HurtInfo info)
        {
            base.OnHitPlayer(target, info);

            SoundStyle zapSound = SoundID.DD2_LightningBugZap;
            zapSound.PitchVariance = 0.5f;
            SoundEngine.PlaySound(zapSound, target.Center);
        }

        public override void OnKill()
        {
            base.OnKill();
            for (int i = 0; i < 16; i++)
            {
                float progress = i / 16f;
                float rot = progress * MathHelper.TwoPi;
                Vector2 vel = rot.ToRotationVector2() * 2;
                Dust d = Dust.NewDustPerfect(NPC.Center + Main.rand.NextVector2Circular(8, 8), DustID.GoldCoin, vel, Scale: 1);
                d.noGravity = true;
            }
        }
    }
}
