using Stellamod.Buffs;
using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ModLoader;

namespace Stellamod.Items.Flasks
{
    public class FloweredInsource : BaseInsource
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 3;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            InsourcePotionSickness = 1200;
            InsourceCannotUseDuration = 1200;
        }

        public override void TriggerEffect(Player player)
        {
            base.TriggerEffect(player);
            player.AddBuff(ModContent.BuffType<Friendzied>(), 300);
        }
    }
}