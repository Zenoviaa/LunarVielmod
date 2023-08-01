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
using Stellamod.Items.Materials.Tech;

namespace Stellamod.Items.Armors.HeavyMetal
{
    [AutoloadEquip(EquipType.Head)]
    public class HeavyMetalHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Heavy Metal Hat");
			// Tooltip.SetDefault("Increases throwing critical strike chance by 4%");
		}

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = 6;

            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {

        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<HeavyMetalBody>() && legs.type == ModContent.ItemType<HeavyMetalLegs>();
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.maxMinions += 1;
            Main.LocalPlayer.GetModPlayer<MyPlayer>().HMArmor = true;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<MetallicOmniSource>(), 1);
            recipe.AddIngredient(ItemType<ArnchaliteBar>(), 5);
            recipe.AddIngredient(ItemID.IronBar, 3);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe.AddIngredient(ItemType<MetallicOmniSource>(), 1);
            recipe.AddIngredient(ItemType<ArnchaliteBar>(), 5);
            recipe2.AddIngredient(ItemID.LeadBar, 3);
            recipe2.AddTile(TileID.Anvils);
            recipe2.Register();
        }
    }
}
