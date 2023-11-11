using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Armors.AcidArmour
{
    [AutoloadEquip(EquipType.Head)]
    public class AcidHelm : ModItem
    {
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Acid Helm");
            // Tooltip.SetDefault("Increases melee Damage by 5% and melee critical strike chance by 6%");
        }

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.Red;
            Item.defense = 5;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Melee) += 0.05f;
            player.GetCritChance(DamageClass.Generic) += 1;
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
            player.GetAttackSpeed(DamageClass.Melee) += 0.03f;
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
