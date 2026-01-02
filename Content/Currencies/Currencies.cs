using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Content.Currencies
{
    public class RuinMedal : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 20, 0, 0);
        }
    }
    
    public class Ereshstyl : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 20, 0, 0);
        }
    }

    public class NoHitCrystal : ModItem
    {
        public override void SetDefaults()
        {
            Item.width = 20;
            Item.height = 20;
            Item.maxStack = Item.CommonMaxStack;
            Item.value = Item.buyPrice(0, 20, 0, 0);
        }
    }
}
