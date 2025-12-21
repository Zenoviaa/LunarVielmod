using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;

using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public partial class E
    {
        /*
         * 
         * Does a charge into Dash that goes right to you, and explodes, 
         * you just have to time the dash right
         */

        private bool IsTransformed()
        {
            return Animator.GetAnimation() == Anim_Swimming;
        }
        private void TransformAndSwimAnimation()
        {
            if (!IsTransformed())
            {
                Animator.PlayAnimation(Anim_Morph);
                if (Animator.IsFinished())
                {
                    Animator.PlayAnimation(Anim_Swimming);
                }
            } else
            {
                Animator.PlayAnimation(Anim_Swimming);
            }
        }
        private void AI_BlackDashStart()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.Center;
               
            }

            float startupTime = 60f;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 start = TargetVector;
            Vector2 direction = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            Vector2 end = MyTarget.Center - direction * 120;
            Vector2 positionToMoveTo = Vector2.Lerp(start, end, ease);
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            NPC.velocity = targetVelocity;
            NPC.direction = TargetDirection;

            Animator.PlayAnimation(Anim_FoundYou);
            if(Timer >= startupTime)
            {
                SwitchState(AIState.BlackDashPreDash);
            }
        }

        private void SetSharkRotation(float targetAngle)
        {
            NPC.rotation = targetAngle;
            if (NPC.direction == 1)
                NPC.rotation -= MathHelper.Pi;
        }
        private void AI_BlackDashPreDash()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            TransformAndSwimAnimation();

            float preDashTime = 60f;

            _telegraphLineRot = (MyTarget.Center - NPC.Center).ToRotation();
            _telegraphLineAlpha = MathHelper.Lerp(0f, 1f, EasingFunction.QuadraticBump(Timer / preDashTime));
            SetSharkRotation(_telegraphLineRot);
            NPC.direction = -TargetDirection;
            if (Timer >= preDashTime)
            {
                SwitchState(AIState.BlackDashDash);
            }
        }

        private void AI_BlackDashDash()
        {
            Timer++;
            if(Timer == 1)
            {
       
                _forwardVector = NPC.velocity;
            }

            if(Timer < 12)
            {
                TargetVector = (MyTarget.Center - NPC.Center).SafeNormalize(Vector2.Zero);
            }

            float dashTime = 25;
            float completionRatio = Timer / dashTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);

            _extraAfterImageAlpha = MathHelper.Lerp(0f, 0.7f, ease);
            _contactDamage = true;

            Vector2 dashVelocity = TargetVector * 100;
            NPC.velocity = Vector2.Lerp(_forwardVector, dashVelocity, ease);
            SetSharkRotation(NPC.velocity.ToRotation());
            Animator.PlayAnimation(Anim_Swimming);
            if (Timer >= dashTime)
            {
                SwitchState(AIState.BlackDashEnd);
            }
        }

        private void AI_BlackDashEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            NPC.rotation = 0;
            if(Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
