using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class GhettingbergWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 800;
            Item.shootSpeed = 10;
            Size = 10;
            TrailLength = 32;
            Form = FormRegistry.Vase.Value;
            normalSlotCount = 5;
            timedSlotCount = 1;
            Item.mana = 100;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<MarshScrap>());
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<NaturalElement>());
            elements.Add(ModContent.ItemType<LightningElement>());
            elements.Add(ModContent.ItemType<GuutElement>());
            elements.Add(ModContent.ItemType<UvilisElement>());
        }
    }
}
