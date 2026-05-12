using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using Stellamod.Items.Harvesting;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class UselessWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Ship.Value;
            Item.damage = 1;
            Item.mana = 10;
            normalSlotCount = 7;
            timedSlotCount = 7;
        }


    }
}
