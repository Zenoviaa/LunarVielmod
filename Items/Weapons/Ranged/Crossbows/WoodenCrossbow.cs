using Stellamod.Common.Bases;
using Stellamod.Projectiles.Crossbows;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Ranged.Crossbows
{
    internal class WoodenCrossbow : BaseCrossbowItem
    {

        public override DamageClass AlternateClass => DamageClass.Magic;

        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 12;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            CrossbowProjectileType = ModContent.ProjectileType<WoodenCrossbowHold>();
        }
    }
}