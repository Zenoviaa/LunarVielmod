using Microsoft.Xna.Framework;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Projectiles.Crossbows
{
    internal class WoodenCrossbowHold : BaseCrossbowProjectile
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