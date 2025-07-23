using Stellamod.Core.HealthbarSystem;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public abstract class ScarletBoss : ModNPC
    {

        public override void SetDefaults()
        {
            base.SetDefaults();
            NPC.boss = true;
        }
        public string Texture_BossIcon => Texture + "_BossIcon";
        public string Texture_BossBar => Texture + "_BossBar";

        public override void AI()
        {
            base.AI();
            if (Main.netMode == NetmodeID.Server)
                return;

            ModContent.GetInstance<BossHealthbarSystem>().Add(this);
        }
    }
}
