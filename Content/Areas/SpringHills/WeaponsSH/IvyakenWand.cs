using Stellamod.Content.Items.Materials;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class IvyakenWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 35;
            Item.shootSpeed = 10;
            Item.useTime = 18;
            Item.useAnimation = 36;
            Size = 8;
            TrailLength = 16;
        }

        public override int GetNormalSlotCount()
        {
            return 2;
        }

        public override int GetTimedSlotCount()
        {
            return 1;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<Ivythorn, BlankStaff>();
        }
    }
}
