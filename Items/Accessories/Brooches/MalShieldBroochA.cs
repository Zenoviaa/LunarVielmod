using Stellamod.Buffs.Charms;
using Stellamod.Common.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class MalShieldBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 24;
            Item.height = 28;
            Item.value = Item.buyPrice(0, 0, 90);
            Item.rare = ItemRarityID.Green;
            Item.buffType = ModContent.BuffType<MalB>();
            Item.accessory = true;
        }

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            player.statDefense += 7;
            player.GetDamage(DamageClass.Melee) *= 1.07f;
        }
    }
}