using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Astrasilk
{
    [AutoloadEquip(EquipType.Body)]
    public class AstrasilkBody : ModItem
    {
        public override void SetStaticDefaults()
        {
            // DisplayName.SetDefault("Astrasilk Jacket");
        }

        public override void SetDefaults()
        {
            Item.width = 30;
            Item.height = 30;
            Item.value = Item.sellPrice(0, 0, 20, 0);
            Item.rare = ItemRarityID.Blue;
            Item.defense = 4;
        }

        public override void UpdateEquip(Player player)
        {
            player.GetCritChance(DamageClass.Magic) += 15f;
        }
    }
}
