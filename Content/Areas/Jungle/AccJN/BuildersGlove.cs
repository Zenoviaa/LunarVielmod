using Stellamod.Content.CommonMaterials;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Jungle.AccJN;

public class BuildersGlove : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<MeleeEffectsPlayer>().noProjectionDamageBonus += 0.5f;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<RadiantNectar, BlankAccessory>();
    }
}
