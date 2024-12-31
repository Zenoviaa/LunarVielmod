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

namespace Stellamod.Items.Armors.LunarianVoid
{
    [AutoloadEquip(EquipType.Legs)]
    public class LunarianVoidLegs : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Astrasilk Boots");
			// Tooltip.SetDefault("Increases movement speed by 20%");
		}
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 22;
            Item.value = 10000;
            Item.rare = ItemRarityID.Green;
            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.moveSpeed += 0.2f;
            player.GetDamage(DamageClass.Throwing) *= 1.05f;
        }

        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<SpacialDistortionFragments>(), 6);
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 20);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
