using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class MiracleWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 1650;
            Item.shootSpeed = 16;
            Size = 8;
            TrailLength = 16;
            Form = FormRegistry.Runic.Value;
            normalSlotCount = 5;
            timedSlotCount = 4;
            Item.mana = 70;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<MiracleThread>());
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<PrimeMagicElement>());
            elements.Add(ModContent.ItemType<DeeyaElement>());
            elements.Add(ModContent.ItemType<BloodletElement>());
            elements.Add(ModContent.ItemType<GuutElement>());
            elements.Add(ModContent.ItemType<HexElement>());
        }
    }
}
