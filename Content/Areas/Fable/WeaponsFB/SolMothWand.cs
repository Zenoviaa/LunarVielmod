using Stellamod.Content.CommonMaterials;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Items;
using Terraria;

namespace Stellamod.Content.Areas.Fable.WeaponsFB
{
    public class SolMothWand : AbstractMagicWand
    {
        public override void SetDefaults2()
        {
            base.SetDefaults2();
            Item.damage = 14;
            Item.shootSpeed = 10;
            Item.useTime = 18;
            Item.useAnimation = 36;
            Size = 8;
            TrailLength = 16;
            normalSlotCount = 1;
            timedSlotCount = 2;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew<AlcadizScrap, BlankStaff>();
        }
    }
}
