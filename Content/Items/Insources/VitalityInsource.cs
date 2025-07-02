using Stellamod.Core.XixianFlaskSystem;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Stellamod.Content.Items.Insources
{
    public class VitalityInsource : BaseInsource
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InsourcePotionSickness = 2400;
            InsourceCannotUseDuration = 2400;
        }

        public override void TriggerEffect(Player player)
        {
            base.TriggerEffect(player);
            player.statMana += 102;
            player.AddBuff(BuffID.Honey, 2400);
        }
    }
}