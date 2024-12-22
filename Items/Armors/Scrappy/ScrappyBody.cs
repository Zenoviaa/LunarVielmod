using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Scrappy
{
    [AutoloadEquip(EquipType.Body)]
    internal class ScrappyBody : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 34; // Width of the item
            Item.height = 20; // Height of the item
            Item.value = Item.sellPrice(gold: 6); // How many coins the item is worth
            Item.rare = ItemRarityID.Lime; // The rarity of the item
            Item.defense = 18; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.lifeRegen += 3;
            player.endurance += 0.08f;
            player.maxMinions += 2;
            player.GetDamage(DamageClass.Summon) += 0.12f;
            player.GetDamage(DamageClass.Magic) += 0.12f;
        }
    }
}
