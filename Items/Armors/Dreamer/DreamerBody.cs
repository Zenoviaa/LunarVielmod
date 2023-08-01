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
using Stellamod.Items.Materials.HardMode;

namespace Stellamod.Items.Armors.Dreamer
{
    [AutoloadEquip(EquipType.Body)]
    public class DreamerBody : ModItem
    {

        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Leather Vest");
            // Tooltip.SetDefault("Increases throwing damage by 25%");
        }
        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 15;
            Item.vanity = true;
        }
        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Ranged) += 0.45f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<DreamerBar>(), 18);
            recipe.AddIngredient(ItemID.SoulofSight, 8);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }

    }
}
