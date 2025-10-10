using Stellamod.Content.Items.MoonlightMagic.Forms;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class CrescentStaff : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Form = FormRegistry.FourPointedStar.Value;
        }
    }
}
