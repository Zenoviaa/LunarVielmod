using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Shaders;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.GameContent.Animations.IL_Actions.NPCs;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles
{
    public class DescendingElectricBoom : ModProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 120;
            Projectile.height = 120;
            Projectile.hostile = true;
            Projectile.ignoreWater = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 15;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            ShakeModSystem.Shake = 10;
            if (Timer == 1)
            {
                var screenShaderSystem = ModContent.GetInstance<ScreenShaderSystem>();
                screenShaderSystem.TintScreen(Color.Yellow, 0.5f, 20);

                SoundStyle boomSound = AssetRegistry.Sounds.SteamPunking.DescendingBoom;
                boomSound.PitchVariance = 0.3f;
                boomSound.Pitch = 1;
                SoundEngine.PlaySound(boomSound, Projectile.position);

                for (float f = 0; f < 4; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.PiOver4 / 3f);
                    pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                    var spark = Particle.NewParticle<ZapParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                    spark.innerColor = Color.White;
                    spark.outerColor = Color.Yellow;
                    spark.fadeToColor = Color.Blue;
                }

                var part = Particle.NewParticle<GlowDonutParticle>(Projectile.Center, Vector2.Zero, Color.White);
                part.Scale *= 2;
                part.noStretch = true;
                part.innerColor = Color.Yellow;
                part.outerColor = Color.Lerp(Color.Yellow, Color.Blue, 0.25f);
                part.fadeToColor = Color.Lerp(Color.Yellow, Color.Black, 0.5f);
                for (float f = 0; f < 8; f++)
                {
                    float radius = 800;
                    Vector2 spawnPos = Projectile.Center + Main.rand.NextVector2CircularEdge(radius, radius);
                    Vector2 velocity = Projectile.Center - spawnPos;
                    velocity = velocity.SafeNormalize(Vector2.Zero);
                    velocity *= Main.rand.NextFloat(8, 32);
                    var p = FXUtil.GlowStretch(spawnPos, velocity);
                    p.InnerColor = Color.Yellow;
                    p.GlowColor = Color.Lerp(Color.Yellow, Color.Blue, 0.25f);
                    p.OuterGlowColor = Color.Lerp(Color.Yellow, Color.Black, 0.5f);
                    p.Scale *= 3f;
                }

                FXUtil.ShakeCamera(Projectile.position, 1024, 10);
                for (float f = 0; f < 8; f++)
                {
                    Vector2 pVelocity = Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi);
                    pVelocity *= Main.rand.NextFloat(0.5f, 8f);
                    var spark = Particle.NewParticle<EmberParticle>(Projectile.Center + Main.rand.NextVector2Circular(64, 64), pVelocity);
                }

                float numDust = 16;
                for (float n = 0; n < numDust; n++)
                {
                    SpawnFlameDust(Projectile.Center, Main.rand.NextVector2Circular(16, 16));
                    SpawnGlowDust(Projectile.Center, Main.rand.NextVector2Circular(64, 64));
                }
                for (float i = 0; i < 8; i++)
                {
                    float progress = i / 4f;
                    float rot = progress * MathHelper.ToRadians(360);
                    rot += Main.rand.NextFloat(-0.5f, 0.5f);
                    Vector2 offset = rot.ToRotationVector2() * 24;
                    var particle = FXUtil.GlowCircleDetailedBoom1(Projectile.Center,
                        innerColor: Color.White,
                        glowColor: Color.Yellow,
                        outerGlowColor: Color.Lerp(Color.Yellow, Color.DarkBlue, 0.5f),
                        baseSize: Main.rand.NextFloat(0.1f, 0.2f),
                        duration: Main.rand.NextFloat(15, 25));
                    particle.Rotation = rot + MathHelper.ToRadians(45);
                    particle.Scale *= 4f;
                }
            }
        }

        private void SpawnFlameDust(Vector2 position, Vector2 velocity)
        {
            var p = Particle.NewParticle<GlowFragmentParticle>(position, velocity, Color.White, Scale: 4f);
            Color twinColor = Color.Yellow;
            p.innerColor = twinColor;
            p.outerColor = Color.Lerp(twinColor, Color.Black, 0.5f);
            p.fadeToColor = Color.Lerp(twinColor, Color.DarkBlue, 0.5f);
        }
        private void SpawnGlowDust(Vector2 position, Vector2 velocity)
        {
            Dust.NewDustPerfect(position, ModContent.DustType<GlowDust>(), velocity, newColor: Color.Yellow, Scale: 2f);
        }
    }
    public class DescendingElectricBall : ModProjectile
    {
        private enum AIState
        {
            Charge,
            Fire
        }
        private ref float Timer => ref Projectile.ai[0];
        private Vector2 TargetVelocity;
        private AIState State
        {
            get => (AIState)Projectile.ai[1];
            set => Projectile.ai[1] = (float)value;
        }
        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 64;
            Projectile.height = 64;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 1800;
            Projectile.ignoreWater = true;
        }

        public override void AI()
        {
            base.AI();
            if(Timer % 4 == 0)
            {
                Vector2 pos = Projectile.Center;
                pos += Main.rand.NextVector2Circular(32, 32);
                var zap = Particle.NewParticle<ZapParticle>(pos, Vector2.UnitY.RotatedByRandom(MathHelper.TwoPi) * Main.rand.NextFloat(1, 4));
            }

            if(Timer % 8 == 0)
            {
                Vector2 pos = Projectile.Center;
                Vector2 pVelocity = Main.rand.NextVector2Circular(2, 2);
                pVelocity *= Main.rand.NextFloat(0.5f, 1f);
                var spark = Particle.NewParticle<SparkParticle>(pos + Main.rand.NextVector2Circular(64, 64), pVelocity);
            }
            switch (State)
            {
                case AIState.Charge:
                    AI_Charge();
                    break;
                case AIState.Fire:
                    AI_Fire();
                    break;
            }
            Lighting.AddLight(Projectile.Center, TorchID.Yellow);
        }

        private void AI_Charge()
        {
            Timer++;
            float chargeTime = 120f;
            float completionRatio = Timer / chargeTime;
            float ease = EasingFunction.InOutExpo(completionRatio);
            Projectile.scale = ease;
            ShakeModSystem.Shake = 2;
        }

        private void AI_Fire()
        {
            Timer++;
            if(Timer == 1)
            {
   
            }
            Player player = PlayerHelper.FindClosestPlayer(Projectile.position, 8000);
            if (player != null)
            {
                TargetVelocity = (player.Center - Projectile.Center).SafeNormalize(Vector2.Zero) * 25;
            }

            float shootTime = 30f;
            if(Timer < shootTime)
            {
                float completionRatio = Timer / shootTime;
                float ease = EasingFunction.Anticipation2(completionRatio);
                Vector2 velocity = Vector2.Lerp(-TargetVelocity * 0.2f, TargetVelocity, ease);
                Projectile.velocity = velocity;

            }



            if (Timer >= 60)
            {
                if(player != null)
                {
                    Projectile.velocity = ProjectileHelper.SimpleHomingVelocity(Projectile, player.Center, 5);
                }
      
                Projectile.velocity *= 0.95f;
                if(Projectile.velocity.Length() <= 1f)
                {
                    Projectile.scale *= 1.1f;
                    if(Projectile.scale >= 1f)
                    {
                        if (this.OwnedByLocalClient())
                        {
                            Projectile.NewProjectile(Projectile.GetSource_FromThis(), Projectile.Center, Vector2.Zero, 
                                ModContent.ProjectileType<DescendingElectricBoom>(), Projectile.damage, 1, Projectile.owner);
                        }
                        Projectile.Kill();
                    }
                }
            }
            else
            {
                Projectile.scale *= 0.95f;
            }
        }

        public override bool PreDraw(ref Color lightColor)
        {
            DescendingLightningBallShader shader = DescendingLightningBallShader.Instance;
            shader.NoiseTexture = AssetRegistry.Textures.Noise.IceWaterCaustics;
            shader.GradientStartColor = Color.Yellow;
            shader.GradientMidColor = Color.Gold;
            shader.GradientEndColor = Color.Purple;


            SpriteBatch spriteBatch = Main.spriteBatch;
            Texture2D texture = ModContent.Request<Texture2D>(Texture).Value;
            Vector2 drawCenter = Projectile.Center - Main.screenPosition;
            Vector2 drawOrigin = texture.Size() / 2f;
            float rotation = Projectile.rotation + Main.GlobalTimeWrappedHourly * 2;
            float scale = Projectile.scale;
            spriteBatch.Restart(effect: shader.Effect);
            spriteBatch.Draw(texture, drawCenter, null, Color.White, 0, drawOrigin, new Vector2(0.3f, 2) * scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawCenter, null, Color.White, 0, drawOrigin, new Vector2(2, 0.3f) * scale, SpriteEffects.None, 0);
            spriteBatch.Draw(texture, drawCenter, null, Color.White, rotation, drawOrigin, new Vector2(1, 1) * scale, SpriteEffects.None, 0);
            spriteBatch.RestartDefaults();
            return false;
        }

        private void SwitchState(AIState state)
        {
            if (this.OwnedByLocalClient())
            {
                Timer = 0;
                State = state;
                Projectile.netUpdate = true;
            }
        }

        public void Fire()
        {
            SwitchState(AIState.Fire);
        }
    }
}
