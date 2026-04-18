using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Content.Vanity.AcademyOutfit;

[AutoloadEquip(EquipType.Head)]
public class AcademyOutfitHead : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
        ArmorIDs.Head.Sets.DrawHatHair[Item.headSlot] = true;
    }

    public override void SetDefaults()
    {
        Item.width = 18;
        Item.height = 18;
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Body)]
public class AcademyOutfitRobe : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        Item.width = 18; 
        Item.height = 18; 
        Item.value = Item.sellPrice(gold: 1); 
        Item.rare = ItemRarityID.Green;
        Item.vanity = true;
    }
}

[AutoloadEquip(EquipType.Legs)]
public class AcademyOutfitLegs : ModItem
{
    public override void SetStaticDefaults()
    {
        base.SetStaticDefaults();
    }

    public override void SetDefaults()
    {
        Item.width = 18; 
        Item.height = 18; 
        Item.value = Item.sellPrice(gold: 1);
        Item.rare = ItemRarityID.Green;
        Item.vanity = true;
    }
}

