using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;

namespace Stellamod.NPCs.Bosses.Azzuria
{
    internal partial class Azzuria
    {
        //Number of tiles to fly off
        public const float Max_Roaming_Time = 7200;

        public float RoamingDirection = -1f;
        public float RoamingSpeed = 2;
        public float RoamingTime;

        private void AIRoaming()
        {
            //Fly to the left of the world and back
            RoamingTime++;
            if(RoamingTime > Max_Roaming_Time)
            {
                RoamingTime = 0;
                RoamingDirection = -RoamingDirection;
            }

            if(RoamingDirection < 0)
            {
                FlightDirection = -1;
                StartSegmentDirection = Vector2.UnitX;
                OrientStraight();
            }
            else
            {
                FlightDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;
                OrientStraight();
            }

            Vector2 targetVelocity = Vector2.UnitX * RoamingSpeed * RoamingDirection;  
            targetVelocity += new Vector2(0, VectorHelper.Osc(-1, 1));
            NPC.velocity = targetVelocity;

            UpdateOrientation();
            LookAtTarget();
        }
    }
}
