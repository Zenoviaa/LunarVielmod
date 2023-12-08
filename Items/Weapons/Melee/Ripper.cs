using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Swords;
using Stellamod.Projectiles.Swords.Ripper;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Melee
{

	public class Ripper : ModItem
    {
		private const int Mana_Cost = 50;


		public override void SetDefaults()
        {
			Item.damage = 78;
			Item.crit = 4;
			Item.knockBack = 3f;
			Item.width = 62;
			Item.height = 54;
			Item.useTime = 5;
			Item.useAnimation = 5;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.UseSound = SoundID.Item1;
			Item.value = Item.buyPrice(0, 30, 0, 0);
			Item.rare = ItemRarityID.LightPurple;
			Item.DamageType = DamageClass.Melee;
			Item.shoot = ModContent.ProjectileType<RipperSwordProj>();
			Item.shootSpeed = 16;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.noMelee = true;
		}

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float swordSpeed = 16;
			float degrees = Main.rand.Next(0, 360);
			float circleDistance = 256;
			Vector2 circlePosition = Main.MouseWorld + new Vector2(circleDistance, 0)
					.RotatedBy(MathHelper.ToRadians(degrees));
			Vector2 vel = (circlePosition.DirectionTo(Main.MouseWorld) * swordSpeed) / 100;
			Projectile.NewProjectile(Item.GetSource_FromThis(), circlePosition, vel,
				ModContent.ProjectileType<RipperSwordProj>(), Item.damage, Item.knockBack, player.whoAmI);
			return false;
        }

        public override bool AltFunctionUse(Player player)
		{
			return true;
		}

		public override bool CanUseItem(Player player)
		{
			if (player.altFunctionUse == 2 && player.statMana >= Mana_Cost)
			{
				int sound = Main.rand.Next(0, 3);
                switch (sound)
                {
					case 0:
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia1"), player.position);
						break;
					case 1:
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia2"), player.position);
						break;
					case 2:
						SoundEngine.PlaySound(new SoundStyle("Stellamod/Assets/Sounds/SwordOfGlactia3"), player.position);
						break;
                }
			
				player.statMana -= Mana_Cost;
				float swordCount = 8;
				for(int i = 0; i < swordCount; i++)
                {
					//360 degrees in circle :P
					float degreesBetween = 360 / swordCount;
					float degrees = degreesBetween * i;
					float circleDistance = 256;
					float swordSpeed = 16;
					Vector2 circlePosition = Main.MouseWorld + new Vector2(circleDistance, 0)
						.RotatedBy(MathHelper.ToRadians(degrees));

					//I divide by 100 here cause I want there to be a delay before the swords converge
					Vector2 velocity = (circlePosition.DirectionTo(Main.MouseWorld) * swordSpeed) / 100;
					Projectile.NewProjectile(player.GetSource_FromThis(), circlePosition, velocity,
						ModContent.ProjectileType<RipperSwordProj>(), Item.damage*10, Item.knockBack, player.whoAmI);
				}
			}

			return base.CanUseItem(player);
		}

        public override void AddRecipes()
        {
			CreateRecipe()
				.AddIngredient(ItemID.BrokenHeroSword, 1)
				.AddIngredient(ModContent.ItemType<MiracleThread>(), 8)
				.AddIngredient(ModContent.ItemType<WanderingFlame>(), 4)
				.AddIngredient(ModContent.ItemType<DarkEssence>(), 2)
				.AddIngredient(ModContent.ItemType<EldritchSoul>(), 2)
				.AddTile(TileID.MythrilAnvil)
				.Register();
		}
    }
}
