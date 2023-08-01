using Terraria.DataStructures;

using Terraria.ID;
using Terraria.ModLoader;
using static Terraria.ModLoader.ModContent;
using Terraria;
using Microsoft.Xna.Framework;
using Stellamod.Projectiles.Weapons.Magic;

namespace Stellamod.Items.Weapons.Mage
{
    public class StormsWrath : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Storm's Wrath");
        }
        public override void SetDefaults()
        {
            Item.staff[Item.type] = true;
            Item.damage = 12;
            Item.DamageType = DamageClass.Magic;
            Item.width = 40;
			Item.mana = 15;
			Item.height = 40;
            Item.useTime = 25;
            Item.useAnimation = 25;
            Item.useStyle = 5;
            Item.knockBack = 6;
            Item.value = Item.sellPrice(0, 0, 80, 0);
            Item.rare = 2;
            Item.UseSound = SoundID.Item92;
            Item.autoReuse = true;
            Item.shoot = ProjectileType<SpirtFlareGood>();
            Item.shootSpeed = 25f;
        }
		public override Vector2? HoldoutOffset()
		{
			return new Vector2(-2, 0);
		}
		public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
		{
			type = ModContent.ProjectileType<SpirtFlareGood>();
			float numberProjectiles = 3;
			float rotation = MathHelper.ToRadians(10);
			position += Vector2.Normalize(new Vector2(velocity.X, velocity.Y)) * 45f;
			for (int i = 0; i < numberProjectiles; i++)
			{
				Vector2 perturbedSpeed = new Vector2(velocity.X, velocity.X).RotatedBy(MathHelper.Lerp(-rotation, rotation, i / (numberProjectiles - 1))) * .4f; // This defines the projectile roatation and speed. .4f == projectile speed
				Projectile.NewProjectile(source, position.X, position.Y, perturbedSpeed.X, perturbedSpeed.Y, type, damage, Item.knockBack, player.whoAmI);
			}

			return false; // return false because we don't want tmodloader to shoot projectile
		}
	}
}
