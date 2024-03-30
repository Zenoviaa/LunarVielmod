using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles;
using Stellamod.UI.Systems;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Summon
{
    internal class CanOfLeaves : ClassSwapItem
	{
		public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
			Item.damage = 18;
        }

        public override void SetDefaults()
		{
			Item.damage = 9;
			Item.mana = 1;
			Item.width = 20;
			Item.height = 20;
			Item.useTime = 23;
			Item.useAnimation = 23;
			Item.useStyle = ItemUseStyleID.Guitar;
			Item.staff[Item.type] = true;
			Item.noMelee = true;
			Item.knockBack = 0f;
			Item.DamageType = DamageClass.Summon;
			Item.value = 200;
			Item.rare = ItemRarityID.Blue;
			Item.UseSound = SoundID.DD2_DrakinShot;
			Item.autoReuse = true;
			Item.shoot = ModContent.ProjectileType<Logger>();
			Item.shootSpeed = 10f;
			Item.autoReuse = true;
			Item.crit = 15;
		}

		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			// NewProjectile returns the index of the projectile it creates in the NewProjectile array.
			// Here we are using it to gain access to the projectile object.
			int projectileID = Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI);
			Projectile projectile = Main.projectile[projectileID];

			ProjectileModifications globalProjectile = projectile.GetGlobalProjectile<ProjectileModifications>();
			// For more context, see ExampleProjectileModifications.cs
			globalProjectile.SetTrail(Color.DarkKhaki);
			globalProjectile.sayTimesHitOnThirdHit = false;
			globalProjectile.applyBuffOnHit = true;

			// We do not want vanilla to spawn a duplicate projectile.
			return false;
		}
	}
}







