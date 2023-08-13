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

namespace Stellamod.Items.Armors.AcidArmour
{
    [AutoloadEquip(EquipType.Head)]
    public class AcidRobe : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acid Robe");
			// Tooltip.SetDefault("Increases magic by 5% and magic critical strike chance by 6%");
		}

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = 6;

            Item.defense = 3;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.05F;
            player.GetCritChance(DamageClass.Magic) += 6;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == Mod.Find<ModItem>("AcidBody").Type && legs.type == Mod.Find<ModItem>("AcidLegs").Type;
        }
        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }
        public override void UpdateArmorSet(Player player)
        {
            player.manaCost -= 0.5F;
            player.moveSpeed = 2f;
        }
        public override void AddRecipes()
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<VirulentPlating>(), 5);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
