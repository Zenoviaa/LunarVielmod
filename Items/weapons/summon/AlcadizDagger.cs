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
			Item.damage = 7;
			Item.knockBack = 1;
			Item.mana = 10;
			Item.useStyle = ItemUseStyleID.Swing;
			Item.useTime = Item.useAnimation = 120;
			Item.DamageType = DamageClass.Summon;
			Item.autoReuse = true;
			Item.noUseGraphic = true;
			Item.shoot = ModContent.ProjectileType<Windeffect>();
			Item.shootSpeed = 8;
			Item.UseSound = SoundID.Item1;

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