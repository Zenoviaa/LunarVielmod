using Stellamod.Items.Materials;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Items.Materials.Tech;

namespace Stellamod.Items.Armors.Scrappy
{
    [AutoloadEquip(EquipType.Legs)]
    internal class ScrappyLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 12; // Height of the item
            Item.value = Item.sellPrice(gold: 5); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 10; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.maxRunSpeed += 0.04f;
            player.runAcceleration += 0.12f;
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Summon) += 0.08f;
            player.GetDamage(DamageClass.Magic) += 0.08f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<ArmorDrive>(), 3);
            recipe.AddIngredient(ModContent.ItemType<BrokenTech>(), 25);
            recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
            recipe.AddIngredient(ItemID.Ectoplasm, 3);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 5);

            recipe.Register();
        }
    }
}
