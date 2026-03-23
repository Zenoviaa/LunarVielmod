using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Items;
using Stellamod.Items.Ores;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.Collosseum.WeaponsCL;

public class BuriedWand : AbstractMagicWand
{
    public override void SetDefaults2()
    {
        base.SetDefaults2();
        Item.damage = 16;
        Item.shootSpeed = 10;
        Item.useTime = 18;
        Item.useAnimation = 36;
        Size = 8;
        TrailLength = 16;
        normalSlotCount = 2;
        timedSlotCount = 2;
    }

    public override void ModifyElementPreferences(List<int> elements)
    {
        base.ModifyElementPreferences(elements);
        elements.Add(ModContent.ItemType<NaturalElement>());
        elements.Add(ModContent.ItemType<LightningElement>());
        elements.Add(ModContent.ItemType<WindElement>());
    }

    public override void AddRecipes()
    {
        base.AddRecipes();
        this.RegisterBrew<GintzlMetal, BlankStaff>();
    }
}