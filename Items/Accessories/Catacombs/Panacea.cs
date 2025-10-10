using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Accessories.Catacombs
{
    public class SpikeResistPlayer : ModPlayer
    {
        public bool hasPanacea;
        public override void ResetEffects()
        {
            hasPanacea = false;
        }
    }

    public class Panacea : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 28;
            Item.height = 36;
            Item.rare = ItemRarityID.LightRed;
            Item.accessory = true;
            Item.value = Item.sellPrice(gold: 2);
        }

        public override void UpdateAccessory(Player player, bool hideVisual)
        {
            player.GetModPlayer<SpikeResistPlayer>().hasPanacea = true;
            player.ClearBuff(BuffID.Bleeding);
            player.ClearBuff(BuffID.Poisoned);
            player.ClearBuff(BuffID.Venom);
            player.statLifeMax2 += 40;
        }
    }
}
