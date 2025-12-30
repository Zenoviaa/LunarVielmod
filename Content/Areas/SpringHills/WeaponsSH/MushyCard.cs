using Stellamod.Common.IgnitersNPowders;
using Stellamod.Content.Items.Materials;
using Stellamod.Items;
using Stellamod.Items.Materials.Molds;
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