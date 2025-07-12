using Stellamod.Core.ItemTemplates;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.Weapons.Ranged.Bows.IronBow
{
    internal class IronBow : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 2;
            CrossbowProjectileType = ModContent.ProjectileType<IronBowHold>();
            staminaProjectileShoot = ModContent.ProjectileType<IronBowStaminaHold>();
        }
    }
}