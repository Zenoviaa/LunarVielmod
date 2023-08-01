using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Tech;
using Terraria.Audio;
using Stellamod.Projectiles.Weapons.Gun;

namespace Stellamod.Items.Weapons.Ranged
{
	public class DeathShot : ModItem
	{
		public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("DeathShot"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}
        public override Vector2? HoldoutOffset()
        {
            return new Vector2(-4, 0);
        }
        public override void SetDefaults()
		{
			Item.damage = 31;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 56;
			Item.height = 56;
			Item.useTime = 50;
			Item.useAnimation = 50;
			Item.useStyle = 5;
			Item.knockBack = 6;
			Item.value = 100000;
			Item.rare = 2;
			Item.UseSound = SoundID.Item11;
			Item.autoReuse = true;
            Item.shoot = ModContent.ProjectileType<DeathShotProj>();
            Item.shopCustomPrice = 23;
			Item.shootSpeed = 15;
			Item.useAmmo = AmmoID.Bullet;
        }
        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            if (type == ProjectileID.Bullet) type = ModContent.ProjectileType<DeathShotProj>();
            int Sound = Main.rand.Next(1, 3);
            if (Sound == 1)
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Item/DeathShot"), player.position);
            }
            else
            {
                SoundEngine.PlaySound(new SoundStyle("Stellamod/Sounds/Custom/Item/DeathShot2"), player.position);
            }
        }
        public override void AddRecipes()
		{
			Recipe recipe = CreateRecipe();
			recipe.AddIngredient(ItemID.Bone, 25);
			recipe.AddIngredient(ModContent.ItemType<LostScrap>(), 10);
			recipe.AddIngredient(ModContent.ItemType<RangerDrive>(), 1);
			recipe.AddIngredient(ItemID.PlatinumBar, 14);
			recipe.AddTile(TileID.HeavyWorkBench);
			recipe.Register();


            Recipe recipe2 = CreateRecipe();
            recipe2.AddIngredient(ItemID.Bone, 25);
            recipe2.AddIngredient(ModContent.ItemType<LostScrap>(), 10);
            recipe2.AddIngredient(ModContent.ItemType<RangerDrive>(), 1);
            recipe2.AddIngredient(ItemID.GoldBar, 14);
            recipe2.AddTile(TileID.HeavyWorkBench);
            recipe2.Register();
        }
	}
}