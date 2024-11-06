using Terraria.GameContent.Creative;

namespace Stellamod.Items.Flasks
{
    public class HealthyInsource : BaseInsource
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InsourceHealValue = 45;
            InsourceCannotUseDuration = 2400;
            InsourcePotionSickness = 2400;
        }
    }
}