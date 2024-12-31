using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.MoonlightMagic.Elements;
using Stellamod.Items.MoonlightMagic.Forms;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.MoonlightMagic.Enchantments.Basic
{
    internal class RunicFormEnchantment : BaseEnchantment
    {
        bool HitOnce = false;
        int Attagain = 14;
        public override float GetStaffManaModifier()
        {
            return 0.1f;
        }

        public override int GetElementType()
        {
            return ModContent.ItemType<BasicElement>();
        }

        public override void SetMagicDefaults()
        {
            Projectile.velocity *= 1.5f;
            MagicProj.Form = FormRegistry.Runic.Value;
        }
    }
}
