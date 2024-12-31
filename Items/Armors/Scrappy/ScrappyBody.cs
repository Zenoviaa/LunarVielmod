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
    [AutoloadEquip(EquipType.Body)]
    internal class ScrappyBody : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34; // Width of the item
            Item.height = 20; // Height of the item
            Item.value = Item.sellPrice(gold: 6); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 18; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 3;
            player.endurance += 0.08f;
            player.maxMinions += 2;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.GetDamage(DamageClass.Magic) += 0.12f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ModContent.ItemType<ArmorDrive>(), 10);
            recipe.AddIngredient(ModContent.ItemType<BrokenTech>(), 10);
            recipe.AddIngredient(ModContent.ItemType<UnknownCircuitry>(), 10);
            recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 5);
            recipe.AddIngredient(ItemID.Ectoplasm, 5);
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 10);

            recipe.Register();
        }
    }
}
