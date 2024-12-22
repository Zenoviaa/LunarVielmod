using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Stellamod.Items.Flasks
{
    public class EpsidonInsource : BaseInsource
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InsourceHealValue = 200;
            InsourceCannotUseDuration = 2400;
            InsourcePotionSickness = 2400;
        }

        public override void TriggerEffect(Player player)
        {
            base.TriggerEffect(player);
            player.AddBuff(BuffID.Honey, 1200);
            player.AddBuff(BuffID.Swiftness, 1200);
            player.AddBuff(BuffID.Ironskin, 1200);
        }
    }
}