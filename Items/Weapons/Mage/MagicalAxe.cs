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
	internal class MagicalAxe : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Star-Gilded Bongo");
			// Tooltip.SetDefault("Bong bong boom :)");
		}
		public override void SetDefaults()
		{
			Item.damage = 70;
			Item.mana = 30;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 44;
			Item.useAnimation = 44;
			Item.useStyle = ItemUseStyleID.HoldUp;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Magic;
			Item.value = 15000;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/StarSheith");
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<BlossomBoom>();
			Item.autoReuse = true;
			Item.crit = 2;
			Item.noUseGraphic = true;
		}
		
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			Projectile.NewProjectile(source, Main.MouseWorld, Vector2.Zero, type, damage, knockback, player.whoAmI, 0f, 0f);
			return false;

		}
	}
}
