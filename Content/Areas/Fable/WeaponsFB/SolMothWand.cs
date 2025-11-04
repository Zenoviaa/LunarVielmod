using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class SolMothWand : BaseStaff
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 14;
            Item.shootSpeed = 10;
            Item.useTime = 18;
            Item.useAnimation = 36;
            Size = 8;
            TrailLength = 16;
        }

        public override int GetNormalSlotCount()
        {
            return 1;
        }

        public override int GetTimedSlotCount()
        {
            return 2;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankStaff>();
        }
    }
}
