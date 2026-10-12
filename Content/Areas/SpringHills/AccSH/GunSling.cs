using Stellamod.Common.GunSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class GunSling : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.GunSling.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        player.GetModPlayer<GunHoldPlayer>().gunSling = true;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankAccessory>();
    }
}
