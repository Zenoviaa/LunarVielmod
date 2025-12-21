using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Illuria.BossesIL.EStyr
{
    public class FlyingKickPlayer : ModPlayer
    {
        public Vector2? kickVelocity;
        public float flyTime;
        public float flyingTimer;
        public override void PreUpdateMovement()
        {
            base.PreUpdateMovement();
            if (kickVelocity.HasValue)
            {
                Player.velocity = kickVelocity.Value;
                kickVelocity = null;
            }
        }
    }

    public partial class E
    {
        /*
         * Runs up to you, and tries to kick you, 
         * if you get hit you go flying and 
         * he runs over while you're in the air and 
         * tries to throw the sword directly down into a giant fountain like explosion, getting hit by both instantly means death.
         * 
         * 
         */

        private void AI_KickStart()
        {
            Timer++;
            if(Timer == 1)
            {
                NPC.TargetClosest();
                TargetVector = NPC.velocity;
            }

            float startupTime = 60f;
            float completionRatio = Timer / startupTime;
            float ease = EasingFunction.InOutExpo7(completionRatio);
            Vector2 offsetDirection = MyTarget.Center.X > NPC.Center.X ? -Vector2.UnitX * 64: Vector2.UnitX * 64;
            Vector2 positionToMoveTo = MyTarget.Center + offsetDirection;
            Vector2 targetVelocity = positionToMoveTo - NPC.Center;
            Vector2 easeVelocity = Vector2.Lerp(TargetVector, targetVelocity, ease);
            NPC.velocity = easeVelocity;
            NPC.direction = offsetDirection.X > 0 ? 1 : -1;
            if(Timer >= startupTime)
            {
                SwitchState(AIState.Kick_Run);
            }
        }

        private void AI_KickRun()
        {
            Timer++;
            if(Timer == 1)
            {
                TargetVector = NPC.Center;
            }
            float anticipationTime = 60;
            float completionRatio = Timer / anticipationTime;
            float ease = EasingFunction.Anticipation2(completionRatio);
            Vector2 start = TargetVector;
            Vector2 end = start + new Vector2(-NPC.direction * 64, 0);
            Vector2 interpolatedPosition = Vector2.Lerp(start, end, ease);
            Vector2 targetVelocity = interpolatedPosition - NPC.velocity;
            NPC.velocity = targetVelocity;
            _extraAfterImageAlpha = MathHelper.Lerp(0f, 0.7f, completionRatio);
            if(Timer >= anticipationTime)
            {
                float distanceToTarget = Vector2.Distance(NPC.Center, MyTarget.Center);
                if(distanceToTarget <= 100)
                {
                    SwitchState(AIState.Kick_Kick);
                }
                else
                {
                    SwitchState(AIState.Kick_Fail);
                }
            }
        }

        private void AI_KickKick()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if(Timer >= 15)
            {
                SwitchState(AIState.Kick_Fly);
            }
        }


        private void AI_KickFail()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if(Timer >= 15)
            {
                SwitchState(AIState.Kick_End);
            }
        }

        private void AI_KickFly()
        {

        }

        private void AI_KickSwordThrowDown()
        {

        }

        private void AI_KickEnd()
        {
            Timer++;
            NPC.velocity *= 0.9f;
            if (Timer >= 15)
            {
                SwitchState(AIState.Idle);
            }
        }
    }
}
