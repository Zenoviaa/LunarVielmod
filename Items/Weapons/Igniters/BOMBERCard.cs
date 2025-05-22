using Stellamod.Items.Materials.Molds;
using Stellamod.Items.Materials;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Weapons.Igniters
{
    internal class BOMBERCard : BaseIgniterCard
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
            Item.damage = 10;
        }
        public override void AddRecipes()
        {
            base.AddRecipes();
            this.RegisterBrew(mold: ModContent.ItemType<BlankAccessory>(), material: ModContent.ItemType<AlcaricMush>());
        }
    }
}