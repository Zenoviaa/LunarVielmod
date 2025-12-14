using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins.Projectiles;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.BossesPT.DescendingTwins
{
    public partial class DescendingTwin
    {
        private Vector2 GetBounceDashAnchorPoint()
        {
            return Commander.GetBouncingDashAnchorPoint();
        }


        private void AI_BouncingDashStart()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
                _startVelocity = NPC.velocity;
                _startRotation = NPC.rotation;
                SoundStyle beepSound = AssetRegistry.Sounds.SteamPunking.DescendingBeep;
                beepSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(beepSound, NPC.position);
            }


            //More wind up time for this attack since it kinda just happens
            float windupTime = 100f;
            float completionRatio = Timer / windupTime;
            float ease = EasingFunction.InOutSine(completionRatio);

            Vector2 anchorPoint = GetBounceDashAnchorPoint();

            float offsetDistance = 300f;
            Vector2 bounceOffset = -Vector2.UnitY * offsetDistance;
            Vector2 startPoint = anchorPoint + bounceOffset;
            Vector2 velocityThere = (startPoint - NPC.Center);
            NPC.velocity = Vector2.Lerp(_startVelocity, velocityThere, ease);

            float targetAngle = Vector2.UnitY.ToRotation();
            NPC.rotation = Utils.AngleLerp(_startRotation, targetAngle, ease);

            _simpleDashNormal = (NPC.Center - anchorPoint).SafeNormalize(Vector2.Zero);
            TargetOutlineColor = Color.Yellow;
            if (Timer >= windupTime)
            {
                SwitchState(TwinAIState.BouncingDashIn);
            }
        }


        private void AI_BouncingDashAnchor()
        {
            Timer++;
            if (Timer == 1)
            {
                SetTargetToCommanderTarget();
            }

            _afterImageAlpha = 1f;
            //So we should slowly move towards the player if they're far, if not we'll just hover in place.
            //Step 1. Look towards the player, we can do this by calculating a target normal, calculating an angle and then lerping to it
            Vector2 targetNormal = TargetNormal;
            float targetAngle = targetNormal.ToRotation();
            //Step 2. Check the distance between this current twin and the player
            //If the distance is too far we'll move closer to them, if not we just slow down/sit there
            float distanceToTarget = Vector2.Distance(NPC.Center, Target.Center);
            float maxDistance = 450;
            if (distanceToTarget > maxDistance)
            {
                //We should scale the movement velocity based on the distance, so the farther they are the faster we'll move
                Vector2 movementVelocity = targetNormal * distanceToTarget / 32f;
                NPC.velocity = Vector2.Lerp(NPC.velocity, movementVelocity, 0.05f);
            }
            else
            {
                //Otherwise, we'll just slow down
                //We want to keep a little bit of movement velocity so it's not just completely static
                NPC.velocity = Vector2.Lerp(NPC.velocity, targetNormal * 3f, 0.1f);

                //Stpe 3. Add a little bit of hovering velocity for a cool effect
                float yHover = MathF.Sin(Timer * 0.1f) * 0.5f;
                NPC.velocity.Y += yHover;
            }

            NPC.rotation += MathHelper.Lerp(0f, 0.2f, EasingFunction.InOutSine(Timer / 120f));
            TargetOutlineColor = Color.Yellow;
            //Receive the next command state.
            //This should be automatically netcoded btw
            if (NextCommandState == TwinAIState.BouncingDashEnd || NextCommandState == TwinAIState.PhaseShiftStart)
            {
                SwitchState(NextCommandState);
                NextCommandState = TwinAIState.Idle;
            }
        }

        private void AI_BouncingDashIn()
        {
            _rotationTimer++;
            Timer++;
            if (Timer == 1)
            {
                //Play a cool little dash sound
                //Wait, I have an idea for how this can sound like
                SoundStyle dashSound = AttackNumber % 2 == 0 ?
                    AssetRegistry.Sounds.SteamPunking.DescendingDash1
                    : AssetRegistry.Sounds.SteamPunking.DescendingDash2;
                dashSound.PitchVariance = 0.3f;
                SoundEngine.PlaySound(dashSound, NPC.position);
                _startRotation = NPC.rotation;
            }

            float inTime = 30f;
            float completionRatio = Timer / inTime;
            float ease = EasingFunction.InExpo(completionRatio);
            Vector2 anchorPoint = GetBounceDashAnchorPoint();
            float offsetDistance = MathHelper.Lerp(300f, 0f, ease);


            Vector2 bounceOffset = _simpleDashNormal * offsetDistance;
            bounceOffset = bounceOffset.RotatedBy(_rotationTimer * 0.05f);

            Vector2 targetPosition = anchorPoint + bounceOffset;
            Vector2 targetVelocity = (targetPosition - NPC.Center);
            NPC.velocity = targetVelocity;


            if(AttackNumber == 0)
            {
                float targetAngle = NPC.velocity.ToRotation();
                NPC.rotation = Utils.AngleLerp(_startRotation, targetAngle, EasingFunction.InOutSine(completionRatio));
            }
            else
            {
                float targetAngle = NPC.velocity.ToRotation();
                NPC.rotation = targetAngle;

            }

            _afterImageAlpha = 1f;
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= inTime)
            {
                SwitchState(TwinAIState.BouncingDashOut);
            }
        }

        private void AI_BouncingDashOut()
        {
            _rotationTimer++;
            Timer++;
            if (Timer == 1)
            {
                if (MultiplayerHelper.IsHost)
                {
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, Vector2.Zero, ModContent.ProjectileType<DescendingBigBoom>(),
                        DescendingBigBoomDamage, 1, Main.myPlayer, ai1: GetVariant());
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, TargetNormal * 9f, ModContent.ProjectileType<DescendingFire>(),
                        DescendingFireDamage, 1, Main.myPlayer, ai1: GetVariant());
                }
            }

            float inTime = 30f;
            float completionRatio = Timer / inTime;
            float ease = EasingFunction.OutExpo(completionRatio);
            Vector2 anchorPoint = GetBounceDashAnchorPoint();

            float offsetDistance = MathHelper.Lerp(0f, 300f, ease);

            Vector2 bounceOffset = _simpleDashNormal * offsetDistance;
            bounceOffset = bounceOffset.RotatedBy(_rotationTimer * 0.05f);

            Vector2 targetPosition = anchorPoint + bounceOffset;
            Vector2 targetVelocity = (targetPosition - NPC.Center);
            NPC.velocity = targetVelocity;
            NPC.rotation = NPC.velocity.ToRotation();

            _afterImageAlpha = 1f;
            _contactDamage = true;
            TargetOutlineColor = Color.Red;
            if (Timer >= inTime)
            {
                AttackNumber++;
                if (AttackNumber >= 16)
                {
                    SwitchState(TwinAIState.BouncingDashEnd);
                }
                else
                {
                    SwitchState(TwinAIState.BouncingDashIn);
                }
            }
        }

        private void AI_BouncingDashEnd()
        {
            Timer++;
            NPC.velocity *= 0.8f;
            if (Timer >= 15f)
            {
                SwitchState(TwinAIState.Idle);
            }
        }
    }
}
