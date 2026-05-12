using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic.Elements;
using Stellamod.Content.Items.MoonlightMagic.Forms;
using Stellamod.Items;
using System.Collections.Generic;
using Terraria.ModLoader;
namespace Stellamod.Content.Items.MoonlightMagic.Weapons
{
    public class RewinderWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Form = FormRegistry.Gear.Value;
            Item.damage = 1200;
            Item.mana = 40;
            Size = 30;
            TrailLength = 30;
            normalSlotCount = 1;
            timedSlotCount = 6;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankStaff>(), material: ModContent.ItemType<MechanizedSoul>());
        }

      
    }
}
