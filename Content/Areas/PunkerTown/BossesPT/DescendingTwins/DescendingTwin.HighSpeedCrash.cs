using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles;
using Stellamod.Helpers;
using Stellamod.UI.Systems;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        private Vector2 _highSpeedTargetPosition;
        private Vector2 GetHighSpeedCrashStartOffset()
        {
            float distanceOffset = 200;
            switch (Variant)
            {
                default:
                case TwinVariant.Spazz:
                    return -Vector2.UnitX * distanceOffset;
                case TwinVariant.Retina:
                    return Vector2.UnitX * distanceOffset;
            }
        }

        private void AI_HighSpeedCrashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
            }            /*
             * 
             * Both of them aim above you, shooting a type of fire (Descender Retina), shoots a red fire,
             * while Descender Spazz, shoots a green flame, and they make a crossing sword, and continuously going downwards, making you dodge
             */

            //So first we need to get them ina  good position for doing this attack
            //I think it'd be best if they position themselves on opposite sides of you
            //Alright so
            //First let's get that position and move to it
            Vector2 highSpeedCrashStartOffset = GetHighSpeedCrashStartOffset();
            Vector2 positionToMoveTo = Target.Center + highSpeedCrashStartOffset;


            float windupTime = 80f;
            if (_phaseShift)
            {
                windupTime *= 0.8f;
            }

            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 movementVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, movementVelocity, completionRatio);

            //Look at the player
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = NPC.rotation;

            //There's no afterimage on this preparation state
            _afterImageAlpha = 0f;

            //Alert the player that something is about to happen fr
            TargetOutlineColor = Color.Yellow;

            //Here we wait a bit longer before they do the sword so that you get a bit of time to react
            if (Timer >= windupTime * 1.3f)
            {
                SwitchState(TwinAIState.HighSpeedCrashWindup);
            }
        }
        private void AI_HighSpeedCrashQuickStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
                _simpleDashNormal = NPC.velocity;
            }           
            
            /*
             * 
             * Both of them aim above you, shooting a type of fire (Descender Retina), shoots a red fire,
             * while Descender Spazz, shoots a green flame, and they make a crossing sword, and continuously going downwards, making you dodge
             */

            //So first we need to get them ina  good position for doing this attack
            //I think it'd be best if they position themselves on opposite sides of you
            //Alright so
            //First let's get that position and move to it
            Vector2 highSpeedCrashStartOffset = GetHighSpeedCrashStartOffset();
            highSpeedCrashStartOffset = highSpeedCrashStartOffset.RotatedBy(AttackNumber * MathHelper.PiOver4);
            Vector2 positionToMoveTo = Target.Center + highSpeedCrashStartOffset;


            float windupTime = 50f;
            if (_phaseShift)
            {
                windupTime *= 0.8f;
            }

            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutSine(completionRatio);
            Vector2 movementVelocity = (positionToMoveTo - NPC.Center);
            NPC.velocity = Vector2.Lerp(_simpleDashNormal, movementVelocity, ease);

            //Look at the player
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineRot = NPC.rotation;

            //There's no afterimage on this preparation state
            _afterImageAlpha = 0f;

            //Alert the player that something is about to happen fr
            TargetOutlineColor = Color.Yellow;

            //Here we wait a bit longer before they do the sword so that you get a bit of time to react
            if (Timer >= windupTime * 1.3f)
            {
                SwitchState(TwinAIState.HighSpeedCrashWindup);
            }
        }

        private void AI_HighSpeedCrashWindup()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = NPC.rotation.ToRotationVector2();
                _highSpeedTargetPosition = Target.Center;
            }

            if (Timer % 5 == 0)
            {
                SpawnFlameDust();
            }
            //High speed crash set the rotation
            float windupTime = 30f;
            if (_phaseShift)
            {
                windupTime *= 0.8f;
            }

            float completionRatio = Timer / windupTime;
            float directionToRotate = _simpleDashNormal.X > 0 ? 1f : -1f;
            float radiansToRotateBy = MathHelper.Lerp(0f, -MathHelper.Pi * directionToRotate, completionRatio);

            Vector2 newDashNormal = _simpleDashNormal.RotatedBy(radiansToRotateBy);
            NPC.rotation = newDashNormal.ToRotation();
            _afterImageAlpha = MathHelper.Lerp(0f, 1f, completionRatio);
            _telegraphLineAlpha = MathHelper.Lerp(1f, 0f, completionRatio);

            float speed = MathHelper.Lerp(4f, 50f, completionRatio);
            NPC.velocity = newDashNormal * speed;
            TargetOutlineColor = Color.Yellow;
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.HighSpeedCrashPreDash);
            }
        }

        private void AI_HighSpeedCrashPreDash()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = (_highSpeedTargetPosition - NPC.Center).SafeNormalize(Vector2.Zero);
                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }
            float windUpTime = 15f;
            if (_phaseShift)
            {
                windUpTime *= 0.8f;
            }

            float completionRatio = Timer / windUpTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            NPC.velocity = Vector2.Lerp(Vector2.Zero, _simpleDashNormal * 5f, ease);
            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.1f);

            _telegraphLineRot = _simpleDashNormal.ToRotation();
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(completionRatio));
            if (Timer >= windUpTime)
            {
                SwitchState(TwinAIState.HighSpeedCrashCrash);
            }
        }

        private void AI_HighSpeedCrashCrash()
        {
            Timer++;
            if (Timer == 1)
            {
                _simpleDashNormal = (_highSpeedTargetPosition - NPC.Center).SafeNormalize(Vector2.Zero);
                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
                    AssetRegistry.Sounds.SteamPunking.DescendingDash1
                    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
            }

            _afterImageAlpha = 1f;
            ShakeModSystem.Shake = 2;

            if (Timer % 5 == 0)
            {
                SpawnSteamParticle();
            }

            if (Timer % 2 == 0)
            {

                SpawnFlameDonut();
                SpawnFlameDust();
            }

            //We need to zoom really quickly to the target position
            //Not sure how to do that tbh
            float dashTime = 25f;
            float completionRatio = Timer / dashTime;
            float dashSpeed = MathHelper.Lerp(25f, 65, completionRatio);
            NPC.velocity = _simpleDashNormal * dashSpeed;
            NPC.rotation = Utils.AngleLerp(NPC.rotation, NPC.velocity.ToRotation(), 0.5f);

            if (Timer == (int)(dashTime - 5))
            {

                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DescendingBigBoom>(),
                        DescendingBigBoomDamage, 1, Main.myPlayer, ai1: (int)Variant);
                }
            }
            //Enable the contact damage as per usual
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= dashTime)
            {
                SwitchState(TwinAIState.HIghSpeedCrashEnd);
            }
        }

        private void AI_HighSpeedCrashEnd()
        {
            Timer++;
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            NPC.rotation = Utils.AngleLerp(NPC.rotation, targetAngle, 0.1f);

            //Step 2. Check the distance between this current twin and the player
            //If the distance is too far we'll move closer to them, if not we just slow down/sit there
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float maxDistance = 400;
            if (distanceToTarget > maxDistance)
            {
                //We should scale the movement velocity based on the distance, so the farther they are the faster we'll move
                Vector2 movementVelocity = targetNormal * distanceToTarget / 32f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, movementVelocity, 0.1f);
            }
            else
            {
                //Otherwise, we'll just slow down
                //We want to keep a little bit of movement velocity so it's not just completely static
                NPC.velocity *= 0.8f;

                //Stpe 3. Add a little bit of hovering velocity for a cool effect
                float yHover = MathF.Sin(Timer * 0.1f) * 0.5f;
                NPC.velocity.Y += yHover;
            }

            if (Timer >= 42)
            {
                AttackNumber++;
                if (AttackNumber < 6)
                {
                    SwitchState(TwinAIState.HighSpeedCrashQuickStart);
                }
                else
                {
                    SwitchState(TwinAIState.Idle);
                }

            }
        }
    }
}
