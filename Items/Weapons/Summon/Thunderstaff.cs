using Microsoft.Xna.Framework;
using Stellamod.Buffs.Minions;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Summons.Minions;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    public class Thunderstaff : ModItem
    {
        public override void SetDefaults()
        {
			Item.damage = 48;
			Item.knockBack = 3f;
			Item.mana = 10;
			Item.width = 48;
			Item.height = 62;
			Item.useTime = 36;
			Item.useAnimation = 36;
			Item.useStyle = ItemUseStyleID.Swing;
            Item.value = Item.sellPrice(0, 1, 33, 0);
            Item.rare = ItemRarityID.LightRed;

			// These below are needed for a minion weapon
			Item.noMelee = true;
			Item.UseSound = SoundID.Item46;
			Item.DamageType = DamageClass.Summon;
			Item.buffType = ModContent.BuffType<CloudMinionBuff>();
			Item.shoot = ModContent.ProjectileType<CloudMinionProj>();
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// This is needed so the buff that keeps your minion alive and allows you to despawn it properly applies
			player.AddBuff(Item.buffType, 2);

			// Minions have to be spawned manually, then have originalDamage assigned to the damage of the summon item
			position = Main.MouseWorld;
			var projectile = Projectile.NewProjectileDirect(source, position, velocity, type, damage, knockback, player.whoAmI);
			projectile.originalDamage = Item.damage;

			// Since we spawned the projectile manually already, we do not need the game to spawn it for ourselves anymore, so return false
			return false;
		}

        public override void AddRecipes()
        {
            base.AddRecipes();
			Recipe recipe = CreateRecipe();
			recipe.AddTile(TileID.MythrilAnvil);
			recipe.AddIngredient(ModContent.ItemType<StickOfWisdom>(), 1);
			recipe.AddIngredient(ItemID.SoulofFlight, 12);
			recipe.AddIngredient(ModContent.ItemType<PearlescentScrap>(), 8);
			recipe.AddIngredient(ModContent.ItemType<WinterbornShard>(), 6);
			recipe.Register();
		}
    }
}
