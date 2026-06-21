using Stellamod.Content.CommonMaterials;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Hallowrooms.AccHR;

public class WOWanizer : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<MeleeEffectsPlayer>().steinWordBonus++;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<KaleidoscopicInk, BlankAccessory>();
    }
}
