using Terraria.ID;
using Terraria.ModLoader;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Stellamod.Projectiles;
using Terraria.DataStructures;
using Stellamod.Projectiles.StringnNeedles.Alcadiz;

namespace Stellamod.Items.weapons.summon
{
	public class AlcadizDagger : ModItem
	{
		public override void SetStaticDefaults()
		{
			DisplayName.SetDefault("Alcadiz String n Needle");
			Tooltip.SetDefault("Your summons will target focus enemies" +
				"\nSummons will manifest out of your needle " +
				"\nThey will act as temporary summons to give your other minions company!");
		}


		public override void SetDefaults()
		{
			Item.width = 20;
			Item.height = 20;
			Item.rare = ItemRarityID.Green;
			Item.value = Terraria.Item.sellPrice(0, 5, 80, 0);
			Item.CloneDefaults(ItemID.Arkhalis);
			Item.damage = 7; // Sets the Item's damage. Note that projectiles shot by this weapon will use its and the used ammunition's damage added together.
			Item.DamageType = DamageClass.Ranged;
			Item.mana = 100;
			Item.useTime = 90; // The Item's use time in ticks (60 ticks == 1 second.)
			Item.useAnimation = 30; // The length of the Item's use animation in ticks (60 ticks == 1 second.)
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.noMelee = true; //so the Item's animation doesn't do damage
			Item.knockBack = 4; // Sets the Item's knockback. Note that projectiles shot by this weapon will use its and the used ammunition's knockback added together.
			Item.value = 10000; // how much the Item sells for (measured in copper)
			Item.UseSound = SoundID.Item11; // The sound that this Item plays when used.
			Item.autoReuse = true; // if you can hold click to automatically use it again
			Item.shoot = ModContent.ProjectileType<StringNNeedlesAlcadiz>();
			Item.shootSpeed = 0f; // the speed of the projectile (measured in pixels per frame)
			Item.channel = true;

		}
		public override void PostDrawInWorld(SpriteBatch spriteBatch, Color lightColor, Color alphaColor, float rotation, float scale, int whoAmI)
		{
			Lighting.AddLight(Item.position, 0.46f, .07f, .52f);


		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{

			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, Main.myPlayer);
			projectile.originalDamage = Item.damage;

			// Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
			return false;
		}
	}
}