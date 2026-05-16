using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class CelestiaWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 24;
            Item.mana = 50;
            Item.shootSpeed = 12;
            Size = 12;
            TrailLength = 18;
            normalSlotCount = 0;
            timedSlotCount = 2;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<Ivythorn>());
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<PhantasmalElement>());
            elements.Add(ModContent.ItemType<BasicElement>());
        }


    }
}