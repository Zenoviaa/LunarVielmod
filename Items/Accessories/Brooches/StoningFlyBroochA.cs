using Stellamod.Common.Bases;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Brooches
{
    public class StoningFlyBroochA : BaseBrooch
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 49;
            Item.height = 34;
            Item.value = Item.sellPrice(gold: 15);
            Item.rare = ItemRarityID.Green;
            Item.accessory = true;
            BroochType = BroochType.Radiant;
        }

        public override void UpdateBrooch(Player player)
        {
            base.UpdateBrooch(player);
            BroochSpawnerPlayer broochSpawnerPlayer = player.GetModPlayer<BroochSpawnerPlayer>();
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<FlyingFishBroochA>());
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<SlimeBroochA>());
            broochSpawnerPlayer.broochesToSpawn.Add(ModContent.ItemType<StoniaBroochA>());

            // the stone brooch nerfs it remember
            player.GetDamage(DamageClass.Generic) *= 1.25f;
        }
    }
}