using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes
{
    public class BulletRework : GlobalItem
    {
        public override void SetDefaults(Item entity)
        {
            base.SetDefaults(entity);
            if (entity.ammo == AmmoID.Bullet)
            {
                entity.damage = 1;
            }
            if (entity.ammo == AmmoID.Arrow)
            {
                entity.damage = 5;
            }
        }
    }
}
