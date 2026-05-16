using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;

using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Terror.AccTR;

public class GremoryExtenderPowder : ModItem
{
    public override void SetStaticDefaults()
    {
        CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
    }

    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        player.GetModPlayer<IgniterPlayer>().extenderBonus += 0.25f;
        player.GetModPlayer<IgniterPlayer>().hasLifesteal = true;
    }


    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(
            mold: ModContent.ItemType<BlankAccessory>(), 
            material: ModContent.ItemType<TerrorFragments>());
    }
}