using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using Stellamod.Items.Materials;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class RainsyWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Aztec.Value;
            Item.damage = 800;
            Item.mana = 70;
            Size = 16;
            TrailLength = 8;
            normalSlotCount = 1;
            timedSlotCount = 5;
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
            elements.Add(ModContent.ItemType<CheckersElement>());
        }
    }
}
