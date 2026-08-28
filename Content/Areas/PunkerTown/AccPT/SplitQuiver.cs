using Stellamod.Content.CommonMaterials;
using Stellamod.Core.Bases;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.AccPT;

public class SplitQuiver : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<CrossbowPlayer>().splittingShot = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MarshScrap, BlankAccessory>();
    }
}
