﻿using Microsoft.Xna.Framework;
using Terraria;
using Stellamod.Core.ItemTemplates;
using Urdveil.Common.Bases;

namespace Stellamod.Content.Items.Weapons.Ranged.Bows.IronBow
{
    internal class IronBowHold : BaseCrossbowProjectile
    {
        public override void Shoot(Vector2 position, Vector2 velocity)
        {
            base.Shoot(position, velocity);
            if (Owner.PickAmmo(Owner.HeldItem, out int projToShoot, out float speed, out int damage, out float knockBack, out int useAmmoItemId)
                && Main.myPlayer == Projectile.owner)
            {
                Vector2 fireVelocity = velocity * speed;
                fireVelocity *= 2f;
                fireVelocity *= ChargeStrength;

                float bowDamage = (float)damage * ChargeStrength;
                Projectile crossShot = Projectile.NewProjectileDirect(Projectile.GetSource_FromThis(), position, fireVelocity,
                    projToShoot,
                    (int)bowDamage, knockBack, Projectile.owner, ai0: projToShoot);
                crossShot.GetGlobalProjectile<CrossbowGlobalProjectile>().CrossbowShot = true;
            }


        }
    }
}