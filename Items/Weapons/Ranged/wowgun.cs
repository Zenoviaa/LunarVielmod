using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class wowgun : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Summon;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 13;
            Item.mana = 10;
        }
        public override void SetStaticDefaults()
		{
			/* Tooltip.SetDefault("You made a gun out of wood what did you expect.." +
				"\nFor your efforts I commend you, now use my voice as a weapon :)"); */
			// DisplayName.SetDefault("WOWanizer");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 62;
			Item.height = 32;
			Item.scale = 0.75f;
			Item.rare = ItemRarityID.Blue;
			Item.useTime = 40;
			Item.useAnimation = 40;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/wow");
			
			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 25;
			Item.knockBack = 3f;
			Item.noMelee = true;

			// Gun Properties
			Item.shoot = ModContent.ProjectileType<wowBullet>();
			Item.shootSpeed = 2f;
			Item.value = 5000;
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}
		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.Anvils);
			recipe.Register();
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
            recipe.AddIngredient(ModContent.ItemType<RippedFabric>(), 4);
			recipe.AddIngredient(ModContent.ItemType<OvermorrowWood>(), 15);
			recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
			
		}
	}
}