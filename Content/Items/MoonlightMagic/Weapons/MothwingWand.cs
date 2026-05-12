using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class MothwingWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 1500;
            Item.shootSpeed = 16;
            Size = 8;
            TrailLength = 16;
            Form = FormRegistry.Fairy.Value;
            normalSlotCount = 2;
            timedSlotCount = 6;
            Item.mana = 100;
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
