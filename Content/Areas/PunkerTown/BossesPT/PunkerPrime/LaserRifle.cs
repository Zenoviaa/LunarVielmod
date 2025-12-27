using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Pixelation;
using Stellamod.Core.Shaders;
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
    public class PunkingLaser : ModProjectile
    {
        private Vector2[] _laserPointsBackingField;
        private Vector2[] LaserPoints
        {
            get
            {
                if (_laserPointsBackingField == null)
                {
                    _laserPointsBackingField = new Vector2[32];
                }
                return _laserPointsBackingField;
            }
        }
        private ref float Timer => ref Projectile.ai[0];
        public override string Texture => TextureRegistry.EmptyTexture;
        private NPC Parent
        {
            get => Main.npc[(int)Projectile.ai[1]];
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Projectile.width = 8;
            Projectile.height = 8;
            Projectile.hostile = true;
            Projectile.tileCollide = false;
            Projectile.penetrate = -1;
            Projectile.ignoreWater = true;
            Projectile.timeLeft = 180;
        }

        public override bool? Colliding(Rectangle projHitbox, Rectangle targetHitbox)
        {
            return ProjectileHelper.OldPosColliding(LaserPoints, projHitbox, targetHitbox);
        }

        public override bool ShouldUpdatePosition()
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            Timer++;
            if(Timer == 1)
            {
                SoundStyle fireSound = new SoundStyle("Stellamod/Assets/Sounds/SingularityFragment_Shot1");
                fireSound.Pitch = 0.6f;
                fireSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(fireSound, Projectile.position);
            }
            float numPoints = LaserPoints.Length;
            for (int i = 0; i < LaserPoints.Length; i++)
            {
                float f = i;
                float completionRatio = f / numPoints;
                LaserPoints[i] = Vector2.Lerp(Projectile.Center, Projectile.Center + Projectile.velocity, completionRatio);
            }
            if (Timer % 5 == 0)
            {
                Vector2 dustVelocity = Projectile.velocity.SafeNormalize(Vector2.Zero);
                dustVelocity = dustVelocity.RotatedByRandom(0.5f);
                Dust.NewDustPerfect(Projectile.Center, ModContent.DustType<GlowDust>(), dustVelocity, newColor: Color.Red, Scale: Main.rand.NextFloat(0.5f, 2f));
            }
            Projectile.Center = Parent.Center;
        }

        private Color ColorFunction(float completionRatio)
        {
            float oscillate = MathF.Sin(completionRatio * 32 - Main.GlobalTimeWrappedHourly * 32) * 0.5f + 0.5f;
            Color oscillatingColor = Color.Lerp(Color.DarkRed, Color.Red, oscillate);
            Color glowingColor = Color.Lerp(Color.White, Color.Red, ExtraMath.Osc(0f, 1f, speed: 64));
            return oscillatingColor.MultiplyRGB(glowingColor);
        }

        private float WidthFunction(float completionRatio)
        {
            float inScale = EasingFunction.InOutSine(Timer / 30f);
            float outScale = EasingFunction.InOutSine((float)(((float)Projectile.timeLeft) / 30f));
            return 32 * inScale * outScale;
        }

        public override bool PreDraw(ref Color lightColor)
        {
            PixelationManager.QueuePrimitivesDrawAction(DrawPixelated);
            return false;
        }

        public void DrawPixelated(GraphicsDevice graphicsDevice)
        {
            var shader = BasicLaserShader.Instance;
            shader.LaserTexture = TrailRegistry.BeamTrail;
            shader.BlendState = BlendState.AlphaBlend;
            shader.SamplerState = SamplerState.PointWrap;
            shader.Time = Main.GlobalTimeWrappedHourly * 32;
            shader.InnerColor = Color.White;
            shader.OuterColor = Color.Red;
            //This just applis the shader changes
            TrailDrawer.Draw(Main.spriteBatch, LaserPoints, ColorFunction, WidthFunction, shader);
        }
    }
    public class LaserRifle : PunkerPrimeArm
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

        private int LaserDamage => 30;
        private float BaseAngle => -160;
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

            Vector2 targetFireVelocity = Vector2.UnitY;
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
                var p = LegacyParticle.NewParticle<ZapParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }



            int fireTime = 240;
            float completionRatio = Timer / fireTime;
            SetAngles(MathHelper.Lerp(BaseAngle - 90, BaseAngle, completionRatio));
            telegraphLineColor = Color.Red;


            int targetDirection = Target.Center.X > NPC.Center.X ? 1 : -1;
            NPC.velocity.Y = MathHelper.Lerp(1, -2, EasingFunction.Anticipation(Timer / 30f));
            NPC.velocity.X = MathHelper.Lerp(5 * -targetDirection, -5 * -targetDirection, EasingFunction.Anticipation(Timer / 30f));

            Vector2 targetFireVelocity = Vector2.UnitY * 1000;
            float targetRotation = targetFireVelocity.ToRotation();
            NPC.rotation = targetRotation;
            if(Timer == 1)
            {
                SoundStyle mechShoot = AssetRegistry.Sounds.SteamPunking.MechShoot1;
                mechShoot.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mechShoot, NPC.position);

                CreateMuzzleFlash();
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, targetFireVelocity,
                        ModContent.ProjectileType<PunkingLaser>(), LaserDamage, 1, Main.myPlayer, ai1: NPC.whoAmI);
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

            if (Timer >= fireTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
