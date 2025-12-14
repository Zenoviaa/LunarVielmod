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
                NPC.TargetClosest();

            }

            float windupTime = 30f;
            float completionRatio = Timer / windupTime;

            Vector2 anchorPoint = GetBounceDashAnchorPoint();
            Vector2 velocityThere = (anchorPoint - NPC.Center).SafeNormalize(Vector2.Zero);
            NPC.velocity = Vector2.Lerp(NPC.velocity, velocityThere, 0.1f);

            _simpleDashNormal = (NPC.Center - anchorPoint).SafeNormalize(Vector2.Zero);
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
                NPC.TargetClosest();
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
            if (NextCommandState == TwinAIState.BouncingDashEnd)
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
            NPC.rotation = NPC.velocity.ToRotation();

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
                        DescendingBigBoomDamage, 1, Main.myPlayer, ai1: (int)Variant);
                    Projectile.NewProjectile(NPC.GetSource_FromThis(), NPC.Center, TargetNormal * 9f, ModContent.ProjectileType<DescendingFire>(),
                        DescendingFireDamage, 1, Main.myPlayer, ai1: (int)(1 - Variant));
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
