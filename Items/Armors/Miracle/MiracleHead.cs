using Stellamod.Items.Materials;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Miracle
{
    [AutoloadEquip(EquipType.Head)]
    public class MiracleHead : ModItem
    {
        public bool Spetalite = false;
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Astrasilk Hat");
			// Tooltip.SetDefault("Increases Mana Regen by 4%");
		}

        public override void SetDefaults()
        {
            Item.width = 40;
            Item.height = 30;
            Item.value = 10000;
            Item.rare = ItemRarityID.LightPurple;
            Item.defense = 8;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.maxTurrets += 1;
        }

        public override void UpdateArmorSet(Player player)
        {
            player.setBonus = "+1 Max Sentry\nWIP";  // This is the setbonus tooltip
            player.maxTurrets += 1;
        }

        public override bool IsArmorSet(Item head, Item body, Item legs)
        {
            return body.type == ModContent.ItemType<MiracleBody>();
        }

        public override void ArmorSetShadows(Player player)
        {
            player.armorEffectDrawShadow = true;
        }

        public override void AddRecipes() 
        {
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ModContent.ItemType<StarSilk>(), 8);
            recipe.AddIngredient(ModContent.ItemType<AuroreanStarI>(), 2);
            recipe.AddTile(TileID.Anvils);
            recipe.Register();
        }
    }
}
