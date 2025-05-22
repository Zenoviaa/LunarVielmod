using Stellamod.Items.MoonlightMagic.Forms;

namespace Stellamod.Items.MoonlightMagic.Weapons
{
    internal class CrescentStaff : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Form = FormRegistry.FourPointedStar.Value;
        }
    }
}
