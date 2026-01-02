using Terraria;
using Terraria.GameContent.Creative;
using Terraria.ID;

namespace Stellamod.Items.Materials.Molds
{
    public class BlankStaff : BaseMold
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height

            Item.maxStack = Item.CommonMaxStack; // The item's max stack value
            Item.value = Item.buyPrice(gold: 5); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ItemRarityID.Green;
        }
    }
}
