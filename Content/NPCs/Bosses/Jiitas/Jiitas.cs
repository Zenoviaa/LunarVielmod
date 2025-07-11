using Stellamod.Core.Helpers;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas : ModNPC
    {       
        /*
         * 
         * Jiitas
            Idle - Hovers around in place a little, like wiggles around like a floating puppet, will jump a few times before doing an attack

            Knife Spin - Spins with knives poking out and just tries to hit you, all you have to do is jump over it or dash through. If you get hit she jumps away, laughing and stops the attack. This also poisons you

            Bombs Away - Goes down and throws out bombs all around, will sometimes get pulled by the strings to mix up the movement, just back away to dodge it

            Spin Jump - Like Deton's and Nexa's jumping attacks, except she throws out knives at you

            Machine Gun Surprise - Pulls up her cloak and let's loose a barrage of bullets, these slowly hitscan (the bullets have no travel time and just raycast) towards you, just weave around it

            Fakeout - Laughs and makes several illusionary clones, if you attack a clone she throws a spread of knives or bombs


            Phase 2
            Her mask breaks, she gets faster, attack pattern changes a bit

            Bomb Drop - Uses strings to drag bombs above you and try to drop them on you
            Empower - She'll sometimes laugh and buff herself, which makes the next attack she does stronger
            -Empowered Knife Spin - Spins faster
            -Empowered Bombs Away - Bombs have a bigger explosion
            -Empowered Spin Jump - Throws more knives
            -Empowered Machine Gun - Does two barrages instead of just one, is a bit faster
            -Empowere Fakeout - Creates more clones
         */
        public override string Texture => AssetHelper.EmptyTexture;
        
        private enum AnimationState
        {
            Idle,
            Sitdown,
            Knifeout,
            Knifespin,
            Knifein,
            Situp,
            BombsAway,
            Bombing,
            Laugh,
            Undress,
            Redress,
            Jumpspin1,
            Jumpspin2,
            Jumpspin3,
            Dragup,
            Death
        }
        private enum ActionState
        {
            Idle,
            Spawn,
            KnifeSpin,
            BombsAway,
            SpinJump,
            MachineGunSurprise,
            Fakeout,
            BombDrop,
            Empower,
            Death
        }
        private ref float Timer => ref NPC.ai[0];
        private ActionState State
        {
            get => (ActionState)NPC.ai[1];
            set => NPC.ai[1] = (float)value;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.boss = true;
            NPC.lifeMax = 2400;
            NPC.width = 32;
            NPC.height = 32;
            NPC.damage = 1;
            NPC.defense = 0;
            NPC.value = 10000;
        }

        public override void FindFrame(int frameHeight)
        {
            base.FindFrame(frameHeight);
        }

        private void SwitchState(ActionState state)
        {
            if (MultiplayerHelper.IsHost)
            {
                Timer = 0;
                State = state;
                NPC.netUpdate = true;
            }
        }

        public override void AI()
        {
            base.AI();
            
            NPC.TargetClosest();
            switch (State)
            {
                case ActionState.Idle:
                    AI_Idle();
                    break;
                case ActionState.Spawn:
                    AI_Spawn();
                    break;
                case ActionState.KnifeSpin:
                    AI_KnifeSpin();
                    break;
                case ActionState.BombsAway:
                    AI_BombsAway();
                    break;
                case ActionState.SpinJump:
                    AI_SpinJump();
                    break;
                case ActionState.MachineGunSurprise:
                    AI_MachineGunSurprise();
                    break;
                case ActionState.Fakeout:
                    AI_Fakeout();
                    break;
                case ActionState.BombDrop:
                    AI_BombDrop();
                    break;
                case ActionState.Empower:
                    AI_Empower();
                    break;
                case ActionState.Death:
                    AI_Death();
                    break;
            }
        }
    }
}

