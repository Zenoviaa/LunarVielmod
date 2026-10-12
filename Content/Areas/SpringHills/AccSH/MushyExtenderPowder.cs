using Microsoft.Xna.Framework;
using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;

using Stellamod.Items;
using System.Collections.Generic;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Content.Areas.SpringHills.AccSH;

public class MushyExtenderPowder : ModItem
{
    public override string Texture => AssetReferences.Assets.Textures.Item.MushyExtenderPowder.KEY;
    public override void SetDefaults()
    {
        Item.DefaultToAccessory();
    }

    public override void UpdateAccessory(Player player, bool hideVisual)
    {
        IgniterPlayer igniterPlayer = player.GetModPlayer<IgniterPlayer>();
        igniterPlayer.bouncing = true;
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(),
            material: ModContent.ItemType<Mushroom>());
    }
}