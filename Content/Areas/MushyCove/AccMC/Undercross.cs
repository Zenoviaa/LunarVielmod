using Stellamod.Common.ArmorRework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MushyCove.AccMC;

public class Undercross : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        if ((player.ZoneRockLayerHeight || player.ZoneUnderworldHeight || player.ZoneDirtLayerHeight) && player.ZonePurity)
        {
            player.GetStats().bossEndurance += 0.25f;
            player.GetStats().enemyEndurance += 0.25f;
        }
    }
}
