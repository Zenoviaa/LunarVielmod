using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.NPCs.Town;
using Terraria;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class Niivi
    {

        public float RoamingDirection = -1f;
        public float RoamingSpeed = 2;
        public float RoamingTime;

        private void AIRoaming()
        {
            if (!Main.dayTime)
            {
                AIRoaming_GoHome();
            }
            else
            {
                AIRoaming_Fly();
            }
        }

        private void AIRoaming_Fly()
        {
            float maxRoamingTime = 2400;
            NPC.TargetClosest();
            //Fly to the left of the world and back
            RoamingTime++;
            if (RoamingTime > maxRoamingTime)
            {
                RoamingTime = 0;
                RoamingDirection = -RoamingDirection;
            }

            if (RoamingDirection < 0)
            {
                FlightDirection = -1;
                LookDirection = -1;
                StartSegmentDirection = Vector2.UnitX;
                OrientStraight();
                TargetHeadRotation = NPC.velocity.ToRotation();
            }
            else
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;
                OrientStraight();
                TargetHeadRotation = NPC.velocity.ToRotation();
            }

            Vector2 targetVelocity = Vector2.UnitX * RoamingSpeed * RoamingDirection;
            targetVelocity += new Vector2(0, VectorHelper.Osc(-1, 1));
            NPC.velocity = targetVelocity;

            UpdateOrientation();
            LookAtTarget();
        }

        private void AIRoaming_GoHome()
        {
            Vector2 home = AlcadSpawnSystem.NiiviSpawnWorld;
            Vector2 directionToHome = NPC.Center.DirectionTo(home);
            float distanceToHome = Vector2.Distance(NPC.Center, home);

            //Set orientation
            if(directionToHome.X > 0)
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;
                OrientStraight();
                TargetHeadRotation = NPC.velocity.ToRotation();

            }
            else
            {
                FlightDirection = -1;
                LookDirection = -1;
                StartSegmentDirection = Vector2.UnitX;
                OrientStraight();
                TargetHeadRotation = NPC.velocity.ToRotation();
            }


            float speed = MathHelper.Min(RoamingSpeed, distanceToHome);
            Vector2 targetVelocity = directionToHome * speed;
            targetVelocity += new Vector2(0, VectorHelper.Osc(-1, 1));
            NPC.velocity = targetVelocity;

            if (distanceToHome <= 1)
            {
                State = ActionState.Sleeping;
            }

            UpdateOrientation();
            LookAtTarget();
        }
    }
}
