using Stellamod.Items.Materials;
using Stellamod.Items.Materials.Molds;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class IllureCard : BaseIgniterCard
    {
        public override void SetClassSwappedDefaults()
        {
            base.SetClassSwappedDefaults();
            Item.damage = 9;
            Item.mana = 0;
        }

        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.damage = 18;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankCard>(), material: ModContent.ItemType<IllurineScale>());
        }
    }
}