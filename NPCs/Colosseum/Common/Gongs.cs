using Stellamod.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.NPCs.Colosseum.Common
{
    public class BronzeGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();


            CreateWaveManager(0);
        }
    }

    public class SilverGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();
            CreateWaveManager(1);
        }
    }

    public class GoldGong : BaseGongNPC
    {
        protected override void StartColosseum()
        {
            base.StartColosseum();
            CreateWaveManager(2);
        }
    }
}
