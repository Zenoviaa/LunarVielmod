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

namespace Stellamod.Items.Armors.ShadeWraith
{
    [AutoloadEquip(EquipType.Head)]
    public class ShadeWraithHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Shade Wraith Head");
			// Tooltip.SetDefault("Increases all damage by 10%");
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
            player.GetDamage(DamageClass.Summon) += 0.02f;
            player.GetDamage(DamageClass.Magic) += 0.02f;
            player.GetDamage(DamageClass.Ranged) += 0.02f;
            player.GetDamage(DamageClass.Melee) += 0.02f;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Mod.Find<ModItem>("ShadeWraithBody").Type && legs.type == Mod.Find<ModItem>("ShadeWraithLegs").Type;
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;

        }
        public override void UpdateArmorSet(Player player)
        {
            if (player.statLife <= player.statLifeMax2 / 2)
            {
                player.statLifeMax2 += 20;
                player.endurance += 0.04f;
                player.moveSpeed += 0.04f;
            }
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<DarkEssence>(), 8);
            recipe.AddIngredient(ItemID.DemoniteBar, 4);
            recipe.AddTile(TileID.WorkBenches);
            recipe.Register();

            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemType<DarkEssence>(), 8);
            recipe2.AddIngredient(ItemID.CrimtaneBar, 4);
            recipe2.AddTile(TileID.WorkBenches);
            recipe2.Register();
        }
    }
}
