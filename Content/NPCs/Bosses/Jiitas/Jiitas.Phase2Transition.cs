using Microsoft.Xna.Framework;
using Stellamod.Assets;
using Stellamod.Core.Helpers;
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
        public float Phase2FalldownTime
        {
            get
            {
                return 60;
            }
        }
        public float Phase2GetupTime
        {
            get
            {
                return 30;
            }
        }
        public float Phase2LaughTime
        {
            get
            {
                return 180;
            }
        }

        private void AI_Phase2Transition()
        {
            _attackCycle.Clear();
            switch (ActionStep)
            {
                case 0:
                    AI_FallDown();
                    break;
                case 1:
                    AI_GetUp();
                    break;
                case 2:
                    AI_ImSoReady();
                    break;
            }
        }

        private void AI_FallDown()
        {
            Timer++;
            if (Timer == 1)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasSit;
                jiitasSit.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jiitasSit, NPC.position);
            }

            NPC.velocity *= 0f;
            NPC.dontTakeDamage = true;
            PlayAnimation(AnimationState.Sitdown);
            if(Timer >= Phase2FalldownTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_GetUp()
        {
            Timer++;
            NPC.velocity *= 0f;
            PlayAnimation(AnimationState.Situp);
            if(Timer >= Phase2GetupTime)
            {
                HasPhaseTransitioned = true;
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_ImSoReady()
        {
            Timer++;
            if(Timer == 1)
            {
                FXUtil.FocusCamera(NPC.Center, Phase2LaughTime * 1.2f);
                Empowered = true;
                CombatText.NewText(NPC.getRect(), Color.Red, "POWER UP!", true);
                SoundStyle laughSound = AssetRegistry.Sounds.Jiitas.JiitasLaugh;
                laughSound.PitchVariance = 0.2f;
                SoundEngine.PlaySound(laughSound, NPC.position);
            }

            NPC.velocity *= 0f;
            NPC.dontTakeDamage = false;
            PlayAnimation(AnimationState.Laugh);
            if(Timer >= Phase2LaughTime)
            {
                SwitchState(ActionState.Idle);
            }
        }
    }
}
