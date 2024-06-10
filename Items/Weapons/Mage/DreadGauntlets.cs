using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Mage
{
    public class DreadGauntlets : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Melee;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 20;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jelly Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 32;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Green;

			Item.autoReuse = true;
			Item.shoot = ProjectileType<DreadFist>();
			Item.shootSpeed = 35f;
			Item.mana = 9;


		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemType<TerrorFragments>(), 6);
            recipe.AddIngredient(ItemType<DreadFoil>(), 15);
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}