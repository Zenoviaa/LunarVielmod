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

    public class CresbrahntWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Form = FormRegistry.Sword.Value;
            Item.damage = 1000;
        }

        public override int GetNormalSlotCount()
        {
            return 7;
        }

        public override int GetTimedSlotCount()
        {
            return 6;
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
        /*
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankStaff>();
        }
        */
    }
}
