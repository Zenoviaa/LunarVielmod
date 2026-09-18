using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Graphics.PackedVector;
using ReLogic.Content;
using ReLogic.Graphics;
using Stellamod.Buffs;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core;
using Stellamod.Core.Tooltips;
using Stellamod.Core.Utilities;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using Terraria;
using Terraria.Audio;
using Terraria.DataStructures;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;
using Terraria.UI;

namespace Stellamod.Common.ArmorRework
{
    public struct ArmorSet
    {
        public int order;
        public ArmorGroup act;
        public int helm;
        public int armor;
        public int legs;
    }
    public enum ArmorGroup
    {
        Act_I,
        Act_II,
        Act_III
    }

    [Autoload(Side = ModSide.Client)]
    public class DiscoverArmorSystem : ModSystem
    {
        public override void Load()
        {
            base.Load();
            On_Player.OpenInventory += CheckForNewArmors;
            On_Player.GetItem_FillEmptyInventorySlot += DiscoverArmor;
            On_Player.GetItem_FillEmptyInventorySlot_VoidBag += DiscoverArmor;
        }

        private void CheckForNewArmors(On_Player.orig_OpenInventory orig)
        {
            DiscoveredArmorsPlayer armorsPlayer = Main.LocalPlayer.GetModPlayer<DiscoveredArmorsPlayer>();
            foreach(var item in Main.LocalPlayer.inventory)
            {
                armorsPlayer.TryDiscover(item);
            }
            foreach (var item in Main.LocalPlayer.armor)
            {
                armorsPlayer.TryDiscover(item);
            }

            orig();
        }

        private bool DiscoverArmor(On_Player.orig_GetItem_FillEmptyInventorySlot_VoidBag orig, Player self, int plr, Item[] inv, Item newItem, GetItemSettings settings, Item returnItem, int i)
        {
            DiscoveredArmorsPlayer armorsPlayer = self.GetModPlayer<DiscoveredArmorsPlayer>();
            armorsPlayer.TryDiscover(newItem);
            return orig(self, plr, inv, newItem, settings, returnItem, i);
        }

        private bool DiscoverArmor(On_Player.orig_GetItem_FillEmptyInventorySlot orig, Player self, int plr, Item newItem, GetItemSettings settings, Item returnItem, int i)
        {
            DiscoveredArmorsPlayer armorsPlayer = self.GetModPlayer<DiscoveredArmorsPlayer>();
            armorsPlayer.TryDiscover(newItem);
            return orig(self, plr, newItem, settings, returnItem, i);
        }
    }
    public class DiscoveredArmorsPlayer : ModPlayer
    {
        private List<Item> _discoveredArmorsBackingField;
        private List<Item> DiscoveredArmors
        {
            get
            {
                _discoveredArmorsBackingField ??= new List<Item>();
                return _discoveredArmorsBackingField;
            }
            set
            {
                _discoveredArmorsBackingField = value;
            }
        }


        public bool IsAnyDiscovered(params int[] itemType)
        {
            for (int i = 0; i < itemType.Length; i++)
            {
                int type = itemType[i];
                bool isDiscovered = DiscoveredArmors.Find(x => x.type == type) != null;
                if (isDiscovered)
                    return true;
            }
            return false;
        }

        public bool IsDiscovered(Item item)
        {
            return IsAnyDiscovered(item.type);
        }

        public void TryDiscover(Item item)
        {
            if (item.bodySlot != -1 || item.headSlot != -1 || item.legSlot != -1)
            {
                DiscoveredArmors.Add(item);
                DiscoveredArmors = DiscoveredArmors.DistinctBy(x => x.type).ToList();
            }
        }

        public int CountDiscoveredArmors()
        {
            HashSet<int> armorTypes = new HashSet<int>();
            foreach(var item in DiscoveredArmors)
            {
                if (item.headSlot == -1 && item.bodySlot == -1 && item.legSlot == -1)
                    continue;

                int helmetType = ArmorSetSystem.GetHelmet(item.type);
                if (armorTypes.Contains(helmetType))
                    continue;

                armorTypes.Add(helmetType);
            }
            return armorTypes.Count;
        }

        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["discoveredArmors"] = DiscoveredArmors;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            DiscoveredArmors = tag.Get<List<Item>>("discoveredArmors");
            DiscoveredArmors ??= new List<Item>();
        }
    }

    public class ArmorSetSystem : ModSystem
    {
        private static List<ArmorSet> _armorSets;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
       
        }
        public override void Load()
        {
            base.Load();
            _armorSets = new List<ArmorSet>();
        }

        public override void Unload()
        {
            base.Unload();
            _armorSets = null;
        }
        public static int GetHelmet(int itemType)
        {
            foreach (var armorSet in _armorSets)
            {
                if(armorSet.helm == itemType || armorSet.armor == itemType || armorSet.legs == itemType)
                {
                    return armorSet.helm;
                }
            }

            return 0;
        }
        public static bool IsArmorSet(int itemType)
        {
            foreach(var armorSet in _armorSets)
            {
                if (armorSet.helm == itemType)
                    return true;
                if (armorSet.armor == itemType)
                    return true;
                if (armorSet.legs == itemType)
                    return true;
            }
            return false;
        }

        public static ArmorSet[] GetArmorSets(params int[] types)
        {
            List<ArmorSet> mySets = new List<ArmorSet>();
     
            foreach (var armorSet in _armorSets)
            {
                for (int i = 0; i < types.Length; i++)
                {
                    int itemType = types[i];
                    if (armorSet.helm == itemType || armorSet.armor == itemType || armorSet.legs == itemType)
                    {
                        mySets.Add(armorSet);
                    }
                }
    
            }
            return mySets.DistinctBy(x => x.helm).ToArray();
        }

        public static ArmorSet[] GetArmorSets() => _armorSets.ToArray();
        public static ArmorSet FindArmorSet(int type)
        {
            return _armorSets.Find(x => x.helm == type || x.armor == type || x.legs == type);
        }

        public static ArmorSet FindArmorSet(Item item)
        {
            return FindArmorSet(item.type);
        }

        public static void GetArmorSet(ArmorSet armorSet, out Item helm, out Item armor, out Item leggings)
        {
            helm = new Item(armorSet.helm);
            armor = new Item(armorSet.armor);
            leggings = new Item(armorSet.legs);
        }
        public static void RegisterArmorSet<Helm, Armor, Legs>(ArmorGroup armorGroup)
            where Helm : ModItem
            where Armor : ModItem
            where Legs : ModItem
        {
            RegisterArmorSet(
                ModContent.ItemType<Helm>(), 
                ModContent.ItemType<Armor>(), 
                ModContent.ItemType<Legs>(), armorGroup);
        }
        public static void RegisterArmorSet<Helm, Armor>(ArmorGroup armorGroup)
            where Helm : ModItem
            where Armor : ModItem
        {
            RegisterArmorSet(ModContent.ItemType<Helm>(), ModContent.ItemType<Armor>(), 0, armorGroup);
        }

        public static void RegisterArmorSet(int helm, int armor, int legs, ArmorGroup armorGroup)
        {
            ArmorSet set = new ArmorSet
            {
                helm = helm,
                armor = armor,
                legs = legs,
                act = armorGroup
            };
            _armorSets.Add(set);
        }
    }

    public class ArmorReworkExpandableTooltip : AbstractExpandingTooltip
    {
        public override void ModifyExpandableTooltips(Item item, List<TooltipLine> lines)
        {
            //            throw new NotImplementedException();
            //Here we want to get the stats for the entire armor set
            if (item.vanity)
                return;

            ArmorStatsPlayer armorStatsPlayer = Main.LocalPlayer.GetModPlayer<ArmorStatsPlayer>();
            armorStatsPlayer.GetStatTooltipsLocalToItem(item, lines);
        }
    }
    public class ArmorReworkGlobalItem : GlobalItem
    {
        public override bool InstancePerEntity => true;
        public override void ModifyTooltips(Item item, List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(item, tooltips);
        }
    }

    public class ExtraAccessorySlot1 : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<ArmorStatsPlayer>().accessorySlots >= 6;
        }
    }

    public class ExtraAccessorySlot2 : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<ArmorStatsPlayer>().accessorySlots >= 7;
        }
    }

    public class ExtraAccessorySlot3 : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<ArmorStatsPlayer>().accessorySlots >= 8;
        }
    }

    public class ExtraAccessorySlot4 : ModAccessorySlot
    {
        public override bool IsEnabled()
        {
            return Player.GetModPlayer<ArmorStatsPlayer>().accessorySlots >= 9;
        }
    }

    public class ArmorAccessoryRework : ModSystem
    {
        public override void OnModLoad()
        {
            base.OnModLoad();

            On_Player.IsItemSlotUnlockedAndUsable += LimitAccessorySlots;
        }
        public override void OnModUnload()
        {
            base.OnModUnload();
            On_Player.IsItemSlotUnlockedAndUsable -= LimitAccessorySlots;
        }


        private bool LimitAccessorySlots(On_Player.orig_IsItemSlotUnlockedAndUsable orig, Player self, int slot)
        {
            int start = 3;
            int end = 9;
            if (Main.masterMode)
                end -= 1;
            if (slot >= start && slot <= end)
            {
                int accessoryNumber = slot - start;
                ArmorStatsPlayer armorStatsPlayer = self.GetModPlayer<ArmorStatsPlayer>();
                if (armorStatsPlayer.accessorySlotsLastFrame > accessoryNumber)
                    return true;
                else
                    return false;
            }

            return orig(self, slot);
        }
    }

    public class ExtraPierceGlobalProjectile : GlobalProjectile
    {

        //I believe net update is called after on spawn automatically
        //and penetrate will be synced
        //so this should work
        //TODO: verify that's real
        public override void OnSpawn(Projectile projectile, IEntitySource source)
        {
            base.OnSpawn(projectile, source);
            Player player = Main.player[projectile.owner];
            ArmorStatsPlayer armorStatsPlayer = player.GetModPlayer<ArmorStatsPlayer>();
            if (projectile.penetrate != -1)
            {
                projectile.penetrate += armorStatsPlayer.rangedPiercing;
                projectile.maxPenetrate += armorStatsPlayer.rangedPiercing;
                if (armorStatsPlayer.rangedPiercing > 0)
                {
                    projectile.usesLocalNPCImmunity = true;
                    projectile.localNPCHitCooldown = -1;
                }
            }
        }
    }

    public static class ArmorStatsExtensions
    {
        public static ArmorStatsPlayer GetStats(this Player player) => player.GetModPlayer<ArmorStatsPlayer>();
    }

    public class ManaReworkSystem : ModSystem
    {
        public override void Load()
        {
            base.Load();
            On_Player.UpdateManaRegen += ManaRework;
        }


        public override void Unload()
        {
            base.Unload();
            On_Player.UpdateManaRegen -= ManaRework;
        }
        private void ManaRework(On_Player.orig_UpdateManaRegen orig, Player self)
        {
            if (self.nebulaLevelMana > 0)
            {
                int num = 6;
                self.nebulaManaCounter += self.nebulaLevelMana;
                if (self.nebulaManaCounter >= num)
                {
                    self.nebulaManaCounter -= num;
                    self.statMana++;
                    if (self.statMana >= self.statManaMax2)
                        self.statMana = self.statManaMax2;
                }
            }
            else
            {
                self.nebulaManaCounter = 0;
            }

            if (self.manaRegenDelay > 0f)
            {
                self.manaRegenDelay -= 1f;
                self.manaRegenDelay -= self.manaRegenDelayBonus;
            }
     
            /*
            if (manaRegenBuff && manaRegenDelay > 20f)
                manaRegenDelay = 20f;
            */
            if (self.manaRegenDelay <= 0f)
            {
                self.manaRegenDelay = 0f;
                self.manaRegen = 30 + self.manaRegenBonus;
                /*
                self.manaRegen = statManaMax2 / 3 + 1 + manaRegenBonus;

                if (IsStandingStillForSpecialEffects || grappling[0] >= 0 || manaRegenBuff)
                  manaRegen += statManaMax2 / 3;

                if (usedArcaneCrystal)
                    manaRegen += statManaMax2 / 50;
           
                float num2 = (float)statMana / (float)statManaMax2 * 0.8f + 0.2f;
             
                if (manaRegenBuff)
                    num2 = 1f;
           
                self.manaRegen = (int)((double)((float)self.manaRegen * num2) * 1.15);*/
            }
            else
            {
                self.manaRegen = 0;
            }

            self.manaRegenCount += self.manaRegen;
            while (self.manaRegenCount >= 120)
            {
                bool flag = false;
                self.manaRegenCount -= 120;
                if (self.statMana < self.statManaMax2)
                {
                    self.statMana++;
                    flag = true;
                }

                if (self.statMana < self.statManaMax2)
                    continue;

                if (self.whoAmI == Main.myPlayer && flag)
                {
                    SoundEngine.PlaySound(SoundID.MaxMana);
                    for (int i = 0; i < 5; i++)
                    {
                        int num3 = Dust.NewDust(self.position, self.width, self.height, DustID.ManaRegeneration, 0f, 0f, 255, default(Color), (float)Main.rand.Next(20, 26) * 0.1f);
                        Main.dust[num3].noLight = true;
                        Main.dust[num3].noGravity = true;
                        Main.dust[num3].velocity *= 0.5f;
                    }
                }

                self.statMana = self.statManaMax2;
            }
        }

    }
    public class ArmorStatsPlayer : ModPlayer
    {
        //Textures
        private Dictionary<string, Asset<Texture2D>> _iconAssets;
        private Player _localDummyPlayer;
        private Player _currentDummyPlayer;
        public float generalEndurance;
        public float bossEndurance;
        public float enemyEndurance;
        public int defenseBonus;
        public int healthBonus;

        public float criticalStrikeChance;
        public float criticalStrikeDamage;

        public int stamina;
        public int accessorySlots;
        public int accessorySlotsLastFrame;
        public int insourceSlots;
        public int inventorySlots;
        public float insourceTimeFlatBonus;
        public float insourceTimeBonus;
        public float movementSpeedBonus;

        public float meleeAttackSpeed;
        public float meleeDamage;
        public int meleeArmorPenetration;
        public int meleeAggressiveness;

        public float rangedBowChargeTime;
        public float rangedDamage;
        public int rangedPiercing;
        public int rangedGunAmmoAmount;
        public float rangedGunAmmoAmountPct;
        public int rangedStealthtiness;

        public float summonCastTime;
        public float summonDamage;
        public int minionSlots;
        public float mainSummonDamage;
        public float mainSummonHealth;
        public float minionSummonHealth;
        public int minionAggressiveness;

        public float artifactManaReduction;
        public float wandCastTime;
        public int totalMana;
        public float magicDamage;
        public int wandNormalEnchantmentSlots;
        public int wandTimerEnchantmentSlots;

        public bool isComparison;


        public override void Unload()
        {
            base.Unload();
            _iconAssets = null;
          
        }


        public Asset<Texture2D> RequestIconTexture(string name)
        {
            _iconAssets ??= new Dictionary<string, Asset<Texture2D>>();
            if (_iconAssets.ContainsKey(name))
                return _iconAssets[name];

            string path = this.GetType().DirectoryHere() + $"/{name}";
            bool exists = ModContent.RequestIfExists<Texture2D>(path, out Asset<Texture2D> asset);
            if (exists)
            {
                _iconAssets.Add(name, asset);
            }
            else
            {
                asset = ModContent.Request<Texture2D>(TextureRegistry.EmptyTexture);
            }

            return asset;
        }

        public override void ResetEffects()
        {
            base.ResetEffects();
            //Defensive Stats
            generalEndurance = 0;
            bossEndurance = 0;
            enemyEndurance = 0;
            defenseBonus = 0;
            healthBonus = 0;

            //Critical Strike Stats
            criticalStrikeChance = 0;
            criticalStrikeDamage = 0;

            //Resource Stats
            stamina = 0;
            accessorySlots = 0;
            insourceSlots = 0;
            insourceTimeBonus = 0;
            movementSpeedBonus = 0;
            inventorySlots = 0;

            //Melee Damage
            meleeAttackSpeed = 0;
            meleeDamage = 0;
            meleeArmorPenetration = 0;
            meleeAggressiveness = 0;

            //Ranged stats
            rangedBowChargeTime = 0;
            rangedDamage = 0;
            rangedPiercing = 0;
            rangedGunAmmoAmount = 0;
            rangedStealthtiness = 0;
            rangedGunAmmoAmountPct = 0;

            //Summoner Stats
            summonCastTime = 0;
            summonDamage = 0;
            minionSlots = 0;
            mainSummonDamage = 0;
            mainSummonHealth = 0;
            minionSummonHealth = 0;
            minionAggressiveness = 0;

            //Magic Stats
            artifactManaReduction = 0;
            wandCastTime = 0;
            totalMana = 0;
            magicDamage = 0;
            wandNormalEnchantmentSlots = 0;
            wandTimerEnchantmentSlots = 0;
        }

        private string GetComparison(string name, float comparedValue, bool invert = false, float localStatValue = 0f, bool isShowingComparison = false)
        {
            if (comparedValue == 0)
                return string.Empty;
            string percentString = MathF.Abs(localStatValue).ToString("P0");
            string increaseDecreaseKey = localStatValue < 0 ? "StatSubtraction" : "StatAddition";
            if (invert)
            {
                increaseDecreaseKey = localStatValue > 0 ? "StatSubtraction" : "StatAddition";
            }
            string comparisonText = LangText.Common(increaseDecreaseKey, LangText.Common($"Stat{name}"), percentString);
            if(isShowingComparison)
            {
                string symbol = MathF.Sign(comparedValue) >= 0f ? "+" : "-";
             
                comparisonText += $" ({symbol}{MathF.Abs(comparedValue)})";
            }
            return comparisonText;
        }

        private string GetComparison(string name, int comparedValue, int localStatValue = 0, bool isShowingComparison = false)
        {
            if (comparedValue == 0)
                return string.Empty;
            string percentString = MathF.Abs(localStatValue).ToString();
            string increaseDecreaseKey = localStatValue < 0 ? "StatSubtractionAlt" : "StatAdditionAlt";
            string comparisonText = LangText.Common(increaseDecreaseKey, LangText.Common($"Stat{name}"), percentString);
            if (isShowingComparison)
            {
                string symbol = MathF.Sign(comparedValue) >= 0f ? "+" : "-";

                comparisonText += $" ({symbol}{MathF.Abs(comparedValue)})";
            }
            return comparisonText;
        }

        private void ApplyArmor(Item item, Player player)
        {
        
            /*
            if (item.accessory)
            {
                ItemLoader.UpdateAccessory(item, player, false);
            }*/
            if (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1)
            {
                ItemLoader.UpdateEquip(item, player);
            }
        }


        public void GetStatTooltipsLocalToItem(Item item, List<TooltipLine> tooltips)
        {
            if (item.IsAir || item == null)
                return;

            _localDummyPlayer ??= new Player();
            _localDummyPlayer.ResetEffects();

            _currentDummyPlayer ??= new Player();
            _currentDummyPlayer.ResetEffects();

            Player player = Main.LocalPlayer;
            Item helmer = player.armor[0];
            Item armor = player.armor[1];
            Item legs = player.armor[2];

            bool isShowingComparison = false;
            //Apply all of our stat bonuses here
            if (!helmer.IsAir && item.headSlot != -1 && item.type != helmer.type)
            {
                isShowingComparison = true;
                ApplyArmor(helmer, _currentDummyPlayer);
            } else if (!armor.IsAir && item.bodySlot != -1 && item.type != armor.type)
            {
                isShowingComparison = true;
                ApplyArmor(armor, _currentDummyPlayer);
            } else if (!legs.IsAir && item.legSlot != -1 && item.type != legs.type)
            {
                isShowingComparison = true;
                ApplyArmor(legs, _currentDummyPlayer);
            }

            ApplyArmor(item, _localDummyPlayer);

            //Compare the differences here
            ArmorStatsPlayer currentStatsPlayer = _currentDummyPlayer.GetModPlayer<ArmorStatsPlayer>();
            ArmorStatsPlayer localItemStatsPlayer = _localDummyPlayer.GetModPlayer<ArmorStatsPlayer>();


            ArmorStatsPlayer comparisonPlayer = currentStatsPlayer.CompareArmorStatsPlayer(localItemStatsPlayer);
            comparisonPlayer.GetStatTooltips(localItemStatsPlayer, tooltips, isShowingComparison);
        }

        public void GetStatTooltips(ArmorStatsPlayer originalStatsPlayer, List<TooltipLine> tooltips, bool isShowingComparison = false)
        {
            void AddLineIfDifferent(string name, float comparisonValue, float currentValue, bool invert = false)
            {
                string comparison = GetComparison(name, comparisonValue, invert, currentValue, isShowingComparison);
                if (string.IsNullOrEmpty(comparison))
                    return;
                TooltipLine line = new TooltipLine(Stellamod.Instance, name, comparison);
                if (comparisonValue < 0)
                    line.OverrideColor = Color.IndianRed;
                if (isShowingComparison && comparisonValue > 0)
                    line.OverrideColor = Color.LightGreen;
                tooltips.Add(line);
            }
            void AddLineIfDifferentInt(string name, int comparisonValue, int currentValue)
            {
                string comparison = GetComparison(name, comparisonValue, currentValue, isShowingComparison);
                if (string.IsNullOrEmpty(comparison))
                    return;
                TooltipLine line = new TooltipLine(Stellamod.Instance, name, comparison);
                if (comparisonValue < 0)
                    line.OverrideColor = Color.IndianRed;
                if (isShowingComparison && comparisonValue > 0)
                    line.OverrideColor = Color.LightGreen;
                tooltips.Add(line);
            }

            //damage goes here
            AddLineIfDifferent("MeleeDamage", meleeDamage, originalStatsPlayer.meleeDamage);
            AddLineIfDifferent("RangedDamage", rangedDamage, originalStatsPlayer.rangedDamage);
            AddLineIfDifferent("MagicDamage", magicDamage, originalStatsPlayer.magicDamage);
            AddLineIfDifferent("MinionDamage", summonDamage, originalStatsPlayer.summonDamage);
            AddLineIfDifferentInt("MaxHealth", healthBonus, originalStatsPlayer.healthBonus);
            AddLineIfDifferent("CriticalStrikeChance", criticalStrikeChance, originalStatsPlayer.criticalStrikeChance);
            AddLineIfDifferent("CriticalStrikeDamage", criticalStrikeDamage, originalStatsPlayer.criticalStrikeDamage);
            AddLineIfDifferentInt("Stamina", stamina, originalStatsPlayer.stamina);
            AddLineIfDifferentInt("ArmorPenetration", meleeArmorPenetration, originalStatsPlayer.meleeArmorPenetration);
            AddLineIfDifferentInt("AccessorySlots", accessorySlots, originalStatsPlayer.accessorySlots);
            AddLineIfDifferentInt("InventorySlots", inventorySlots, originalStatsPlayer.inventorySlots);
            AddLineIfDifferent("MovementSpeed", movementSpeedBonus, originalStatsPlayer.movementSpeedBonus);
            AddLineIfDifferent("Endurance", generalEndurance, originalStatsPlayer.generalEndurance);
            AddLineIfDifferent("BossEndurance", bossEndurance, originalStatsPlayer.bossEndurance);
            AddLineIfDifferent("EnemyEndurance", enemyEndurance, originalStatsPlayer.enemyEndurance);
            AddLineIfDifferentInt("InsourceSlots", insourceSlots, originalStatsPlayer.insourceSlots);
            AddLineIfDifferent("MeleeAttackSpeed", meleeAttackSpeed, originalStatsPlayer.meleeAttackSpeed);
            AddLineIfDifferentInt("Defense", defenseBonus, originalStatsPlayer.defenseBonus);
            AddLineIfDifferent("InsourceTime", insourceTimeBonus, originalStatsPlayer.insourceTimeBonus);
            AddLineIfDifferent("Aggressiveness", meleeAggressiveness, originalStatsPlayer.meleeAggressiveness);
            AddLineIfDifferent("BowChargeTime", rangedBowChargeTime, originalStatsPlayer.rangedBowChargeTime, invert: true);
            AddLineIfDifferentInt("Piercing", rangedPiercing, originalStatsPlayer.rangedPiercing);
            AddLineIfDifferentInt("GunAmmoAmount", rangedGunAmmoAmount, originalStatsPlayer.rangedGunAmmoAmount);
            AddLineIfDifferent("GunAmmoAmount", rangedGunAmmoAmountPct, originalStatsPlayer.rangedGunAmmoAmountPct);
            AddLineIfDifferentInt("Stealthiness", rangedStealthtiness, originalStatsPlayer.rangedStealthtiness);
            AddLineIfDifferent("SummonCastTime", summonCastTime, originalStatsPlayer.summonCastTime, invert: true);
            AddLineIfDifferentInt("MinionSlots", minionSlots, originalStatsPlayer.minionSlots);
            AddLineIfDifferent("MainMinionDamage", mainSummonDamage, originalStatsPlayer.mainSummonDamage);
            AddLineIfDifferent("MainMinionHealth", mainSummonHealth, originalStatsPlayer.mainSummonHealth);
            AddLineIfDifferent("MinionHealth", minionSummonHealth, originalStatsPlayer.minionSummonHealth);
            AddLineIfDifferentInt("MinionAggressiveness", minionAggressiveness, originalStatsPlayer.minionAggressiveness);
            AddLineIfDifferent("ArtifactManaReduction", artifactManaReduction, originalStatsPlayer.artifactManaReduction);
            AddLineIfDifferent("WandCastTime", wandCastTime, originalStatsPlayer.wandCastTime, invert: true);
            AddLineIfDifferentInt("MaxMana", totalMana, originalStatsPlayer.totalMana);
            AddLineIfDifferentInt("WandNormalEnchantmentSlots", wandNormalEnchantmentSlots, originalStatsPlayer.wandNormalEnchantmentSlots);
            AddLineIfDifferentInt("WandTimerEnchantmentSlots", wandTimerEnchantmentSlots, originalStatsPlayer.wandTimerEnchantmentSlots);
        }


        public ArmorStatsPlayer CompareArmorStatsPlayer(ArmorStatsPlayer otherPlayer)
        {
            ArmorStatsPlayer armorStatsPlayer = new ArmorStatsPlayer();
            armorStatsPlayer.generalEndurance = otherPlayer.generalEndurance - generalEndurance;
            armorStatsPlayer.bossEndurance = otherPlayer.bossEndurance - bossEndurance;
            armorStatsPlayer.enemyEndurance = otherPlayer.enemyEndurance - enemyEndurance;
            armorStatsPlayer.defenseBonus = otherPlayer.defenseBonus - defenseBonus;
            armorStatsPlayer.healthBonus = otherPlayer.healthBonus - healthBonus;

            armorStatsPlayer.criticalStrikeChance = otherPlayer.criticalStrikeChance - criticalStrikeChance;
            armorStatsPlayer.criticalStrikeDamage = otherPlayer.criticalStrikeDamage - criticalStrikeDamage;

            armorStatsPlayer.stamina = otherPlayer.stamina - stamina;
            armorStatsPlayer.accessorySlots = otherPlayer.accessorySlots - accessorySlots;
            armorStatsPlayer.insourceSlots = otherPlayer.insourceSlots - insourceSlots;
            armorStatsPlayer.inventorySlots = otherPlayer.inventorySlots - inventorySlots;
            armorStatsPlayer.insourceTimeBonus = otherPlayer.insourceTimeBonus - insourceTimeBonus;
            armorStatsPlayer.movementSpeedBonus = otherPlayer.movementSpeedBonus - movementSpeedBonus;

            armorStatsPlayer.meleeAttackSpeed = otherPlayer.meleeAttackSpeed - meleeAttackSpeed;
            armorStatsPlayer.meleeDamage = otherPlayer.meleeDamage - meleeDamage;
            armorStatsPlayer.meleeArmorPenetration = otherPlayer.meleeArmorPenetration - meleeArmorPenetration;
            armorStatsPlayer.meleeAggressiveness = otherPlayer.meleeAggressiveness - meleeAggressiveness;

            armorStatsPlayer.rangedBowChargeTime = otherPlayer.rangedBowChargeTime - rangedBowChargeTime;
            armorStatsPlayer.rangedDamage = otherPlayer.rangedDamage - rangedDamage;
            armorStatsPlayer.rangedPiercing = otherPlayer.rangedPiercing - rangedPiercing;
            armorStatsPlayer.rangedGunAmmoAmount = otherPlayer.rangedGunAmmoAmount - rangedGunAmmoAmount;
            armorStatsPlayer.rangedStealthtiness = otherPlayer.rangedStealthtiness - rangedStealthtiness;
            armorStatsPlayer.rangedGunAmmoAmountPct = otherPlayer.rangedGunAmmoAmountPct - rangedGunAmmoAmountPct;

            armorStatsPlayer.summonCastTime = otherPlayer.summonCastTime - summonCastTime;
            armorStatsPlayer.summonDamage = otherPlayer.summonDamage - summonDamage;
            armorStatsPlayer.minionSlots = otherPlayer.minionSlots - minionSlots;
            armorStatsPlayer.mainSummonDamage = otherPlayer.mainSummonDamage - mainSummonDamage;
            armorStatsPlayer.mainSummonHealth = otherPlayer.mainSummonHealth - mainSummonHealth;
            armorStatsPlayer.minionAggressiveness = otherPlayer.minionAggressiveness - minionAggressiveness;

            armorStatsPlayer.artifactManaReduction = otherPlayer.artifactManaReduction - artifactManaReduction;
            armorStatsPlayer.wandCastTime = otherPlayer.wandCastTime - wandCastTime;
            armorStatsPlayer.totalMana = otherPlayer.totalMana - totalMana;
            armorStatsPlayer.magicDamage = otherPlayer.magicDamage - magicDamage;
            armorStatsPlayer.wandNormalEnchantmentSlots = otherPlayer.wandNormalEnchantmentSlots - wandNormalEnchantmentSlots;
            armorStatsPlayer.wandTimerEnchantmentSlots = otherPlayer.wandTimerEnchantmentSlots - wandTimerEnchantmentSlots;

            armorStatsPlayer.isComparison = true;
            return armorStatsPlayer;
        }

        public override void ModifyWeaponCrit(Item item, ref float crit)
        {
            base.ModifyWeaponCrit(item, ref crit);
            crit += criticalStrikeChance * 100;
        }

        public override void ModifyHitNPC(NPC target, ref NPC.HitModifiers modifiers)
        {
            base.ModifyHitNPC(target, ref modifiers);
            modifiers.CritDamage += criticalStrikeDamage;
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();
            Player.statLifeMax2 += healthBonus;
            Player.statDefense += defenseBonus;
            Player.moveSpeed += movementSpeedBonus;
            Player.GetAttackSpeed(DamageClass.Melee) += meleeAttackSpeed;
            if (NPC.AnyDanger())
            {
                Player.endurance += bossEndurance;
            }
            else
            {
                Player.endurance += enemyEndurance;
            }
            Player.endurance += generalEndurance;


            Player.GetDamage(DamageClass.Melee) += meleeDamage;
            Player.GetArmorPenetration(DamageClass.Generic) += meleeArmorPenetration;
            AggroSystem aggroSystem = ModContent.GetInstance<AggroSystem>();
            aggroSystem.aggro[Player.whoAmI] += meleeAggressiveness;

            Player.GetDamage(DamageClass.Ranged) += rangedDamage;
            Player.GetDamage(DamageClass.Magic) += magicDamage;
            Player.statManaMax2 += totalMana;
            Player.GetModPlayer<DashPlayer>().MaxDashCount += stamina;
            Player.GetModPlayer<FlaskPlayer>().maxInsourceCount += insourceSlots;
            Player.maxMinions += minionSlots;
            Player.aggro += meleeAggressiveness;
            Player.aggro -= rangedStealthtiness;
            accessorySlotsLastFrame = accessorySlots;
        }
    }
}
