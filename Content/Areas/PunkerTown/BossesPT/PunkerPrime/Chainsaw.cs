using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class Chainsaw : PunkerPrimeArm
    {
        private enum AIState
        {
            Idle,
            Saw_Start,
            Saw
        }


        private bool _revvedUp;
        private int _frame;
        private Vector2 _oldTargetCenter;
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
        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_oldTargetCenter);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _oldTargetCenter = reader.ReadVector2();    
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
            if (_revvedUp)
            {
                NPC.frameCounter += 0.5f;
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
            var steamParticle = Particle<ThickSmokeParticle>.Spawn(spawnPosition, spawnVelocity, color: Color.DarkGray, Scale: spawnScale);
        }
        private void SetAngles(float baseAngle)
        {
            float osc = MathF.Sin(Timer * 0.06f) * 0.5f + 0.5f;

            Segments[0].angle = MathHelper.ToRadians(baseAngle) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc));
            Segments[1].angle = Segments[0].angle + MathHelper.ToRadians(75);
            Segments[2].angle = Segments[1].angle;
            Segments[3].angle = Segments[2].angle + MathHelper.ToRadians(80);
        }
        private void AI_Idle()
        {
            Timer++;
            isAttacking = false;
            _revvedUp = false;
            heldLightningScale *= 0.9f;
            telegraphLineColor *= 0.2f;
        

            AimGunTowardTarget();
            SetAngles(-45);
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
            if(Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle revSound = AssetRegistry.Sounds.SteamPunking.MechSaw;
                revSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(revSound, NPC.position);
                CreateMuzzleFlash();
            }

            if(Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            TargetOutlineColor = Color.Yellow;


            float revTime = 100;
            float completionRatio = Timer / revTime;
            telegraphLineColor = Color.Lerp(Color.Transparent, Color.Red, completionRatio);
            heldLightningScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(completionRatio));
            SetAngles(MathHelper.Lerp(-45, -175, EasingFunction.OutExpo(completionRatio)));
            AimGunTowardTarget();
            if (Timer >= revTime)
            {
                SwitchState(AIState.Saw);
            }
          
        }

        private void AI_Saw()
        {
            isAttacking = true;
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle revLoopSound = AssetRegistry.Sounds.SteamPunking.MechSawRevLoop;
                revLoopSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(revLoopSound, NPC.position);

                Vector2 rocketVelocity = NPC.rotation.ToRotationVector2() * 40;
                NPC.velocity = rocketVelocity;
                ShakeScreenPosition.Shake = 8;
                CreateMuzzleFlash();
                FXUtil.ShakeCamera(NPC.position, 1024, 8);


                float numDust = 24;
                for(float f = 0; f < numDust; f++)
                {
                    Vector2 dustVelocity = NPC.velocity * 0.2f;
                    dustVelocity = dustVelocity.RotatedByRandom(0.5f);
                    Dust.NewDustPerfect(NPC.Center, DustID.FireworkFountain_Red, dustVelocity, Scale: Main.rand.NextFloat(0.5f, 1f));
                }
            }
            telegraphLineColor *= 0.2f;
            if (Timer % 10 == 0)
            {
                SpawnSteamParticle();
            }

            if(Timer % 5 == 0)
            {
                Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FireworkFountain_Red);
            }

            if(Timer % 10 == 0)
            {
                var spawnPos = NPC.Center;
                spawnPos += Main.rand.NextVector2Circular(8, 8);
                var p = LegacyParticle.NewParticle<ZapParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), Color.Red, Main.rand.NextFloat(0.2f, 0.5f));
            }

            _revvedUp = true;
            float sawTime = 180;
            float divisor = sawTime / 6f;
            if(Timer >= 5 && Timer < 15)
            {
                NPC.velocity *= 0.94f;
            }
            if(Timer <= 5)
            {
                _oldTargetCenter = Target.Center;
            }
            if(Timer >= 30)
            {
                float degreesToRotate = MathHelper.Lerp(9f, 1, EasingFunction.Anticipation2((Timer - 30f)/ divisor));
                Vector2 homingVelocity = ProjectileHelper.SimpleHomingVelocity(NPC.Center, _oldTargetCenter, NPC.velocity, degreesToRotate);
                NPC.velocity = homingVelocity;
                NPC.velocity *= 1.01f;
            }

            if(Timer >= 145f)
            {
                NPC.velocity *= 0.96f;
            }
            afterImageStrength = EasingFunction.QuadraticBump(Timer / sawTime);
            NPC.rotation = NPC.velocity.ToRotation();
            TargetOutlineColor = Color.Red;

            SetAngles(MathHelper.Lerp(-175, -45, Timer / sawTime));
            if (Timer >= sawTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
