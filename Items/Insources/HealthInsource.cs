using Stellamod.Core.XixianFlaskSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Insources
{
    public class HealthInsource : InsourceItem
    {
        public override int GetAddedTime()
        {
            return 60 * 30;
        }

        public void UseInsource(Player player)
        {
            player.Heal(50);
        }
    }
}
