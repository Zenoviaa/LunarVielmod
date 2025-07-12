using Microsoft.Xna.Framework;
using Stellamod.Assets;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas
    {
        public float EmpowerStartupTime
        {
            get
            {
                return 120;
            }
        }
        private void AI_Empower()
        {
            switch (ActionStep)
            {
                case 0:
                    AI_EmpowerLaugh();
                    break;
            }
        }

        private void AI_EmpowerLaugh()
        {
            Timer++;
            if(Timer == 1)
            {
                CombatText.NewText(NPC.getRect(), Color.Yellow, "Power Up!", true);
                SoundStyle laughSound = AssetRegistry.Sounds.Jiitas.JiitasLaugh;
                laughSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(laughSound, NPC.position);
            }
            PlayAnimation(AnimationState.Laugh);
            Empowered = true;
            if(Timer >= EmpowerStartupTime)
            {
                SwitchState(ActionState.Idle);
            }
        }
    }
}
