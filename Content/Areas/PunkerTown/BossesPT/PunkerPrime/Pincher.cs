using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class Pincher : PunkerPrimeArm
    {
        private enum AIState
        {
            Idle,
            Pinch_Start,
            Pinching
        }


        private bool _revvedUp;
        private bool _shouldHome;
        private int _frame;
        private AIState State
        {
            get => (AIState)NPC.ai[3];
            set => NPC.ai[3] = (float)value;
        }

        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && State == AIState.Pinching;
        }

        private void SwitchState(AIState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }
        private void SetAngles(float baseAngle)
        {

            float osc = MathF.Sin(Timer * 0.1f) * 0.5f + 0.5f;
            Segments[0].angle = MathHelper.ToRadians(baseAngle) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc));
            Segments[1].angle = Segments[0].angle + MathHelper.ToRadians(-75);
            Segments[2].angle = Segments[1].angle;
            Segments[3].angle = Segments[2].angle + MathHelper.ToRadians(-80);
        }
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 4;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            if (_revvedUp)
            {
                NPC.frameCounter += 0.25f;
                if (NPC.frameCounter >= 1f)
                {
                    _frame++;
                    NPC.frameCounter = 0f;
                }

                if (_frame >= Main.npcFrameCount[Type])
                    _frame = 0;
            }
            else
            {
                _frame = 0;
            }


            NPC.frame.Y = frameHeight * _frame;
        }

        public override void ArmAI()
        {
            base.ArmAI();
            SetRootToParentCenter();
            switch (State)
            {
                case AIState.Idle:
                    AI_Idle();
                    break;
                case AIState.Pinch_Start:
                    AI_SawStart();
                    break;
                case AIState.Pinching:
                    AI_Saw();
                    break;
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

        private void AI_Idle()
        {
            heldLightningScale *= 0.9f;
            telegraphLineColor *= 0.9f;

            isAttacking = false;
            _revvedUp = false;
            Timer++;

            AimGunTowardTarget();
            SetAngles(-165);
            if (DoAttack)
            {
                DoAttack = false;
                SwitchState(AIState.Pinch_Start);
            }
            TargetOutlineColor = Color.Transparent;
        }

        private void AI_SawStart()
        {
            isAttacking = true;
            Timer++;
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

            float revTime = 100;
            float completionRatio = Timer / revTime;
            telegraphLineColor = Color.Lerp(Color.Transparent, Color.Red, completionRatio);
            heldLightningScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(completionRatio));
            SetAngles(MathHelper.Lerp(-165, -330, EasingFunction.OutExpo(completionRatio)));
            AimGunTowardTarget();
            if (Timer >= revTime)
            {
                SwitchState(AIState.Pinching);
            }

        }

        private void AI_Saw()
        {
            isAttacking = true;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                CreateMuzzleFlash();
                SoundStyle revLoopSound = AssetRegistry.Sounds.SteamPunking.MechSawRevLoop;
                revLoopSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(revLoopSound, NPC.position);

                _shouldHome = true;
                Vector2 targetVelocity = NPC.rotation.ToRotationVector2() * 18f;
                NPC.velocity = targetVelocity;
                ShakeModSystem.Shake = 10;
            }

            if (Timer % 20 == 0)
            {
                SpawnSteamParticle();
            }

            if (Timer % 5 == 0)
            {
                var d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Firework_Red);
                Vector2 upVelocity = -Vector2.UnitY * 5;
                Main.dust[d].velocity += upVelocity;

                d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FireworkFountain_Red);
                upVelocity = -Vector2.UnitY * 2;
                Main.dust[d].velocity += upVelocity;
            }


            if(Timer >= 30 && Timer < 60)
            {
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(NPC.Center, Target.Center, NPC.velocity, 5);
                NPC.velocity = homingVelocity;
                NPC.velocity *= 1.005f;
            }
            _revvedUp = true;
            float sawTime = 180;
            float completionRatio = Timer / sawTime;

            if(NPC.velocity.Y < 20)
                NPC.velocity.Y += 0.6f;
                NPC.rotation = NPC.velocity.ToRotation();
            SetAngles(MathHelper.Lerp(-330, -165, completionRatio));



            afterImageStrength = EasingFunction.QuadraticBump(completionRatio);
            TargetOutlineColor = Color.Red;
            telegraphLineColor *= 0.2f;
            if (Timer >= sawTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
