using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class TheSunAndMoonCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 5;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 9;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankCard>(), material: ModContent.ItemType<PearlescentScrap>());
        }
    }
}