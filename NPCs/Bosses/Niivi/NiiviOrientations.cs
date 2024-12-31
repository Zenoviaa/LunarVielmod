using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using Terraria;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class Niivi
    {
        public float LookTimer;
        public float LookDirection = -1;

        public float TargetHeadRotation;
        public float TargetSegmentRotation;
        public Vector2 TargetSegmentDirection;
        public float OscTimer;
        public float OscDir = 1;

        public float OrientationSpeed = 0.2f;
        private void UpdateOrientation()
        {
            //This makes it smoothly enter the thingy


            float duration = 300f;
            //Oscillate
            OscTimer +=  OscDir;
            if(OscTimer >= duration)
            {
                OscDir = -1;
            } 
            else if (OscTimer <= 0)
            {
                OscDir = 1;
            }

            float progress = OscTimer / duration;
            float sinOsc = MathF.Sin(progress * -16);
            float rotOsc = progress * MathHelper.PiOver4 / 16;

            SegmentPosOsc = new Vector2(0, sinOsc);
            SegmentRotationOsc = rotOsc;


            if (LookDirection < 0)
            {
                LookTimer -= 0.01f;
                if (LookTimer <= 0)
                    LookTimer = 0;
            }
            else
            {
                LookTimer += 0.01f;
                if (LookTimer >= 1)
                    LookTimer = 1;
            }
            float lookProgress = LookTimer / 1f;
            float easedProgress = Easing.InOutCirc(lookProgress);
            CurrentFlightDirection = MathHelper.Lerp(-1, 1, easedProgress);
            StartSegmentDirection = Vector2.Lerp(Vector2.UnitX, -Vector2.UnitX, easedProgress);

            float targetHeadRotation = TargetHeadRotation;
            if (lookProgress <= 0.5f)
            {
                targetHeadRotation += MathHelper.Pi;
                targetHeadRotation = MathHelper.WrapAngle(targetHeadRotation);
            }

            HeadRotation = MathHelper.Lerp(HeadRotation, targetHeadRotation, 0.04f);
            if(lookProgress >= 0.5f)
            {
                HeadRotation = MathHelper.Clamp(HeadRotation, -0.8f, 1f);
            }
            else
            {
                HeadRotation = MathHelper.Clamp(HeadRotation, -1f, 0.8f);
            }
            SegmentTurnRotation = MathHelper.Lerp(SegmentTurnRotation, TargetSegmentRotation, 0.04f);
        }

        private void OrientArching()
        {
            TargetSegmentRotation = (MathHelper.PiOver4 / Total_Segments) * 1.3f * -LookDirection;
            TargetHeadRotation = MathHelper.PiOver4;
        }

        private void OrientStraight()
        {
            TargetSegmentRotation = (MathHelper.PiOver4 / Total_Segments) / 3;
            TargetHeadRotation = 0;
        }

        private void FlipToDirection()
        {
            if (LookDirection < 0)
            {
                FlightDirection = -1;
                TargetSegmentDirection = Vector2.UnitX;
            }
            else
            {
                FlightDirection = 1;
                TargetSegmentDirection = -Vector2.UnitX;
            }

        }

        private void LookAtTarget()
        {
            Player target = Main.player[NPC.target];
            float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
            float tiles = 96;
            Vector2 directionToTarget = NPC.Center.DirectionTo(target.Center);
            if(distanceToTarget < tiles.TilesToDistance())
            {
                TargetHeadRotation = NPC.Center.DirectionTo(target.Center).ToRotation() * LookDirection;
            } else
            {
                TargetHeadRotation = MathHelper.PiOver4;
            }
        }
    }
}
