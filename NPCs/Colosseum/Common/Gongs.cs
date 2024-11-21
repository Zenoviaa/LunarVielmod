using Terraria;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Common
{
    public class BronzeGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            colosseumSystem.StartColosseum(0, NPC.Bottom.ToTileCoordinates());
        }
    }

    public class SilverGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            colosseumSystem.StartColosseum(1, NPC.Bottom.ToTileCoordinates());
        }
    }

    public class GoldGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();
            ColosseumSystem colosseumSystem = ModContent.GetInstance<ColosseumSystem>();
            colosseumSystem.StartColosseum(2, NPC.Bottom.ToTileCoordinates());
        }
    }
}
