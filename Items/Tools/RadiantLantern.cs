using Stellamod.Buffs;
using Stellamod.Common.Bases;
using Stellamod.Projectiles.Lanterns;
using Terraria.ModLoader;

namespace Stellamod.Items.Tools
{
    internal class RadiantLantern : BaseLanternItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.buffType = ModContent.BuffType<RadiatingLantern>();
            Item.shoot = ModContent.ProjectileType<RadiantLanternProjectile>();
        }
    }
}
