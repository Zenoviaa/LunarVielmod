using Stellamod.Assets;
using System;
using Terraria;
using Terraria.Audio;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas
    {
        public float SitdownStartupTime
        {
            get
            {
                return 60;
            }
        }
        public float SitupTime
        {
            get
            {
                return 30;
            }
        }
        public float KnifeSpinStartupTime
        {
            get
            {
                return 60;
            }
        }
        public float KnifeSpinSpeed
        {
            get
            {
                if (Empowered)
                {
                    return 16;
                }

                return 12;
            }
        }

        public float KnifeSpinTime
        {
            get
            {
                if (Empowered)
                {
                    return 280;
                }
                return 180;
            }
        }
        public float KnifeSpinEndTime
        {
            get
            {
                return 60;
            }
        }

        private void AI_KnifeSpin()
        {
            NPC.noGravity = false;
            switch (ActionStep)
            {
                case 0:
                    AI_Sitdown();
                    break;
                case 1:
                    AI_KnifeSpinStart();
                    break;
                case 2:
                    AI_KnifeSpinLoop();
                    break;
                case 3:
                    AI_KnifeSpinEnd();
                    break;
                case 4:
                    AI_Situp();
                    break;
            }
        }

        private void AI_Sitdown()
        {
            PlayAnimation(AnimationState.Sitdown);
            Timer++;
            if(Timer == 1)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasSit;
                jiitasSit.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jiitasSit, NPC.position);
            }

            NPC.noGravity = false;
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            if (Timer >= SitdownStartupTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }
        private void AI_KnifeSpinStart()
        {
            Warn();
            PlayAnimation(AnimationState.Knifeout);
            Timer++;
            if (Timer == 1)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasKnifeSlash;
                jiitasSit.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jiitasSit, NPC.position);
            }
            NPC.noGravity = false;
            NPC.velocity.X *= 0.9f;
            NPC.rotation *= 0.9f;
            if (Timer >= KnifeSpinStartupTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_KnifeSpinLoop()
        {
            WarnContactDamage();
            Timer++;
            if (NPC.collideX)
            {
                Collision.StepUp(ref NPC.position, ref NPC.velocity, NPC.width, NPC.height, ref NPC.stepSpeed, ref NPC.gfxOffY);
            }

            ShouldDealContactDamage = true;

            PlayAnimation(AnimationState.Knifespin);

            if (Timer % 30 == 0)
            {
                SoundStyle jiitasSit = AssetRegistry.Sounds.Jiitas.JiitasLightSpin;
                jiitasSit.PitchVariance = 0.2f;
                SoundEngine.PlaySound(jiitasSit, NPC.position);
            }
            //Check if going wrong direction
            bool isPlayerOnRightAndWereGoingLeft = NPC.velocity.X < 0 && NPC.Center.X > Target.Center.X;
            bool isPlayerOnLeftAndWereGoingRight = NPC.velocity.X > 0 && NPC.Center.X < Target.Center.X;
            if (isPlayerOnRightAndWereGoingLeft || isPlayerOnLeftAndWereGoingRight)
            {
                NPC.velocity += DirectionToTarget * 0.5f;
            }
            else
            {
                if (NPC.velocity.Length() >= KnifeSpinSpeed)
                {
                    NPC.velocity.X *= 0.9f;
                }
                else
                {
                    NPC.velocity.X *= 1.01f;
                }
            }


            //Check distance to target, if far then turn
            if (MathF.Abs(NPC.velocity.X) < 0.2f)
            {
                NPC.velocity += DirectionToTarget;
            }



            if (Timer >= KnifeSpinTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_KnifeSpinEnd()
        {
            NoWarn();
            PlayAnimation(AnimationState.Knifein);
            ShouldDealContactDamage = false;

            Timer++;
            NPC.velocity *= 0.9f;
            NPC.noGravity = true;
            if (Timer >= KnifeSpinEndTime)
            {
                Timer = 0;
                ActionStep++;
            }
        }

        private void AI_Situp()
        {
            PlayAnimation(AnimationState.Situp);
            ShouldDealContactDamage = false;

            Timer++;
            NPC.velocity.X *= 0.9f;
            NPC.velocity.Y -= 0.02f;
            NPC.noGravity = true;
            Empowered = false;

            if (Timer >= SitupTime)
            {
                SwitchState(ActionState.Idle);
            }
        }
    }
}
