using Terraria;

namespace Stellamod.Core.Utilities;

public static partial class CombatUtil
{
    extension(NPC npc)
    {
        /// <summary>
        /// Faces the current target by changing both the sprite direction and direction
        /// </summary>
        public void SpriteFaceTarget()
        {
            var player = Main.player[npc.target];
            if (player.Center.X < npc.Center.X)
                npc.spriteDirection = -1;
            else
                npc.spriteDirection = 1;
            npc.direction = npc.spriteDirection;
        }


        /// <summary>
        /// Slows down the x velocity and makes sure tile collide and gravity are both enabled
        /// </summary>
        public void StayGroundedAndRooted()
        {
            npc.velocity.X *= 0.96f;
            npc.noTileCollide = false;
            npc.noGravity = false;
        }
    }
}
