using Stellamod.Common.WeaponTypes;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.AccWD;


public class DoubleBaller : ModItem
{
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }
    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<ManaSpherePlayer>().hasDoubleSpheres = true;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<HypnotizedSoul, BlankAccessory>();
    }
}
