using Microsoft.Xna.Framework;
using Stellamod.Items.Materials;
using Stellamod.Projectiles.Bow;
using Stellamod.Projectiles.Summons;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    public class IrradieagleWrath : ClassSwapItem
    {

        public override DamageClass AlternateClass => DamageClass.Generic;

        public override void SetClassSwappedDefaults()
        {
            Item.damage = 25;
            Item.mana = 0;
        }
        public override void SetStaticDefaults()
		{
			// DisplayName.SetDefault("Irradieagle's Wrath"); // By default, capitalization in classnames will add spaces to the display name. You can customize the display name here by uncommenting this line.
		}

		public override void SetDefaults()
		{
			Item.damage = 53;
			Item.DamageType = DamageClass.Ranged;
			Item.width = 78;
			Item.height = 130;
			Item.useTime = 4;
			Item.useAnimation = 12;
			Item.useStyle = ItemUseStyleID.Shoot;
			Item.knockBack = 6;
			Item.value = 10000;
			Item.rare = ItemRarityID.Blue;
			Item.shoot = ProjectileID.WoodenArrowFriendly;
			Item.shootSpeed = 3;
			Item.UseSound = SoundID.Item5;
            Item.autoReuse = true;
			Item.useAmmo = AmmoID.Arrow;
			Item.scale = 0.9f;
            Item.noMelee = true;
        }

		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}

        public override void ModifyShootStats(Player player, ref Vector2 position, ref Vector2 velocity, ref int type, ref int damage, ref float knockback)
        {
            base.ModifyShootStats(player, ref position, ref velocity, ref type, ref damage, ref knockback);
			velocity = -Vector2.UnitY;
			type = ModContent.ProjectileType<ToxicRainArrow>();
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			float numberProjectiles = 1;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 spawnPoint = position;
				spawnPoint.X = Main.MouseWorld.X + Main.rand.NextFloat(-64, 64);
				spawnPoint.Y -= 768;

				Vector2 newVelocity = Vector2.UnitY * Item.shootSpeed;
				newVelocity = newVelocity.RotatedByRandom(MathHelper.ToRadians(5));
				Projectile.NewProjectile(source, spawnPoint, newVelocity, type, damage, knockback, player.whoAmI);
			}
			return false;
		}

		
	}
}