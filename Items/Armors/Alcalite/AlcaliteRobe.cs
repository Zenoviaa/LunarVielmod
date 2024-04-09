using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Alcalite
{
    [AutoloadEquip(EquipType.Body)]
    internal class AlcaliteRobe : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34; // Width of the item
            Item.height = 26; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime;// The rarity of the item
            Item.defense = 10; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.12f;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.maxMinions += 2;
        }
    }
}
