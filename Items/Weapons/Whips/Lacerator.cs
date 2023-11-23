using Stellamod.Buffs.Whipfx;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Whips;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Whips
{
    public class Lacerator : ModItem
    {
		public override LocalizedText Tooltip => base.Tooltip.WithFormatArgs(LaceratorDebuff.TagDamage);
		public override void SetDefaults()
		{
			// This method quickly sets the whip's properties.
			// Mouse over to see its parameters.
			Item.DefaultToWhip(ModContent.ProjectileType<LaceratorProj>(), 100, 3, 24);
			Item.width = 40;
			Item.height = 34;
			Item.rare = ItemRarityID.LightPurple;
		}

		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		public override void AddRecipes()
		{
			CreateRecipe()
				.AddIngredient(ItemID.SwordWhip, 1)
				.AddIngredient(ModContent.ItemType<MiracleThread>(), 20)
				.AddIngredient(ModContent.ItemType<WanderingFlame>(), 8)
				.AddIngredient(ModContent.ItemType<DarkEssence>(), 4)
				.AddIngredient(ModContent.ItemType<EldritchSoul>(), 4)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}

		// Makes the whip receive melee prefixes
		public override bool MeleePrefix()
		{
			return true;
		}
	}
}
