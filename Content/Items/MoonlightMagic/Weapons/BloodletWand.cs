using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class BloodletWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Squid.Value;
            Item.damage = 150;
            Item.mana = 100;
            Item.shootSpeed = 10;
            Size = 8;
            TrailLength = 16;
            normalSlotCount = 4;
            timedSlotCount = 0;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<TerrorFragments>());
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<BloodletElement>());
            elements.Add(ModContent.ItemType<NaturalElement>());
            elements.Add(ModContent.ItemType<GuutElement>());
        }
    }
}
