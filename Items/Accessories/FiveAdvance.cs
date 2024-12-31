using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Special.MinerLogs;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories

{
    public class FiveAdvance : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Toxic Striders");
            // Tooltip.SetDefault("Increases Move Speed by 9%");
        }

        public override void SetDefaults()
        {
            Item.Size = new Vector2(20);
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 5);
            Item.rare = ItemRarityID.Green;
            Item.defense = 5;
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.moveSpeed += 0.05f;
            player.GetDamage(DamageClass.Generic) *= 1.05f; // Increase ALL player damage by 100%
            player.GetArmorPenetration(DamageClass.Generic) *= 1.05f;
            player.maxRunSpeed += 0.05f;
            player.statLifeMax2 += 5;
            player.lifeRegen += 5;
            player.endurance += 0.05f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner1>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner2>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner3>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner4>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner5>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner6>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner7>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner8>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner9>(), 1);
            recipe.AddIngredient(ModContent.ItemType<VeiledScriptureMiner10>(), 1);
            recipe.Register();
        }
    }
}



