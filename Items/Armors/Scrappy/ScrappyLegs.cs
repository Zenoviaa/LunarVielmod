using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Scrappy
{
    [AutoloadEquip(EquipType.Legs)]
    internal class ScrappyLegs : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 12; // Height of the item
            Item.value = Item.sellPrice(gold: 5); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 10; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.maxRunSpeed += 0.04f;
            player.runAcceleration += 0.12f;
            player.maxMinions += 1;
            player.GetDamage(DamageClass.Summon) += 0.08f;
            player.GetDamage(DamageClass.Magic) += 0.08f;
        }
    }
}
