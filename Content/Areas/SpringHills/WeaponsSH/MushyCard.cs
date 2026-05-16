using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.CommonMaterials;

using Stellamod.Items;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.SpringHills.WeaponsSH
{
    public class MushyCard : BaseIgniterCard
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 3;
        }

        public override int GetPowderSlotCount()
        {
            return 2;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankCard>(),
                material: ModContent.ItemType<Mushroom>());
        }
    }
}