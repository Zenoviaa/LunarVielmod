using Stellamod.Common.ArmorRework;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.PunkerTown.AccPT;

public class Wandanizer : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetStats().wandNormalEnchantmentSlots += 2;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<MechanizedSoul, BlankAccessory>();
    }
}
