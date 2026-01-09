using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using System;
using System.Collections.Generic;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class ZuisGiftedWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 5;
            Item.shootSpeed = 13;
            Item.useTime = 30;
            Item.useAnimation = 60;
            Size = 12;
            TrailLength = 20;
            Form = FormRegistry.Sword.Value;
            normalSlotCount = 2;
            timedSlotCount = 1;
        }
        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<HolinessElement>());
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<UvilisElement>());
            elements.Add(ModContent.ItemType<GuutElement>());
            elements.Add(ModContent.ItemType<BloodletElement>());
        }
    }
}
