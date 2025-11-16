using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Buffs
{
    public class BurnedWings : ModBuff
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Charm Buff!");
            // Description.SetDefault("Icy Frileness!");
            Main.pvpBuff[Type] = true;
            Main.buffNoTimeDisplay[Type] = true;
        }


        public override void Update(Player player, ref int buffIndex)
        {
            base.Update(player, ref buffIndex);
            player.wingTimeMax = 0;
            player.wingTime = 0;
        }
    }
}
