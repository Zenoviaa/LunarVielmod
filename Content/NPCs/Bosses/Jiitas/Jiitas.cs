using Stellamod.Core.Helpers;
using Terraria.ModLoader;

namespace Stellamod.Content.NPCs.Bosses.Jiitas
{
    internal partial class Jiitas : ModNPC
    {
        public override string Texture => AssetHelper.EmptyTexture;
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
            NPC.lifeMax = 2400;
            NPC.boss = true;
            NPC.damage = 1;
            NPC.defense = 0;
            NPC.value = 10000;
        }

        public override void AI()
        {
            base.AI();
            /*
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
                    break;*/
        }
    }
}

