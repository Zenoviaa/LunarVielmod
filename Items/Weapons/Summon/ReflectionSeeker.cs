using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Helpers;
using Stellamod.Projectiles.Summons.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class ReflectionSeeker : ModItem
	{
		public override void SetStaticDefaults()
		{
			ItemID.Sets.GamepadWholeScreenUseRange[Item.type] = true;
			ItemID.Sets.LockOnIgnoresCollision[Item.type] = true;
			CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
		}

		public override void SetDefaults()
		{
			Item.damage = 21;
			Item.DamageType = DamageClass.Summon;
			Item.mana = 5;
			Item.width = 26;
			Item.height = 28;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.noMelee = true;
			Item.knockBack = 1;
			Item.value = Item.buyPrice(0, 0, 100, 0);
			Item.rare = ModContent.RarityType<SirestiasSpecialRarity>();
			Item.UseSound = SoundID.Item44;
			Item.shoot = ModContent.ProjectileType<ReflectionSeekerMinionProj>();
			Item.buffType = ModContent.BuffType<ReflectionSeekerMinionBuff>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			position = Main.MouseWorld;
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;
			return false;
		}
	}
}