using Terraria;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Items.Special.Sirestias;

public class SiresMail : ModItem
{
    public override void SetDefaults()
    {
        Item.width = 24;
        Item.height = 24;
        Item.useTime = 10;
        Item.useAnimation = 10;
        Item.useStyle = ItemUseStyleID.HoldUp;
    }


    public override bool? UseItem(Player player)
    {

        return true;
    }
}









