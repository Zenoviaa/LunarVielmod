using Stellamod.Common.StaminaSystem;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.AccSH;

[AutoloadEquip(EquipType.Neck)]
public class IvynNecklace : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.IvynNecklace.KEY;
    public override void SetDefaults()
    {
        base.SetDefaults();
        Item.width = 30;
        Item.height = 40;
        Item.rare = ItemRarityID.Green;
        Item.accessory = true;
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        base.UpdateAccessory(player, hideVisual);
        StaminaNDashPlayer dashPlayer = player.GetModPlayer<StaminaNDashPlayer>();
        dashPlayer.MaxDashCount += 1;
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<Ivythorn, BlankAccessory>();
    }
}