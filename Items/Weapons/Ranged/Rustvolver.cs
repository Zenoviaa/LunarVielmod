using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Items.Weapons.Ranged
{
    public class Rustvolver : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Western Shotgun"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 39;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 56;
			Item.height = 56;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 100000;
			Item.rare = ItemRarityID.LightRed;
			Item.UseSound = SoundID.Item36;
			Item.autoReuse = true;
			Item.shoot = ProjectileID.Bullet;
			Item.shopCustomPrice = 23;
			Item.shootSpeed = 15;
			Item.useAmmo = AmmoID.Bullet;
            Item.noMelee = true;
        }
 
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

    
            int numberProjectiles = 2 + Main.rand.Next(2); // 4 or 5 shots
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.Y).RotatedByRandom(MathHelper.ToRadians(20)); // 30 degree spread.
																												// If you want to randomize the speed to stagger the projectiles
				float scale = 1f - (Main.rand.NextFloat() * .4f);
				// perturbedSpeed = perturbedSpeed * scale; 
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}
			return false; // return false because we don't want tmodloader to shoot projectile
		}
	}
}