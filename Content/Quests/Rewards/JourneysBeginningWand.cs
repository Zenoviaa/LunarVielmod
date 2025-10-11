using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Quests.Rewards
{

    public class JourneysBeginningWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Form = FormRegistry.SmallKnife.Value;
            Item.damage = 320;
        }

        public override int GetNormalSlotCount()
        {
            return 3;
        }

        public override int GetTimedSlotCount()
        {
            return 2;
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<HexElement>());
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<PhantasmalElement>());
        }
    }
}
