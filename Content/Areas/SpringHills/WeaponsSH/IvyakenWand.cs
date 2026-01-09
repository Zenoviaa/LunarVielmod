using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.Materials;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Items;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class IvyakenWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 35;
            Item.shootSpeed = 10;
            Item.useTime = 18;
            Item.useAnimation = 36;
            Size = 8;
            TrailLength = 16;
            normalSlotCount = 2;
            timedSlotCount = 1;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankStaff>();
        }
    }
}
