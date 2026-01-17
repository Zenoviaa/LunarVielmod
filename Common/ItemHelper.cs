using Stellamod.Common.SummonerSystem;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.Items.MoonlightMagic;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Common
{
    /// <summary>
    /// Manages collections of our different item types so we don't have to constantly look them up
    /// </summary>
    public class ItemHelper : ModSystem
    {
        public static Item[] BellMinions { get; private set; }
        public static Item[] Insources { get; private set; }
        public static BaseEnchantment[] Enchantments { get; private set; }
        public static BaseEnchantment[] SpecialEnchantments { get; private set; }
        public override void OnModUnload()
        {
            base.OnModUnload();
            BellMinions = null;
            Insources = null;
            Enchantments = null;
            SpecialEnchantments = null;
        }

        public override void PostAddRecipes()
        {
            base.PostAddRecipes();
            //We're doing it in post add recipes instead of on modload because doing this in mod load
            //causes a race condition with the global item check for some reason
            //maybe there's a better way to do this, but this works, so.
            var minionCollection = new List<Item>();
            var insourceCollection = new List<Item>();
            var enchantmentCollection = new List<BaseEnchantment>();
            var specialEnchantmentCollection = new List<BaseEnchantment>();
            IEnumerable<ModItem> modItemCollection = ModContent.GetContent<ModItem>();
            foreach (var modItem in modItemCollection)
            {
                if (modItem.Item.TryGetGlobalItem(out BellMinionGlobalItem bellMinion))
                {
                    if (bellMinion.isBellMinion)
                    {
                        Item itemClone = new Item(modItem.Item.type);
                        //The template instance does not have all the global items and whatnot applied to them
                        minionCollection.Add(itemClone);
                    }
                    else if (bellMinion.isGuardian)
                    {
                        Item itemClone = new Item(modItem.Item.type);
                        //The template instance does not have all the global items and whatnot applied to them
                        minionCollection.Add(itemClone);
                    }
                }
                if (modItem is InsourceItem)
                {
                    insourceCollection.Add(modItem.Item);
                }
                if (modItem is BaseEnchantment enchantment)
                {
                    enchantmentCollection.Add(enchantment);
                    if (enchantment.isSpecial)
                    {
                        specialEnchantmentCollection.Add(enchantment);
                    }
                }
            }

            Insources = insourceCollection.ToArray();
            BellMinions = minionCollection.ToArray();
            Enchantments = enchantmentCollection.ToArray();
            SpecialEnchantments = specialEnchantmentCollection.ToArray();
        }
    }
}
