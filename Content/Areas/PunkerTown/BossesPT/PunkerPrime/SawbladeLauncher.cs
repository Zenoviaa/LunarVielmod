using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Dusts;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class SawbladeLauncher : PunkerPrimeArm
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

        private int SawbladeDamage => 20;
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


        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                State = state;
                Timer = 0;
                NPC.netUpdate = true;
            }
        }

        private void AI_Idle()
        {
            isAttacking = false;

            Timer++;

            float osc = MathF.Sin(Timer * 0.02f) * 0.5f + 0.5f;
            TargetOutlineColor = Color.Transparent;
            Segments[0].angle = MathHelper.ToRadians(-135) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc));
            Segments[1].angle = Segments[0].angle + MathHelper.ToRadians(-75);
            Segments[2].angle = Segments[1].angle;
            Segments[3].angle = Segments[2].angle + MathHelper.ToRadians(-80);
            Vector2 holdCenter = GetGunHoldCenter();
            Vector2 targetVelocity = (holdCenter - NPC.Center);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, targetVelocity, EasingFunction.InOutSine(Timer / 60f));

            float targetAngle = Segments[Segments.Length - 1].angle;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);
            if (DoAttack)
            {
                DoAttack = false;
                SwitchState(AIState.Shoot_Start);
            }
        }

        private void AI_ShootStart()
        {
            isAttacking = true;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
            }
            TargetOutlineColor = Color.Yellow;

            Vector2 holdCenter = GetGunHoldCenter();
            Vector2 targetVelocity = (holdCenter - NPC.Center);
            NPC.velocity = targetVelocity;

            Vector2 aimVelocity = (Target.Center - NPC.Center);
            aimVelocity = aimVelocity.SafeNormalize(Vector2.Zero);
            float rotation = aimVelocity.ToRotation();
            //NPC.rotation = Utils.AngleLerp(NPC.rotation, rotation, 0.01f);//
            if (Timer >= 60f)
            {
                SwitchState(AIState.Shoot);
            }
        }

        private void AI_Shoot()
        {
            isAttacking = true;
            Timer++;

            NPC.velocity *= 0.1f;

            int fireTime = 45;
            int fireCount = 3;
            if (Timer % fireTime == 0)
            {
                SoundStyle mechShoot = AssetRegistry.Sounds.SteamPunking.MechShoot1;
                mechShoot.PitchVariance = 0.3f;
                SoundEngine.PlaySound(mechShoot, NPC.position);

                CreateMuzzleFlash();
                if (MultiplayerHelper.IsHost)
                {
                    Vector2 fireVelocity = NPC.rotation.ToRotationVector2();
                    fireVelocity *= 7f;
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, fireVelocity,
                        ModContent.ProjectileType<PrimeSawblade>(), SawbladeDamage, 1, Main.myPlayer);
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

            if (Timer >= (fireTime * fireCount))
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
