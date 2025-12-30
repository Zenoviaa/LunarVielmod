using Stellamod.Common.IgnitersNPowders;
using Stellamod.Items;
using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Areas.WondrousDarkspace.WeaponsWD
{
    public class LarvaedCard : BaseIgniterCard
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 13;
        }

        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankCard>(), material: ModContent.ItemType<HypnotizedSoul>());
        }
    }
}