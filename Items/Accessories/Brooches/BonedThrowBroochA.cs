using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class BonedThrowBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.sellPrice(gold: 1);
            Item.rare = ItemRarityID.Green;
            Item.buffType = ModContent.BuffType<BonedB>();
            Item.accessory = true;
            BroochType = BroochType.Advanced;
        }

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            player.GetDamage(DamageClass.Throwing) *= 1.2f;
            player.ThrownVelocity += 5;
        }
    }
}