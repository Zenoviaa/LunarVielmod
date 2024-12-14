using Microsoft.Xna.Framework;
using Terraria;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.Bases
{
    internal abstract class BaseChainedBallItem : ModItem
    {
        // You can use a vanilla texture for your item by using the format: "Terraria/Item_<Item ID>".
        public static Color OverrideColor = new(122, 173, 255);

        public override void SetDefaults()
        {
            // Start by using CloneDefaults to clone all the basic item properties from the vanilla Last Prism.
            // For example, this copies sprite size, use style, sell price, and the item being a magic weapon.
            Item.CloneDefaults(ItemID.LastPrism);
            Item.mana = 0;
            Item.channel = false;
            Item.DamageType = DamageClass.Ranged;
            Item.damage = 42;
            Item.shootSpeed = 30f;
        }

        // Because this weapon fires a holdout projectile, it needs to block usage if its projectile already exists.
        public override bool CanUseItem(Player player)
        {
            return player.ownedProjectileCounts[Item.shoot] == 0;
        }

        public override bool Shoot(Player player, EntitySource_ItemUse_WithAmmo source, Vector2 position, Vector2 velocity, int type, int damage, float knockback)
        {
            Projectile.NewProjectile(source, position, velocity, type, damage, knockback, player.whoAmI, ai2: 1);
            return false;
        }
    }
}
