using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Stellamod.Projectiles.IgniterExplosions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Mage
{
    internal class Bongos : ClassSwapItem
    {
        //Alternate class you want it to change to
        public override DamageClass AlternateClass => DamageClass.Ranged;

	//Defaults for the other class
	public override void SetClassSwappedDefaults()
	{
		//Do if(IsSwapped) if you want to check for the alternate class
		//Stats to have when in the other class
		Item.damage = 30;
		Item.knockBack = 12;
	}
	public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star-Gilded Bongo");
			// Tooltip.SetDefault("Bong bong boom :)");
		}
		public override void SetDefaults()
		{
			Item.damage = 13;
			Item.mana = 3;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 20;
			Item.useAnimation = 20;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/bongo");
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<BongoBoom>();
			Item.autoReuse = true;
			Item.crit = 22;
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.FallenStar, 3);
			recipe.AddTile(TileID.WorkBenches);
			recipe.Register();
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 120);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
			return false;
			
		}
	}
}
