using Microsoft.Xna.Framework;
using Stellamod.Projectiles;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged
{
    internal class IvynShot : ClassSwapItem
    {
        public override DamageClass AlternateClass => DamageClass.Throwing;
        public int combo;
        public override void SetClassSwappedDefaults()
        {
            Item.damage = 3;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            Item.damage = 5;
            Item.width = 50;
            Item.height = 50;
            Item.useStyle = ItemUseStyleID.Shoot;
            Item.knockBack = 4;
            Item.value = Item.sellPrice(0, 1, 1, 29);
            Item.rare = ItemRarityID.Blue;

            Item.shootSpeed = 16;
            Item.autoReuse = true;
            Item.DamageType = DamageClass.Ranged;
            Item.shoot = ProjectileID.PurificationPowder;
            Item.shootSpeed = 15f;
            Item.useAmmo = AmmoID.Arrow;
            Item.UseSound = SoundID.Item5;
            Item.useAnimation = 32;
            Item.useTime = 32;
            Item.consumeAmmoOnLastShotOnly = true;
            Item.noMelee = true;
        }


        public override Vector2? HoldoutOffset()
        {
            return new Vector2(0, 0f);
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            float rads = 16;
            combo++;
            if (combo >= 3)
            {
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(-rads)) * 0.5f, ModContent.ProjectileType<Logger>(), damage / 2, knockback, player.whoAmI);
                Projectile.NewProjectile(source, position, velocity.RotatedBy(MathHelper.ToRadians(rads)) * 0.5f, ModContent.ProjectileType<Logger>(), damage / 2, knockback, player.whoAmI);
                combo = 0;
            }

            return base.Shoot(player, source, position, velocity, type, damage, knockback);
        }
    }
}
