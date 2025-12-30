using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.Bases;
using Stellamod.Core.SwingSystem;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ID;
using Terraria.ModLoader;


namespace Stellamod.Common.ItemBrowser
{
    public class ItemInternalNameComparer : IComparer<Item>
    {
        public int Compare(Item x, Item y)
        {

            return x.Name.CompareTo(y.Name);
        }
    }

    public class ItemTypeComparer : IComparer<Item>
    {
        public int Compare(Item x, Item y)
        {

            return x.type.CompareTo(y.type);
        }
    }
    public class ItemPathComparer : IComparer<Item>
    {
        public int Compare(Item x, Item y)
        {

            return x.ModItem.GetType().AssemblyQualifiedName.CompareTo(y.ModItem.GetType().AssemblyQualifiedName);
        }
    }

    [Autoload(Side = ModSide.Client)]
    public class ItemCategoryUtility : ModSystem
    {
        public override void PostAddRecipes()
        {
            base.PostAddRecipes();
            InitializeCategories();
        }

        public static Category All;
        public static void InitializeCategories()
        {
            //Melee subclasses
            int numMeleeWeaponTypes = Enum.GetNames(typeof(MeleeWeaponType)).Length;
            Category[] subMelee = new Category[numMeleeWeaponTypes];
            for (int i = 0; i < numMeleeWeaponTypes; i++)
            {
                MeleeWeaponType weaponType = (MeleeWeaponType)i;
                Category meleeSubCategory = new Category(weaponType.ToString(), (x) =>
                {
                    BaseSwingItemV2 baseSwingItemV2 = x.ModItem as BaseSwingItemV2;
                    if (baseSwingItemV2 != null && baseSwingItemV2.meleeWeaponType == weaponType)
                        return true;
                    return false;
                });
                subMelee[i] = meleeSubCategory;
            }
            Category melee = new Category("Melee", (x) =>
            {
                return x.DamageType == DamageClass.Melee;
            }, subMelee);


            Category ranged = new Category("Ranged", (x) =>
            {
                return x.DamageType == DamageClass.Ranged;
            });
            Category mage = new Category("Mage", (x) =>
            {
                return x.DamageType == DamageClass.Magic;
            });
            Category summon = new Category("Summon", (x) =>
            {
                return x.DamageType == DamageClass.Summon;
            });
            Category whip = new Category("Whips", (x) =>
            {
                return x.DamageType == DamageClass.SummonMeleeSpeed;
            });
            Category sentries = new Category("Sentries", (x) =>
            {
                return x.sentry;
            });

            Category weapons = new Category("Weapons", (x) =>
            {
                return x.damage != -1;
            }, melee, ranged, mage, summon, whip, sentries);

            //Armors
            Category legs = new Category("Legs", (x) =>
            {
                return x.legSlot != -1;
            });
            Category bodies = new Category("Bodies", (x) =>
            {
                return x.bodySlot != -1;
            });
            Category helms = new Category("Helms", (x) =>
            {
                return x.headSlot != -1;
            });
            Category armors = new Category("Armors", (x) =>
            {
                return x.legSlot != -1 || x.bodySlot != -1 || x.headSlot != -1;
            }, legs, bodies, helms);

            Category accessories = new Category("Accessories", (x) =>
            {
                return x.accessory;
            });

            Category mounts = new Category("Mounts", (x) =>
            {
                return x.mountType != -1;
            });

            Category pets = new Category("Pets", (x) =>
            {
                return ProjectileID.Sets.LightPet[x.shoot] || Main.projPet[x.shoot];
            });

            Category insources = new Category("Insources", (x) =>
            {
                return x.ModItem is InsourceItem;
            });

            Category tiles = new Category("Tiles", (x) =>
            {
                return x.createTile != -1;
            });
            Category walls = new Category("Walls", (x) =>
            {
                return x.createWall != -1;
            });
            Category dyes = new Category("Dyes", (x) =>
            {
                return x.dye != 0;
            });
            Category paints = new Category("Paints", (x) =>
            {
                return x.paint != 0;
            });
            Category expert = new Category("Expert", (x) =>
            {
                return x.expert;
            });
            Category master = new Category("Master", (x) =>
            {
                return x.master;
            });
            Category consumables = new Category("Consumables", (x) =>
            {
                return x.consumable;
            });

            Cauldron c = ModContent.GetInstance<Cauldron>();
            Item[] cauldronMaterials = c.GetMaterials();
            Category[] cauldronSub = new Category[cauldronMaterials.Length];
            for (int m = 0; m < cauldronMaterials.Length; m++)
            {
                Item material = cauldronMaterials[m];
                Category cauldronCategory = new Category(material.ModItem.DisplayName.Value, (x) =>
                {
                    return c.FindMaterial(x).type == material.type;
                });
                cauldronSub[m] = cauldronCategory;
            }
            Category cauldron = new Category("Cauldron", (x) =>
            {
                return !c.FindMaterial(x).IsAir;
            }, cauldronSub);


            List<Category> topCategories = new List<Category>()
                    {
                        weapons,
                        armors,
                        accessories,
                        mounts,
                        pets,
                        insources,
                        tiles,
                        walls,
                        dyes,
                        paints,
                        expert,
                        master,
                        consumables,
                        cauldron

                    };
            Category mainCategory = new Category("All", (x) => true, topCategories.ToArray());
            All = mainCategory;
        }
    }
    /// <summary>
    /// Handles get items in a specific tab
    /// </summary>
    public static class ItemBrowserUtility
    {
        public delegate bool CompareFunction(Item item);
        private static void ItemSearchInnerLoop(Category category, Item item, List<Item> output)
        {
            if (category == null)
            {
                output.Add(item);
                return;
            }

            Category current = category;
            bool fail = false;
            if (!current.compareFunction(item))
            {
                fail = true;
            }

            //Walk backwards through all the categories and see if they meet the criteria
            while (current.parentCategory != null && !fail)
            {
                current = current.parentCategory;
                if (!current.compareFunction(item))
                {
                    fail = true;
                }
            }
            if (!fail)
            {
                output.Add(item);
            }
        }
        public static void PopulateItems(Category category)
        {
            List<Item> result = new List<Item>();
 

            for(int i = 0; i < ItemLoader.ItemCount; i++)
            {
                Item item = new Item(i);
                ItemSearchInnerLoop(category, item, result);
            }

            category.items = result.ToArray();
        }
    }
}
