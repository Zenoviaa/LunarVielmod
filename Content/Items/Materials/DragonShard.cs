using Stellamod.Helpers;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.Content.Items.Materials
{
    public class DragonShard : ModItem
    {
        public override void SetDefaults()
        {
            base.SetDefaults();
            Item.maxStack = Item.CommonMaxStack;
        }
    }
}
