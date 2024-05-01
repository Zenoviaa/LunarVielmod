using Microsoft.Xna.Framework;
using Stellamod.NPCs.Town;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;

namespace Stellamod.NPCs.Bosses.Niivi
{
    internal partial class Niivi
    {
        private void AISleeping()
        {
            if (Main.dayTime)
            {
                State = ActionState.Roaming;
            }
            else
            {
                //Go sleep
                Vector2 sleepPos = AlcadSpawnSystem.NiiviSpawnWorld + new Vector2(0, 164);
                NPC.Center = Vector2.Lerp(NPC.Center, sleepPos, 0.01f);
                TargetSegmentRotation = -MathHelper.PiOver4 / 80;
                TargetHeadRotation = 0;
            }
            UpdateOrientation();

        }
    }
}
