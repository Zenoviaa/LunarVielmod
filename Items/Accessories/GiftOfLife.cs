using Stellamod.Items.Materials;

using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Accessories
{
    public class GiftOfLife : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Gift Of Life");
            // Tooltip.SetDefault("Increases Mana Regen and Max Mana Count");
        }
        public override void SetDefaults()
        {
            Item.value = Item.sellPrice(gold: 4);
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(silver: 12);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 2;
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.manaRegen += 3;
            player.statManaMax2 += 20;

        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<WoodShield>(), 1);
            recipe.AddIngredient(ItemType<Ivythorn>(), 10);
            recipe.AddIngredient(ItemType<CoreGem>(), 3);
            recipe.AddIngredient(ItemID.NaturesGift, 1);
            recipe.AddTile(TileID.TinkerersWorkbench);
            recipe.Register();
        }
    }
}