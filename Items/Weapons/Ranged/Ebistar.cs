using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class Ebistar : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Magic;
        public override void SetClassSwappedDefaults()
        {
			Item.damage = 62;
			Item.mana = 4;
        }

        public override void SetDefaults()
		{
			Item.CloneDefaults(ItemID.Arkhalis);
			Item.damage = 16; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40; // hitbox width of the Item
			Item.height = 20; // hitbox height of the Item
			Item.useTime = 90; // The Item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true; //so the Item's animation doesn't do damage
			Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.value = 10000; // how much the Item sells for (measured in copper)
			Item.rare = ItemRarityID.Blue; // the color that the Item's name will be in-game
			Item.autoReuse = true; // if you can hold click to automatically use it again
			Item.shoot = ModContent.ProjectileType<EbistarProj>();
			Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
			Item.channel = true;

		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);

            recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 9);
			recipe.AddIngredient(ItemID.FallenStar, 3);
			recipe.AddIngredient(ItemID.IceBlock, 10);

			recipe.Register();
		}
	}

}