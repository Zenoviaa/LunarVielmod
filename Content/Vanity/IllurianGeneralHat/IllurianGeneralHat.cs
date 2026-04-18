using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.IllurianGeneralHat;


[AutoloadEquip(EquipType.Head)]
public class IllurianGeneralHat : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 18; // Width of the item
        Item.height = 18; // Height of the item
        Item.value = Item.sellPrice(gold: 10); // How many coins the item is worth
        Item.rare = ItemRarityID.LightPurple;// The rarity of the item
        Item.vanity = true;
    }
}
