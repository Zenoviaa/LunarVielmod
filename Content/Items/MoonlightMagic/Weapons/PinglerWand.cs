using Stellamod.Content.CommonMaterials;

using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using Stellamod.Items.Materials;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class PinglerWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Arrow.Value;
            Item.damage = 18;
            Item.mana = 35;
            Size = 16;
            TrailLength = 8;
            normalSlotCount = 1;
            timedSlotCount = 0;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<Mushroom>());
        }

       
    }
}
