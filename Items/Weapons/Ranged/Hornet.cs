using Microsoft.Xna.Framework;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
	public class Hornet : ModItem
	{
		public override void SetStaticDefaults()
		{
			Tooltip.SetDefault("Classy!" +
				"\nTotally has no reference to Hollow Knight" +
				"\nA weapon that shoots classy bomb and bullets.. A LOT");
			DisplayName.SetDefault("Hornet");

			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.width = 62;
			Item.height = 32;
			Item.scale = 0.9f;
			Item.rare = ItemRarityID.Green;
			Item.useTime = 12;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.autoReuse = true;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/gun1");

			// Weapon Properties
			Item.DamageType = DamageClass.Ranged;
			Item.damage = 27;
			Item.knockBack = 4;
			Item.noMelee = true;

			// Gun Properties
			Item.shoot = ProjectileID.PurificationPowder;
			Item.shootSpeed = 8f;
			Item.useAmmo = AmmoID.Bullet; // Restrict the type of ammo the weapon can use, so that the weapon cannot use other ammos
		}
		// Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.
		// This method lets you adjust position of the gun in the player's hands. Play with these values until it looks good with your graphics.
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(2f, -2f);
		}
		public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
		{

			type = Main.rand.Next(new int[] { type, ModContent.ProjectileType<HornetLob>() });
		}

	
	}
}