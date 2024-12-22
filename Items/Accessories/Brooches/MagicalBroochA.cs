using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class MagicalBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 25);
            Item.rare = ItemRarityID.LightPurple;
            Item.buffType = ModContent.BuffType<MagicalBroo>();
            Item.accessory = true;
            BroochType = BroochType.Advanced;
        }

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            player.GetDamage(DamageClass.Magic) *= 1.2f;
        }
    }
}