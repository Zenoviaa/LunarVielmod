using Stellamod.Core.HealthbarSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Core
{
    public  abstract class ScarletBoss : ModNPC
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
            ModContent.GetInstance<BossHealthbarSystem>().Add(this);
        }
    }
}
