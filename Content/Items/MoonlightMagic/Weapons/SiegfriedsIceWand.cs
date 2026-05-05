using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class SiegfriedsIceWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 1500;
            Item.shootSpeed = 8;
            Size = 20;
            TrailLength = 30;
            Form = FormRegistry.Tickler.Value;
            normalSlotCount = 6;
            timedSlotCount = 2;
            Item.mana = 60;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<IllurineScale>());
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<PrimeMagicElement>());
            elements.Add(ModContent.ItemType<DeeyaElement>());
            elements.Add(ModContent.ItemType<MothlightElement>());
            elements.Add(ModContent.ItemType<PhantasmalElement>());
            elements.Add(ModContent.ItemType<UvilisElement>());
        }
    }
}
