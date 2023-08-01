using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


using Stellamod;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;

namespace Stellamod.Items.Accessories
{
    internal class StarflareBand : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Starflare Band");
            // Tooltip.SetDefault("Decreases Mana Cost and magic damage");
        }

        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.value = 2500;
            Item.rare = 1;
            Item.accessory = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.FallenStar, 5);
            recipe.AddIngredient(ModContent.ItemType<DarkEssence>(), 15);
            recipe.AddIngredient(ModContent.ItemType<WanderingFlame>(), 25);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();
        }
        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetDamage(DamageClass.Magic) += 0.4f;
            player.manaCost -= 0.4f;
            player.manaRegen += 2;

        }
    }
}