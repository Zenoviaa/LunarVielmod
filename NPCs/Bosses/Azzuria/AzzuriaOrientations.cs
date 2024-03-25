using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using Terraria;

namespace Stellamod.NPCs.Bosses.Azzuria
{
    internal partial class Azzuria
    {
        public float TargetHeadRotation;
        public float TargetSegmentRotation;

        public float OscTimer;
        public float OscDir = 1;
        private void UpdateOrientation()
        {
            //This makes it smoothly enter the thingy
            HeadRotation = MathHelper.Lerp(HeadRotation, TargetHeadRotation, 0.2f);
            SegmentTurnRotation = MathHelper.Lerp(SegmentTurnRotation, TargetSegmentRotation, 0.2f);

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
            NPC.velocity = new Vector2(0, VectorHelper.Osc(-1, 1));
        }

        private void OrientArching()
        {
            TargetSegmentRotation = -(MathHelper.PiOver4 / Total_Segments);
            TargetHeadRotation = MathHelper.PiOver4;
        }

        private void OrientStraight()
        {
            TargetSegmentRotation = -(MathHelper.PiOver4 / Total_Segments);
            TargetHeadRotation = 0;
        }

        private void LookAtTarget()
        {
            Player target = Main.player[NPC.target];
            float distanceToTarget = Vector2.Distance(NPC.Center, target.Center);
            float tiles = 32f;
            Vector2 directionToTarget = NPC.Center.DirectionTo(target.Center);
            if(distanceToTarget < tiles.TilesToDistance())
            {
                TargetHeadRotation = NPC.Center.DirectionTo(target.Center).ToRotation();
            } else
            {
                TargetHeadRotation = MathHelper.PiOver4;
            }
        }
    }
}
