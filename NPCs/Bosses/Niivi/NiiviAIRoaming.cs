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
        private void AIRoaming()
        {
            if (!Main.dayTime)
            {
                AIRoaming_GoHome();
            }
            else
            {
                AIRoaming_FlyAroundTree();
            }
        }

        private void AIRoaming_FlyAroundTree()
        {
            float orbitDistance = 2000;
            Vector2 home = AlcadSpawnSystem.NiiviSpawnWorld + new Vector2(0, 1024);
            Vector2 direction = home.DirectionTo(NPC.Center);
            direction = direction.RotatedBy(MathHelper.TwoPi / 2000);
            Vector2 targetCenter = home + direction * orbitDistance;
            Vector2 directionToTargetCenter = NPC.Center.DirectionTo(targetCenter);
            AI_MoveToward(targetCenter, 2);
            OrientArching();
            if (directionToTargetCenter.X > 0)
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;
     
            }
            else
            {
                FlightDirection = -1;
                LookDirection = -1;
                StartSegmentDirection = Vector2.UnitX;
                TargetHeadRotation = -MathHelper.PiOver4 + MathHelper.Pi;
            }

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
                ResetState(ActionState.Sleeping);
            }

            UpdateOrientation();
            LookAtTarget();
        }
    }
}
