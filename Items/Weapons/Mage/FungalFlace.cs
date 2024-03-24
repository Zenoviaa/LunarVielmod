using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Magic;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Mage
{
	public class FungalFlace : ClassSwapItem
	{
		//Alternate class you want it to change to
		public override DamageClass AlternateClass => DamageClass.Ranged;

		//Defaults for the other class
		public override void SetClassSwappedDefaults()
		{
			//Do if(IsSwapped) if you want to check for the alternate class
			//Stats to have when in the other class
			Item.damage = 20;
			Item.knockBack = 3;
			Item.mana = 0;
			Item.useTime = 15;
			Item.useAnimation = 15;
		}
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Jelly Tome"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.DamageType = DamageClass.Magic;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 30;
			Item.useAnimation = 30;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/SwordThrow");
			Item.autoReuse = true;
			Item.shoot = ProjectileType<FungalFlaceBall>();
			Item.shootSpeed = 14f;
			Item.mana = 8;


		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float numberProjectiles = 5;
			float rotation = MathHelper.ToRadians(14);
			position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * 1f; // This defines the projectile roatation and speed. .4f == projectile speed
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}
			return false;
		}
		
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
	}
}