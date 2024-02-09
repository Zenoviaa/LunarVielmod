using Microsoft.Xna.Framework;
using Stellamod.Items.Materials.Tech;
using Stellamod.Items.Ores;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class MiniPistol : ModItem
	{
		private int _comboCounter;
		public override void SetDefaults()
		{
			Item.damage = 17;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 40;
			Item.height = 40;
			Item.useTime = 31;
			Item.useAnimation = 31;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 100000;
			Item.rare = ItemRarityID.Pink;
			Item.UseSound = SoundID.Item36;
			Item.autoReuse = false;
			Item.shoot = ProjectileID.Bullet;
			Item.shootSpeed = 35f;
			Item.useAmmo = AmmoID.Bullet;
		}
		
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
			_comboCounter++;
			if(_comboCounter > 100)
            {
                Item.useTime = 31;
                Item.useAnimation = 31;
                _comboCounter = 0;
			}

            if (Item.useAnimation > 4)
			{
                Item.useTime--;
                Item.useAnimation--;
            }

            for (int p = 0; p < 1; p++)
            {
                // Rotate the velocity randomly by 30 degrees at max.
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Projectile.NewProjectileDirect(source, position, newVelocity, type, damage, knockback, player.whoAmI);
            }

            for (int k = 0; k < 3; k++)
            {
                Vector2 newVelocity = velocity.RotatedByRandom(MathHelper.ToRadians(7));
                newVelocity *= 1f - Main.rand.NextFloat(0.3f);
                Dust.NewDust(position, 0, 0, DustID.Smoke, newVelocity.X * 0.5f, newVelocity.Y * 0.5f);
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }

        public override void AddRecipes()
        {
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ModContent.ItemType<BasicGunParts>(), 1);
			recipe.AddIngredient(ItemID.HallowedBar, 12);
			recipe.AddRecipeGroup(nameof(ItemID.GoldBar), 10);
			recipe.AddIngredient(ModContent.ItemType<FrileBar>(), 8);
            recipe.AddIngredient(ModContent.ItemType<RangerDrive>(), 1);
            recipe.AddTile(TileID.MythrilAnvil);
			recipe.Register();
        }
    }
}
