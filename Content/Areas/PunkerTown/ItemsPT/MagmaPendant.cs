using Stellamod.Content.Areas.PunkerTown.BossesPT.Gothivia;
using Stellamod.Items.Accessories.Players;
using Terraria;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.PunkerTown.ItemsPT;

public class MagmaPendant : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<GothiviaPlayer>().maxStacks++;
        player.GetModPlayer<DashPlayer>().doubleStaminaCost = true;
    }
}
