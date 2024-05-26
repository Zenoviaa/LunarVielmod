using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Mage
{
    public class LeadGauntlets : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Ranged;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 34;
            Item.mana = 0;
        }

		public override void SetDefaults()
		{
			Item.damage = 23;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;

			Item.autoReuse = true;
			Item.shoot = ProjectileType<LeadFist>();
			Item.shootSpeed = 25f;
			Item.mana = 5;


		}

		public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddRecipeGroup(nameof(ItemID.IronBar), 6);
			recipe.AddIngredient(ItemType<ConvulgingMater>(), 15);
            recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}