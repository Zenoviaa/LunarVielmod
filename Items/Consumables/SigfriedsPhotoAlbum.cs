

using Stellamod.Helpers;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Items.Consumables
{
    public class SigfriedsPhotoAlbum : ModItem
    {
        public override void SetDefaults()
        {
            Item.maxStack = 1;
            Item.width = 18;
            Item.height = 28;
            Item.value = Item.sellPrice(0, 0, 1, 0);

        }



    }
}