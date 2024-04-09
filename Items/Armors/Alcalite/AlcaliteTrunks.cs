using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Alcalite
{
    [AutoloadEquip(EquipType.Legs)]
    internal class AlcaliteTrunks : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 26; // Width of the item
            Item.height = 12; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime;// The rarity of the item
            Item.defense = 9; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.GetDamage(DamageClass.Magic) += 0.04f;
            player.GetDamage(DamageClass.Summon) += 0.04f;
            player.maxMinions += 1;
            player.runAcceleration *= 1.025f;
            player.moveSpeed += 0.1f;
            player.maxRunSpeed += 0.1f;
        }
    }
}
