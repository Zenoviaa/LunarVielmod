using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class Chainsaw2 : PunkerPrimeArm
    {
        private enum AIState
        {
            Idle,
            Saw_Start,
            Saw
        }


        private bool _revvedUp;
        private int _frame;
        private AIState State
        {
            get => (AIState)NPC.ai[3];
            set => NPC.ai[3] = (float)value;
        }
        public override bool CanHitPlayer(Player target, ref int cooldownSlot)
        {
            return base.CanHitPlayer(target, ref cooldownSlot) && State == AIState.Saw;
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

        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.npcFrameCount[Type] = 3;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            if (_revvedUp)
            {
                NPC.frameCounter += 0.15f;
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
                case AIState.Saw_Start:
                    AI_SawStart();
                    break;
                case AIState.Saw:
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
            var steamParticle = Particle.NewParticle<BlackSmokeParticle>(spawnPosition, spawnVelocity, Scale: spawnScale);
            steamParticle.innerColor = Color.DarkGray;
            steamParticle.outerColor = Color.Black;
            steamParticle.fadeToColor = Color.Black;
        }

        private void AI_Idle()
        {
            isAttacking = false;
            _revvedUp = false;
            Timer++;
            float osc = MathF.Sin(Timer * 0.06f) * 0.5f + 0.5f;

            Segments[0].angle = MathHelper.ToRadians(-90) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc));
            Segments[1].angle = Segments[0].angle + MathHelper.ToRadians(-75);
            Segments[2].angle = Segments[1].angle;
            Segments[3].angle = Segments[2].angle + MathHelper.ToRadians(-80);
            AimGunTowardTarget();
            if (DoAttack)
            {
                DoAttack = false;
                SwitchState(AIState.Saw_Start);
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
            float revTime = 60;
            if (Timer >= revTime)
            {
                SwitchState(AIState.Saw);
            }

        }

        private void AI_Saw()
        {
            isAttacking = true;
            Timer++;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle revLoopSound = AssetRegistry.Sounds.SteamPunking.MechSawRevLoop;
                revLoopSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(revLoopSound, NPC.position);

                Vector2 targetVelocity = (Target.Center - NPC.Center);
                targetVelocity = targetVelocity.SafeNormalize(Vector2.Zero);
                targetVelocity *= 3f;
                NPC.velocity = targetVelocity;
            }

            if (Timer % 10 == 0)
            {
                SpawnSteamParticle();
            }

            _revvedUp = true;
            float sawTime = 180;
            Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(NPC.Center, Target.Center, NPC.velocity);
            NPC.velocity = Vector2.Lerp(NPC.velocity, homingVelocity, 0.1f);
            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.01f);

            TargetOutlineColor = Color.Red;
            if (Timer >= sawTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
