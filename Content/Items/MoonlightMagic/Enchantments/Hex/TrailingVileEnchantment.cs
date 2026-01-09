using Stellamod.Content.Items.MoonlightMagic.Elements;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Enchantments.Hex
{
    public class TrailingVileEnchantment : BaseEnchantment
    {
        public override void SetMagicDefaults()
        {
            base.SetMagicDefaults();
            MagicProj.damagingTrail = true;
        }
        public override int GetElementType()
        {
            return ModContent.ItemType<HexElement>();
        }

    }
}
