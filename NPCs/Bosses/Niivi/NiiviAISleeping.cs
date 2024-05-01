using Microsoft.Xna.Framework;
using ParticleLibrary;
using Stellamod.NPCs.Town;
using Stellamod.Particles;
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
        int SleepingTimer;
        private void AISleeping()
        {
            if (Main.dayTime)
            {
                SleepingTimer = 0;
                State = ActionState.Roaming;
            }
            else
            {
                FlightDirection = 1;
                LookDirection = 1;
                StartSegmentDirection = -Vector2.UnitX;

                //Go sleep
                Vector2 sleepPos = AlcadSpawnSystem.NiiviSpawnWorld + new Vector2(0, 164);
                NPC.Center = Vector2.Lerp(NPC.Center, sleepPos, 0.01f);
                TargetSegmentRotation = -MathHelper.PiOver4 / 80;
                TargetHeadRotation = 0;
                SleepingTimer++;
                if(SleepingTimer > 60 && SleepingTimer % 60 == 0)
                {
                    ParticleManager.NewParticle<ZeeParticle>(NPC.Center + new Vector2(64, -32), -Vector2.UnitY, Color.White, 1f);
                }
            }
            UpdateOrientation();
        }
    }
}
