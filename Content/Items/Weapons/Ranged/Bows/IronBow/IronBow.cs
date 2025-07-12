using Urdveil.Common.Bases;
using Terraria;
using Terraria.ModLoader;
using Stellamod.Core.ItemTemplates;

namespace Stellamod.Content.Items.Weapons.Ranged.Bows.IronBow
{
    internal class IronBow : BaseCrossbowItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            CrossbowProjectileType = ModContent.ProjectileType<IronBowHold>();
        }
    }
}