using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.MothlightManor.WeaponsMM
{

    public class CresbrahntWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Sword.Value;
            Item.damage = 2500;
            Item.mana = 45;
            normalSlotCount = 7;
            timedSlotCount = 5;
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<PrimeMagicElement>());
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<HexElement>());
            elements.Add(ModContent.ItemType<MothlightElement>());
            elements.Add(ModContent.ItemType<PhantasmalElement>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            //   this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<MarshScrap>());
        }
    }
}

