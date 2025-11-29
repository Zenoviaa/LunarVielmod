using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
using Stellamod.Core.Shaders.MagicTrails;
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
    public class AssaultBullet : ScarletProjectile,
        IDrawPixelated
    {
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
            Projectile.velocity *= 1.01f;
            Projectile.rotation = Projectile.velocity.ToRotation();
        }

        public override bool PreDraw(ref Color lightColor)
        {
            return false;
        }
        private Color ColorFunction(float completionRatio)
        {
            return Color.Lerp(Color.Yellow, Color.Red, completionRatio);
        }

        private float WidthFunction(float completionRatio)
        {
            float w = 100;
            float ew = w / 10;
            float width = w;

            float p = completionRatio / 0.5f;
            float ep = EasingFunction.OutCirc(p);
            float circleWidth = MathHelper.Lerp(0, w, ep);
            float trailWidth = MathHelper.Lerp(width, 0, EasingFunction.OutCirc(completionRatio));
            return MathHelper.Lerp(circleWidth, trailWidth, EasingFunction.OutExpo(completionRatio));
        }

        public void DrawPixelated()
        {
            var shader = MagicNormalShader.Instance;
            shader.PrimaryTexture = TrailRegistry.GlowTrail;
            shader.NoiseTexture = TrailRegistry.SpikyTrail1;
            shader.BlendState = BlendState.Additive;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Speed = 0.5f;
            shader.Repeats = 1f;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, OldCenterPos, ColorFunction, WidthFunction, shader);
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
        private float BaseAngle => 45;
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
            var steamParticle = Particle.NewParticle<BlackSmokeParticle>(spawnPosition, spawnVelocity, Scale: spawnScale);
            steamParticle.innerColor = Color.DarkGray;
            steamParticle.outerColor = Color.Black;
            steamParticle.fadeToColor = Color.Black;
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

            if (Timer >= 60f)
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
                var p = Particle.NewParticle<ZapParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }

            NPC.velocity *= 0.1f;

            int fireTime = 25;
            int fireCount = 15;

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
                SoundStyle mechShoot = AssetRegistry.Sounds.SteamPunking.MechShoot1;
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
