using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Mage
{
    public class BookOfSkullsII : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 44;
            Item.mana = 6;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jelly Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 144;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 32;
			Item.useAnimation = 32;
			Item.useStyle = ItemUseStyleID.Shoot;
            Item.noUseGraphic = true;
            Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightRed;

			Item.autoReuse = true;
			Item.shoot = ProjectileType<BookOfSkullsIIProg>();
			Item.shootSpeed = 8f;
			Item.mana = 5;


		}
		public override void AddRecipes()
		{
            Recipe recipe = CreateRecipe();
            recipe.AddIngredient(ItemID.Bone, 16);
			recipe.AddIngredient(ItemType<EldritchSoul>(), 6);
            recipe.AddIngredient(ItemID.BookofSkulls, 1);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
		}
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}