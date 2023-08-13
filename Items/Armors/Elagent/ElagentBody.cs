using Terraria;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;
using Terraria.ID;
using static Terraria.ModLoader.ModContent;
using Stellamod.Items.Materials;
using Terraria.GameContent.Creative;

namespace Stellamod.Items.Armors.Elagent
{
    [AutoloadEquip(EquipType.Body)]
    public class ElagentBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            // DisplayName.SetDefault("Astolfo Breasts");
            /* Tooltip.SetDefault("Nya~"
				+ "\nYummy!" +
				"\n+20 Health"); */

            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 18; // Width of the item
            Item.height = 18; // Height of the item
            Item.value = Item.sellPrice(gold: 1); // How many coins the item is worth
            Item.rare = ItemRarityID.Green; // The rarity of the item
            Item.defense = 9; // The amount of defense the item will give when equipped
        }
        public override void UpdateEquip(Player player)
        {
            player.maxMinions += 1;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<StarSilk>(), 9);
            recipe.AddIngredient(ItemType<PearlescentScrap>(), 9);
            recipe.AddIngredient(ItemID.Feather, 12);
            recipe.AddIngredient(ItemID.Bone, 16);
            recipe.AddTile(TileID.SkyMill);
            recipe.Register();

        }
    }
}
