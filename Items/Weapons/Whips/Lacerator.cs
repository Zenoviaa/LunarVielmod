using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Buffs.Whipfx;
using Stellamod.Helpers;
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
        public override void SetStaticDefaults()
        {
			ItemID.Sets.ItemNoGravity[Item.type] = true;
		}

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
				.AddIngredient(ModContent.ItemType<MiracleThread>(), 12)
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


		public override bool PreDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, ref float rotation, ref float scale, int whoAmI)
		{
			DrawHelper.DrawGlow2InWorld(Item, spriteBatch, ref rotation, ref scale, whoAmI);
			return true;
		}

		public override void Update(ref float gravity, ref float maxFallSpeed)
		{
			//The below code makes this item hover up and down in the world
			//Don't forget to make the item have no gravity, otherwise there will be weird side effects
			float hoverSpeed = 5;
			float hoverRange = 0.2f;
			float y = VectorHelper.Osc(-hoverRange, hoverRange, hoverSpeed);
			Vector2 position = new Vector2(Item.position.X, Item.position.Y + y);
			Item.position = position;
		}
	}
}
