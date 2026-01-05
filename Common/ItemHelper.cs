using Stellamod.Common.SummonerSystem;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    public class ItemHelper : ModSystem
    {
        public static List<Item> BellMinions { get; private set; }
        public override void OnModLoad()
        {
            base.OnModLoad();

        }

       
        public override void OnModUnload()
        {
            base.OnModUnload();
            BellMinions = null;
        }
        public override void PostAddRecipes()
        {
            base.PostAddRecipes();
            //We're doing it in post add recipes instead of on modload because doing this in mod load
            //causes a race condition with the global item check for some reason
            //maybe there's a better way to do this, but this works, so.
            BellMinions = new List<Item>();
            foreach (var modItem in ModContent.GetContent<ModItem>())
            {
                if (modItem.Item.TryGetGlobalItem<BellMinionGlobalItem>(out BellMinionGlobalItem item))
                {
                    if (item.isBellMinion)
                    {
                        BellMinions.Add(modItem.Item);
                    }
                }
            }
        }
    }
}
