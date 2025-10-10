using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Scrappy
{
    [AutoloadEquip(EquipType.Body)]
    public class ScrappyBody : ModItem
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
            player.endurance += 0.08f;
            player.maxMinions += 2;
            player.GetDamage(DamageClass.Summon) += 0.10f;
            player.GetDamage(DamageClass.Magic) += 0.10f;
        }

        // Please see Content/ExampleRecipes.cs for a detailed explanation of recipe creation.

    }
}
