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
    [AutoloadEquip(EquipType.Head)]
    public class DreamerHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Leather Hat");
			// Tooltip.SetDefault("Increases throwing critical strike chance by 4%");
		}

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = 6;

            Item.defense = 21;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Ranged) += 0.25f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<DreamerBody>() && legs.type == ModContent.ItemType<DreamerLegs>();
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        public override void UpdateArmorSet(Player player)
        {
            Main.LocalPlayer.GetModPlayer<MyPlayer>().Leather = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();

            recipe.AddIngredient(ItemType<DreamerBar>(), 12);
            recipe.AddIngredient(ItemID.SoulofSight, 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
