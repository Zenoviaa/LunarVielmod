using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Common
{
    public class BronzeGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();
            NPC.NewNPC(NPC.GetSource_FromThis(), 
                (int)NPC.Bottom.X, 
                (int)NPC.Bottom.Y, ModContent.NPCType<ColosseumWaveManager>(), ai1: 0);
        }
    }

    public class SilverGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();
            NPC.NewNPC(NPC.GetSource_FromThis(),
                (int)NPC.Bottom.X,
                (int)NPC.Bottom.Y, ModContent.NPCType<ColosseumWaveManager>(), ai1: 1);
        }
    }

    public class GoldGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();
            NPC.NewNPC(NPC.GetSource_FromThis(),
                (int)NPC.Bottom.X,
                (int)NPC.Bottom.Y, ModContent.NPCType<ColosseumWaveManager>(), ai1: 2);
        }
    }
}
