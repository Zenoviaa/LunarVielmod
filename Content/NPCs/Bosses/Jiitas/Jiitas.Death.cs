using Stellamod.Assets;
using Stellamod.Core.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas
    {
        public float DeathPullupTime
        {
            get
            {
                return 30;
            }
        }

        public float DeathWaitTime
        {
            get
            {
                return 120;
            }
        }
        private void AI_Death()
        {
            switch (ActionStep)
            {
                case 0:
                    AI_DeathPullup();
                    break;
                case 1:
                    AI_DeathDeath();
                    break;
            }
        }

        private void AI_DeathPullup()
        {
            Timer++;
            NPC.noGravity = true;
            NPC.velocity *= 0.9f;
            NPC.rotation *= 0.9f;
            PlayAnimation(AnimationState.Dragup);
            if(Timer >= DeathPullupTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_DeathDeath()
        {
            Timer++;
            if(Timer == 1)
            {
                FXUtil.FocusCamera(NPC.Center, DeathWaitTime);
                SoundStyle jiitasSad = AssetRegistry.Sounds.Jiitas.JiitasSadWah;
                SoundEngine.PlaySound(jiitasSad, NPC.position);
            }

            NPC.noGravity = true;
            NPC.rotation *= 0.9f;
            NPC.velocity.Y -= 0.3f;
            PlayAnimation(AnimationState.Death);

            if(Timer >= DeathWaitTime)
            {
               
                NPC.Kill();
            }
        }
    }
}
