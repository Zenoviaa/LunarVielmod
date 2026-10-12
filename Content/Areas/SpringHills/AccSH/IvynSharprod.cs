using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class IvynSharprod : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.IvynSharprod.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetArmorPenetration(DamageClass.Generic) += 3;
    }
    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankAccessory>();
    }
}
