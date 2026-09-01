using Stellamod.Common.BackpackSystem;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.ChoirGardens.AccCG;

public class SongofWinds : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        BackpackPlayer bp = player.GetModPlayer<BackpackPlayer>();
        bp.hasDamageBonus = true;
    }
}
