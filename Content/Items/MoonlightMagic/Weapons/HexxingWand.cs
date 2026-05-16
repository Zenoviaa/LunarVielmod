using Stellamod.Content.CommonMaterials;

using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class HexxingWand : AbstractMagicWand
    {
        
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Snowglobe.Value;
            Item.damage = 450;
            Item.mana = 100;
            Item.shootSpeed = 10;
            Size = 16;
            TrailLength = 32;
            normalSlotCount = 4;
            timedSlotCount = 2;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<PearlescentScrap>());
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<PhantasmalElement>());
            elements.Add(ModContent.ItemType<MothlightElement>());
            elements.Add(ModContent.ItemType<HexElement>());
        }
    }
}

