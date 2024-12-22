using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged.Crossbows
{

    internal class MerchantCrossbow : BaseCrossbowItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 14;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 29;
            CrossbowProjectileType = ModContent.ProjectileType<MerchantCrossbowHold>();
        }
    }

    internal class MerchantCrossbowHold : BaseCrossbowProjectile
    {
        public override void Shoot(Vector2 position, Vector2 velocity)
        {
            base.Shoot(position, velocity);
            if (Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId)
                && Main.myPlayer == Projectile.owner)
            {
                Vector2 fireVelocity = velocity * speed;
                fireVelocity *= 2f;
                Projectile crossShot = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), position, fireVelocity,
                    projToShoot,
                    damage, knockBack, Projectile.owner, ai0: projToShoot);
                crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().CrossbowShot = true;
                crossShot.netUpdate = true;
            }


        }
    }
}