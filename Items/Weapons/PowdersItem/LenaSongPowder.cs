using Stellamod.Projectiles.Powders;
using Terraria;
using Terraria.Audio;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.DataStructures;
using Microsoft.Xna.Framework;

namespace Stellamod.Items.Weapons.PowdersItem
{
    internal class LenaSongPowder : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Whimsical Drear Powder");
			/* Tooltip.SetDefault("Throw magical dust on them!" +
				"\nA musical dust! that explodes with your igniter!"); */
		}

		
		public override void SetDefaults()
		{
			
			Item.damage = 4;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.rare = ItemRarityID.Green;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<LenaPowderProj>();
			Item.autoReuse = true;
			Item.shootSpeed = 20f;
			Item.crit = 51;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/Lenabee");
			Item.sellPrice(1, 50, 0, 0);
		
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			int dir = player.direction;

			Projectile.NewProjectile(source, position, velocity *= player.GetModPlayer<MyPlayer>().IgniterVelocity, type, damage, knockback, player.whoAmI);
			return false;
		}


	}
}