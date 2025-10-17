using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{

    public class WandOfHypnolight : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Form = FormRegistry.Fairy.Value;
            Item.damage = 100;
        }

        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 2;
        }

        public override void ModifyElementPreferences(List<int> elements)
        {
            base.ModifyElementPreferences(elements);
            elements.Add(ModContent.ItemType<PrismaticElement>());
            elements.Add(ModContent.ItemType<RadianceElement>());
            elements.Add(ModContent.ItemType<UvilisElement>());
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<HypnotizedSoul, BlankStaff>();
        }
    }
}
