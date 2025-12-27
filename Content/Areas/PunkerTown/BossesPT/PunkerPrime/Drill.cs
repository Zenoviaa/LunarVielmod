using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Particles;
using Stellamod.Helpers;
using Stellamod.Visual.Particles;
using System;
using System.IO;
using Terraria;
using Terraria.Audio;
using Terraria.ID;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.PunkerPrime
{
    public class Drill : PunkerPrimeArm
    {
        private enum AIState
        {
            Idle,
            Saw_Start,
            Saw
        }


        private bool _revvedUp;
        private int _frame;
        private Vector2 _startCenter;
        private Vector2 _targetStartCenter;
        private float _targetStartRotation;
        private float _fireFromDirection;
        private AIState State
        {
            get => (AIState)NPC.ai[3];
            set => NPC.ai[3] = (float)value;
        }

        public override void SendExtraAI(BinaryWriter writer)
        {
            base.SendExtraAI(writer);
            writer.WriteVector2(_startCenter);
            writer.WriteVector2(_targetStartCenter);
            writer.Write(_targetStartRotation);
            writer.Write(_fireFromDirection);
        }

        public override void ReceiveExtraAI(BinaryReader reader)
        {
            base.ReceiveExtraAI(reader);
            _startCenter = reader.ReadVector2();
            _targetStartCenter = reader.ReadVector2();
            _targetStartRotation = reader.ReadSingle();
            _fireFromDirection = reader.ReadSingle();
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
            float osc = MathF.Sin(Timer * 0.05f) * 0.5f + 0.5f;
            Segments[0].angle = MathHelper.ToRadians(baseAngle) + MathHelper.ToRadians(MathHelper.Lerp(0, 10, osc));
            Segments[1].angle = Segments[0].angle + MathHelper.ToRadians(75);
            Segments[2].angle = Segments[1].angle;
            Segments[3].angle = Segments[2].angle + MathHelper.ToRadians(80);
        }
        private void AI_Idle()
        {
            telegraphLineColor = Color.Transparent;
            isAttacking = false;
            _revvedUp = false;
            heldLightningScale *= 0.9f;
            Timer++;
    
            SetAngles(-90);
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

            float startOffset = 420;
            if (Timer == 1)
            {
                NPC.TargetClosest();
                SoundStyle revSound = AssetRegistry.Sounds.SteamPunking.MechSaw;
                revSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(revSound, NPC.position);
                CreateMuzzleFlash();
                if (MultiplayerHelper.IsHost)
                {
                    _fireFromDirection = Main.rand.NextBool(2) ? 1 : -1;
                    _startCenter = NPC.Center;
                    _targetStartCenter = Target.Center + new Vector2(startOffset * _fireFromDirection, 0);
           

                    Vector2 direction = Vector2.UnitX * -_fireFromDirection;
                    _targetStartRotation = direction.ToRotation();
                    NPC.netUpdate = true;
                }

        
            }


            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }


            TargetOutlineColor = Color.Yellow;
            float revTime = 100;
            float completionRatio = Timer / revTime;
            float ease = EasingFunction.Anticipation2(completionRatio);

            heldLightningScale = MathHelper.Lerp(0f, 1f, EasingFunction.OutExpo(completionRatio));
            _targetStartCenter = Target.Center + new Vector2(startOffset * _fireFromDirection, 0);

            Vector2 positionToLerpTo = Vector2.Lerp(_startCenter, _targetStartCenter, ease);
            Vector2 velocity = (positionToLerpTo - NPC.Center);
            NPC.velocity = velocity;
            telegraphLineColor = Color.Lerp(Color.Transparent, Color.Red, ease);

            SetAngles(MathHelper.Lerp(-90, -135, EasingFunction.OutExpo(completionRatio)));
            NPC.rotation = Utils.AngleLerp(NPC.rotation, _targetStartRotation, completionRatio);
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
                revLoopSound.Pitch = 0.6f;
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

            if(Timer % 5 == 0)
            {
                Vector2 spawnPos = NPC.Center + Main.rand.NextVector2Circular(32, 32);
                var p = LegacyParticle.NewParticle<ZapParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), newColor: Color.Red, Main.rand.NextFloat(0.5f, 1f));
                p.innerColor = Color.White;
                p.outerColor = Color.Red;
                if (Main.rand.NextBool(2))
                {
                    SoundStyle zapSound = SoundID.DD2_LightningAuraZap;
                    zapSound.PitchVariance = 0.5f;
                    zapSound.Pitch = 0.66f;
                    SoundEngine.PlaySound(zapSound, NPC.position);
                    var spark = LegacyParticle.NewParticle<SparkParticle>(spawnPos, Main.rand.NextVector2Circular(4, 4), Scale: Main.rand.NextFloat(0.5f, 1f));
                    spark.innerColor = Color.White;
                    spark.outerColor = Color.Red;
                    spark.fadeToColor = Color.DarkBlue;
                }
            }

            if(Timer % 5 == 0)
            {
                var d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.Firework_Red);
                Vector2 upVelocity = -Vector2.UnitY * 5;
                Main.dust[d].velocity += upVelocity;

                d = Dust.NewDust(NPC.position, NPC.width, NPC.height, DustID.FireworkFountain_Red);
                upVelocity = -Vector2.UnitY * 2;
                Main.dust[d].velocity += upVelocity;
            }

            if(Timer % 6 == 0)
            {
                var donut = LegacyParticle.NewParticle<GlowDonutParticle>(NPC.Center, -NPC.velocity, Color.Red);
                donut.shrink = true;
                donut.innerColor = Color.Yellow;
                donut.outerColor = Color.Red;
                donut.fadeToColor = Color.Black;
            }
            _revvedUp = true;

            telegraphLineColor *= 0.5f;
            heldLightningScale = 1f;
            float sawTime = 80;
            Vector2 dashVelocity = NPC.rotation.ToRotationVector2() * 30f;

            NPC.rotation -= 0.001f;

            float completionRatio = Timer / sawTime;
            float ease = EasingFunction.Anticipation2(completionRatio / 0.25f);
            Vector2 velocity = Vector2.Lerp(-dashVelocity * 0.5f, dashVelocity, ease);
            NPC.velocity = velocity;


            afterImageStrength = EasingFunction.QuadraticBump(completionRatio);
            TargetOutlineColor = Color.Red;

            SetAngles(MathHelper.Lerp(-135, -90, ease));
            if (Timer >= sawTime)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
