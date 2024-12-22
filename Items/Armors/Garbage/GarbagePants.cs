using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Armors.Garbage
{
    // The AutoloadEquip attribute automatically attaches an equip texture to this item.
    // Providing the EquipType.Legs value here will result in TML expecting a X_Legs.png file to be placed next to the item's main texture.
    [AutoloadEquip(EquipType.Legs)]
    public class GarbagePants : ModItem
    {
        public override void SetStaticDefaults()
        {
            CreativeItemSacrificesCatalog.Instance.SacrificeCountNeededByItemId[Type] = 1;
        }

        public override void SetDefaults()
        {
            Item.width = 22; // Width of the item
            Item.height = 12; // Height of the item
            Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
            Item.rare = ItemRarityID.Pink; // The rarity of the item
            Item.defense = 12; // The amount of defense the item will give when equipped
        }

        public override void UpdateEquip(Player player)
        {
            player.runAcceleration *= 1.05f;
            player.moveSpeed += 0.1f;
            player.maxRunSpeed += 0.1f; // Increase the movement speed of the player
            player.maxMinions += 1;
        }
    }
}