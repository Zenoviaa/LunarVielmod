using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Common.Shaders;
using Stellamod.Common.Shaders.MagicTrails;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Dusts;
using Stellamod.Helpers;
using Stellamod.Trails;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class AssaultBullet : ScarletProjectile
    {
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        public override void SetDefaults()
        {
            base.SetDefaults();
            TrailCacheLength = 16;
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.penetrate = -1;
            Projectile.timeLeft = 180;
            Projectile.extraUpdates = 1;
        }
        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                FXUtil.GlowCircleBoom(Projectile.Center, Color.Red, Color.DarkRed, Color.Black);
                for(float f = 0; f < 4; f++)
                {
                    Vector2 fireVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                    fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                    fireVelocity *= Main.rand.NextFloat(3f, 8f);

                    DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                    spawnParams.outerColor = Color.Red;
                    spawnParams.scaleRange *= 0.5f;
                    DustParticle.Spawn(Projectile.Center, fireVelocity, spawnParams);
                }
            }
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }


        private Color GetTrailColor(float completionRatio)
        {
            float osc = MathF.Sin(Main.GlobalTimeWrappedHourly * 4 + completionRatio * 8) * 0.5f + 0.5f;
            return Color.Lerp(Color.White, Color.Red, osc);
        }

        private float GetTrailWidth(float completionRatio)
        {
            return MathHelper.SmoothStep(8, 2, completionRatio);
        }
        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueueSpritebatchDrawAction(DrawHead);
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Red, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 5;
            float ew = w / 10;
            float width = w;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            var shader = RichLaserShader.Instance;
            shader.LaserColor = Color.White;
            shader.InnerColor = Color.Red;
            shader.OuterColor = Color.DarkRed;
            shader.LaserTexture = AssetManager.LaserTextures.TexturedLaser;
            shader.BloomTexture = AssetManager.LaserTextures.TexturedLaser2;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, GetTrailColor, GetTrailWidth, shader);
        }

        private void DrawHead(SpriteBatch sb, Vector2 screenPos)
        {
            SpritebatchDrawer headDrawer = SpritebatchDrawer.FromTextureAsset(AssetManager.GlowMask.StarFlare1, Projectile.Center);
            headDrawer.scale *= 0.3f;
            headDrawer.scale.Y *= 0.45f;
            headDrawer.scale.X *= 2;
            headDrawer.rotation = Projectile.rotation;
            headDrawer.color = Color.Red;
            headDrawer.color.A = 0;
            sb.Draw(headDrawer);

            headDrawer.color = Color.White;
            headDrawer.color.A = 0;
            headDrawer.scale *= 0.75f;
            sb.Draw(headDrawer);

        }
        public override void OnKill(int timeLeft)
        {
            base.OnKill(timeLeft);
            float numDust = 3f;
            for(float d = 0; d <numDust; d++)
            {
                Vector2 velocity = Main.rand.NextVector2Circular(8, 8);
                Dust.NewDustPerfect(Projectile.Center, 
                    ModContent.DustType<GlowDust>(), velocity, newColor: Color.Yellow, Scale: Main.rand.NextFloat(0.5f, 2f));
            }
            for (float f = 0; f < 4; f++)
            {
                Vector2 fireVelocity = -Projectile.oldVelocity.SafeNormalize(Vector2.Zero);
                fireVelocity = fireVelocity.RotatedByRandom(MathHelper.ToRadians(60));
                fireVelocity *= Main.rand.NextFloat(3f, 8f);

                DustParticleSpawnParams spawnParams = DustParticleSpawnParams.Default;
                spawnParams.outerColor = Color.Red;
                spawnParams.scaleRange *= 0.5f;
                DustParticle.Spawn(Projectile.Center, fireVelocity, spawnParams);
            }
        }
    }

    public class AssaultRifle : PunkerPrimeArm
    {
        private enum AIState
        {
            Idle,
            Shoot_Start,
            Shoot
        }

        private AIState State
        {
            get => (AIState)NPC.ai[3];
            set => NPC.ai[3] = (float)value;
        }

        private int RifleDamage => 13;
        private float BaseAngle => -15;
        public override void ArmAI()

        {
            base.ArmAI();
            SetRootToParentCenter();
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Shoot_Start:
                    AI_ShootStart();
                    break;
                case AIState.Shoot:
                    AI_Shoot();
                    break;
            }

        }


        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return false;
        }
        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                State = state;
                Timer = 0;
                NPC.netUpdate = true;
            }
        }
        private void SetAngles(float baseAngle)
        {
            float osc = MathF.Sin(Timer * 0.02f) * 0.5f + 0.5f;

            Segments[0].angle = MathHelper.ToRadians(baseAngle) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc));
            Segments[1].angle = Segments[0].angle + MathHelper.ToRadians(75);
            Segments[2].angle = Segments[1].angle;
            Segments[3].angle = Segments[2].angle + MathHelper.ToRadians(80);
        }
        private void AI_Idle()
        {
            Timer++;
            isAttacking = false;
            heldLightningScale *= 0.9f;
            telegraphLineColor *= 0.2f;



            TargetOutlineColor = Color.Transparent;
            AimGunTowardTarget();
            SetAngles(BaseAngle);
            if (DoAttack)
            {
                DoAttack = false;
                SwitchState(AIState.Shoot_Start);
            }
        }
        private void SpawnSteamParticle()
        {
            Vector2 spawnPosition = NPC.Top;
            spawnPosition.X += Main.rand.NextFloat(-64, 64);

            Vector2 spawnVelocity = Vector2.Zero;
            spawnVelocity.Y = Main.rand.NextFloat(-10, -1f);

            float spawnScale = Main.rand.NextFloat(0.75f, 1f);
            var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }

        private void AI_ShootStart()
        {
            isAttacking = true;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle revSound = AssetRegistry.Sounds.SteamPunking.MechSaw;
                revSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(revSound, NPC.position);
                CreateMuzzleFlash();
            }

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            TargetOutlineColor = Color.Yellow;

            AimGunTowardTarget();
            float revTime = 100;
            float completionRatio = Timer / revTime;
            telegraphLineColor = Color.Lerp(Color.Transparent, Color.Red, completionRatio);
            heldLightningScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(completionRatio));
            SetAngles(MathHelper.Lerp(BaseAngle, BaseAngle - 90, EasingFunction.OutExpo(completionRatio)));

            Vector2 targetFireVelocity = (Target.Center - NPC.Center);
            float targetRotation = targetFireVelocity.ToRotation();
            NPC.rotation = targetRotation;

            if (Timer >= revTime)
            {
                SwitchState(AIState.Shoot);
            }
        }

        private void AI_Shoot()
        {
            isAttacking = true;
            Timer++;
            telegraphLineColor *= 0.2f;
            if (Timer % 10 == 0)
            {
                SpawnSteamParticle();
            }

            if (Timer % 5 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FireworkFountain_Red);
            }

            if (Timer % 10 == 0)
            {
                var spawnPos = NPC.Center;
                spawnPos += Main.rand.NextVector2Circular(8, 8);
                var p = LegacyParticle.NewParticle<ZapParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }

            NPC.velocity *= 0.1f;

            int fireTime = 15;
            int fireCount = 6;

            AimGunTowardTarget();
            float fullFireTime = (fireTime * fireCount);
            float completionRatio = Timer / fullFireTime;
            SetAngles(MathHelper.Lerp(BaseAngle - 90, BaseAngle, completionRatio));
            telegraphLineColor = Color.Red;
            Vector2 targetFireVelocity = (Target.Center - NPC.Center);
            float targetRotation = targetFireVelocity.ToRotation();
            NPC.rotation = targetRotation;

            if (Timer % fireTime == 0)
            {
                SoundStyle mechShoot = new SoundStyle("Stellamod/Assets/Sounds/GunShootNew1");
                mechShoot.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mechShoot, NPC.position);

                CreateMuzzleFlash();
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 fireVelocity = NPC.rotation.ToRotationVector2();
                    fireVelocity *= 12;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<AssaultBullet>(), RifleDamage, 1, Main.myPlayer);
                }
                float numDust = 8;
                for (float f = 0; f < numDust; f++)
                {
                    Vector2 dustVelocity = NPC.rotation.ToRotationVector2();
                    dustVelocity *= Main.rand.NextFloat(1f, 10f);
                    dustVelocity = dustVelocity.RotatedByRandom(0.5f);
                    Dust.NewDustPerfect(NPC.Center, ModContent.DustType<GlowDust>(), dustVelocity, newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
                var stretchParticle = FXUtil.GlowStretch(NPC.Center, NPC.rotation.ToRotationVector2() * 5f);
                stretchParticle.InnerColor = Color.Red;
                stretchParticle.GlowColor = Color.Violet;
            }

            if (Timer >= fullFireTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
