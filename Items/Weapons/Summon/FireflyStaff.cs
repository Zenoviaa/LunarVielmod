using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Summons.Minions;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;

namespace Stellamod.Items.Weapons.Summon
{
    public class FireflyStaff : ModItem
    {
		private FireflyMinionProj.AttackState _attackState;
		public override void SetDefaults()
		{
			Item.damage = 30;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 48;
			Item.height = 72;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.value = Item.sellPrice(0, 1, 0, 0);
			Item.rare = ItemRarityID.LightRed;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.UseSound = SoundID.Item46;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<FireflyMinionBuff>();
			Item.shoot = ModContent.ProjectileType<FireflyMinionProj>();
		}

        public override bool? UseItem(Player player)
        {
			if(player.altFunctionUse == 2)
			{
				if (_attackState == FireflyMinionProj.AttackState.Defense_Mode)
				{
					_attackState = FireflyMinionProj.AttackState.Attack_Mode;
					SoundEngine.PlaySound(SoundID.Item46, player.position);

				}
				else
				{
					_attackState = FireflyMinionProj.AttackState.Defense_Mode;
					SoundEngine.PlaySound(SoundID.Item43, player.position);
				}
				

				int fireflyMinionType = ModContent.ProjectileType<FireflyMinionProj>();
				//Loop over and swap the projectile attack states
				for (int i = 0; i < Main.maxProjectiles; i++)
				{
					Projectile other = Main.projectile[i];
					//Ignore projectiles that are not fireflies and are from a different owner.
					if (other.type != fireflyMinionType)
						continue;
					if (other.owner != player.whoAmI)
						continue;


                    FireflyMinionProj fireflyMinion = other.ModProjectile as FireflyMinionProj;
					fireflyMinion.AttackStyle = _attackState;

					//Dust Burst in Circle at Muzzle
					int count = 32;
					float degreesPer = 360 / (float)count;
					for (int k = 0; k < count; k++)
					{
						float degrees = k * degreesPer;
						Vector2 direction = Vector2.One.RotatedBy(MathHelper.ToRadians(degrees));
						Vector2 vel = direction * 8;
						Dust.NewDust(other.Center, 0, 0, DustID.CopperCoin, vel.X * 0.5f, vel.Y * 0.5f);
					}
				}

				return true;
            }

            return base.UseItem(player);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback,player.whoAmI);
			projectile.originalDamage = Item.damage;

            FireflyMinionProj fireflyMinion = projectile.ModProjectile as FireflyMinionProj;
			fireflyMinion.AttackStyle = _attackState;
			// Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
			return false;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
            recipe.AddIngredient(ItemType<StickOfWisdom>(), 1);
            recipe.AddIngredient(ItemType<PearlescentScrap>(), 12);
			recipe.AddIngredient(ItemType<LostScrap>(), 10);
			recipe.AddIngredient(ItemID.SoulofLight, 10);
			recipe.AddIngredient(ItemID.LifeCrystal, 1);
			recipe.Register();
		}
    }
}
