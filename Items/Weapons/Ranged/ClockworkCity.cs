using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Stellamod.Projectiles.Gun;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
	public class ClockworkCity : ClassSwapItem
    {
        public int dir;
        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 20;
            Item.mana = 20;
        }
        public override void SetDefaults() 
		{
			Item.damage = 25;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 84;
			Item.height = 36;
			Item.useTime = 37;
			Item.useAnimation = 37;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = Item.sellPrice(0, 0, 20, 0);
			Item.rare = ItemRarityID.LightRed;
            Item.UseSound = new SoundStyle("Stellamod/Assets/Sounds/ClockworkCity3");
            Item.autoReuse = true;
			Item.shootSpeed = 50f;
			Item.shoot = ModContent.ProjectileType<ClockworkBomb>();
            Item.noMelee = true;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			int numProjectiles = Main.rand.Next(4, 7);
			for (int p = 0; p < numProjectiles; p++)
			{
				// Rotate the velocity randomly by 30 degrees at max.
				Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(15));
				newVelocity *= 1f - Main.rand.NextFloat(0.3f);
				Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
			}

            return true; // return false because we don't want tmodloader to shoot projectile
		}

		public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ItemID.Shotgun, 1);
			recipe.AddIngredient(ItemID.Cog, 5);
            recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
			recipe.AddIngredient(ModContent.ItemType<GrailBar>(), 10);
			recipe.AddIngredient(ModContent.ItemType<AlcadizScrap>(), 35);
			recipe.AddIngredient(ModContent.ItemType<WeaponDrive>(), 1);
			recipe.Register();
		}
	}
}