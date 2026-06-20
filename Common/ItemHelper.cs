using Stellamod.Common.SummonerSystem;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Content.Ammo;
using Stellamod.Content.Bar.Drinks;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Projectiles.Arrows;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
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
        public static Item[] PermanentFoods { get; private set; }
        public static List<Item> Act1Ammos { get; private set; }
        public static List<Item> Act2Ammos { get; private set; }
        public static List<Item> Act3Ammos { get; private set; }
        public override void OnModUnload()
        {
            base.OnModUnload();
            BellMinions = null;
            Insources = null;
            Enchantments = null;
            SpecialEnchantments = null;
            PermanentFoods = null;
            Act1Ammos = null;
            Act2Ammos = null;
            Act3Ammos = null;
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
            var permanentFoodCollection = new List<Item>();
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
                if(modItem.Item.TryGetGlobalItem(out PermamentFoodGlobalItem permamentFoodGlobalItem))
                {
                    if (permamentFoodGlobalItem.isPermanentFood)
                    {
                        permanentFoodCollection.Add(modItem.Item);
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
            PermanentFoods = permanentFoodCollection.ToArray();


            int act = 0;
            void AddAmmo(int type)
            {
                List<Item> ammos = null;
                switch (act)
                {
                    default:
                    case 0:
                        ammos = Act1Ammos;
                        break;
                    case 1:
                        ammos = Act2Ammos;
                        break;
                    case 2:
                        ammos = Act3Ammos;
                        break;
                }

                ammos.Add(new Item(type));
            }

            Act1Ammos = new List<Item>();
            Act2Ammos = new List<Item>();
            Act3Ammos = new List<Item>();

            AddAmmo(ItemID.WoodenArrow);
            AddAmmo(ItemID.HellfireArrow);
            AddAmmo(ItemID.JestersArrow);
            AddAmmo(ItemID.UnholyArrow);
            AddAmmo(ItemID.FlamingArrow);
            AddAmmo(ItemID.FrostburnArrow);
            AddAmmo(ItemID.BoneArrow);
  
            AddAmmo(ItemID.ShimmerArrow);
            //AddAmmo(ModContent.ItemType<BladedA>)
            AddAmmo(ItemID.MusketBall);
            AddAmmo(ItemID.SilverBullet);
            AddAmmo(ItemID.MeteorShot);
            AddAmmo(ItemID.PartyBullet);
            AddAmmo(ModContent.ItemType<SpacialDistortionBullet>());
            AddAmmo(ItemID.GoldenBullet);
            AddAmmo(ItemID.ExplodingBullet);

            act = 1;
            //Celestial Arrow
            AddAmmo(ModContent.ItemType<EldritchArrow>());
            AddAmmo(ItemID.IchorArrow);
            AddAmmo(ItemID.CursedArrow);
            AddAmmo(ItemID.VenomArrow);
            AddAmmo(ItemID.ChlorophyteArrow);
            AddAmmo(ItemID.HolyArrow);
            AddAmmo(ModContent.ItemType<VoidArrowItem>());

            //Bullets
            AddAmmo(ItemID.CrystalBullet);
            AddAmmo(ItemID.NanoBullet);
            AddAmmo(ModContent.ItemType<DriveRound>());
            AddAmmo(ModContent.ItemType<AdamantiteBullet>());
            AddAmmo(ModContent.ItemType<TitaniumBullet>());
            AddAmmo(ItemID.CursedBullet);
            AddAmmo(ItemID.IchorBullet);

            act = 2;
            AddAmmo(ModContent.ItemType<FlowerArrowItem>());
            AddAmmo(ModContent.ItemType<RadiantArrow>());
            AddAmmo(ItemID.MoonlordArrow);
            //Bladed Arrow
            //AddAmmo(ModContent.ItemType<Blaa>)
            AddAmmo(ItemID.MoonlordBullet);
            AddAmmo(ItemID.ChlorophyteBullet);
        }
    }
}
