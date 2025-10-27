using Microsoft.Xna.Framework;
using Stellamod.Core.HealthbarSystem;
using System;
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

        public override bool? DrawHealthBar(byte hbPosition, ref float scale, ref Vector2 position)
        {
            return false;
        }

        public override void AI()
        {
            base.AI();
            if (Main.netMode == NetmodeID.Server)
                return;

            ModContent.GetInstance<BossHealthbarSystem>().Add(this);
        }

        public virtual bool CanFight()
        {
            return true;
        }
    }
}
