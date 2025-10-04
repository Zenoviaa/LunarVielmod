
using Stellamod.Items.Armors.Vanity.Gothivia;
using Stellamod.Items.Harvesting;
using Stellamod.Items.Materials;
using Stellamod.Items.Placeable;
using Stellamod.Items.Quest.BORDOC;
using Stellamod.Items.Weapons.Melee;
using Stellamod.Items.Weapons.Summon;
using Stellamod.Items.Weapons.Thrown;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Quest.Merena
{
    internal class OrbOfTheMorrow : ModItem
    {

        public override void SetDefaults()
        {
            Item.width = 20; // The item texture's width
            Item.height = 20; // The item texture's height

            Item.maxStack = 1; // The item's max stack value
            Item.value = Item.buyPrice(silver: 1); // The value of the item in copper coins. Item.buyPrice & Item.sellPrice are helper methods that returns costs in copper coins based on platinum/gold/silver/copper arguments provided to it.
            Item.rare = ItemRarityID.Quest;
        }
    }
}
