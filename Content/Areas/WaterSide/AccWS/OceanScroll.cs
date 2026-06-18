using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorRework;
using Stellamod.Content.CommonMaterials;
using Stellamod.Items;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WaterSide.AccWS
{
    public class OceanScroll : ModItem
    {
        public override void SetDefaults()
        {
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetStats().artifactManaReduction += 0.2f;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<ConvulgingMater, BlankAccessory>();
        }
    }
}

