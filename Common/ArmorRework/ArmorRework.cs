using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.Core.Tooltips;
using Stellamod.Helpers;
using Stellamod.Items.Accessories.Players;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.ArmorRework
{
    public struct ArmorSet
    {
        public int helm;
        public int armor;
        public int legs;
    }

    public class ArmorSetSystem : ModSystem
    {
        private static List<ArmorSet> _armorSets;
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            _armorSets = new List<ArmorSet>();
        }

        public override void Unload()
        {
            base.Unload();
            _armorSets = null;
        }

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
        public static void RegisterArmorSet<Helm, Armor, Legs>()
            where Helm : ModItem
            where Armor : ModItem
            where Legs : ModItem
        {
            RegisterArmorSet(ModContent.ItemType<Helm>(), ModContent.ItemType<Armor>(), ModContent.ItemType<Legs>());
        }

        public static void RegisterArmorSet(int helm, int armor, int legs)
        {
            ArmorSet set = new ArmorSet
            {
                helm = helm,
                armor = armor,
                legs = legs
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
            if (slot >= start && slot <= end)
            {
                int accessoryNumber = slot - start;
                ArmorStatsPlayer armorStatsPlayer = self.GetModPlayer<ArmorStatsPlayer>();
                if (armorStatsPlayer.accessorySlots > accessoryNumber)
                    return true;
                else
                    return false;
            }

            return orig(self, slot);
        }
    }

 
    public class ArmorStatsPlayer : ModPlayer
    {        
        //Textures
        private Dictionary<string, Asset<Texture2D>> _iconAssets;
        private Player _dummyPlayer;
        public float generalEndurance;
        public float bossEndurance;
        public float enemyEndurance;
        public int defenseBonus;
        public int healthBonus;

        public float criticalStrikeChance;
        public float criticalStrikeDamage;

        public int stamina;
        public int accessorySlots;
        public int insourceSlots;
        public int inventorySlots;
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
        public int rangedStealthtiness;

        public float summonCastTime;
        public float summonDamage;
        public int minionSlots;
        public float mainSummonDamage;
        public float mainSummonHealth;
        public float minionSummonHealth;
        public float minionAggressiveness;

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

        private string GetComparison(string name, float currentValue)
        {
            if (currentValue == 0)
                return string.Empty;
            string percentString = MathF.Abs(currentValue).ToString("P0");
            string increaseDecreaseKey = currentValue < 0 ? "StatSubtraction" : "StatAddition";
            string comparisonText = LangText.Common(increaseDecreaseKey, LangText.Common($"Stat{name}"), percentString);
            return comparisonText;
        }

        private string GetComparison(string name, int currentValue)
        {
            if (currentValue == 0)
                return string.Empty;
            string percentString = MathF.Abs(currentValue).ToString();
            string increaseDecreaseKey = currentValue < 0 ? "StatSubtractionAlt" : "StatAdditionAlt";
            string comparisonText = LangText.Common(increaseDecreaseKey, LangText.Common($"Stat{name}"), percentString);
            return comparisonText;
        }
        public void GetStatTooltipsLocalToItem(Item item, List<TooltipLine> tooltips)
        {
            _dummyPlayer ??= new Player();
            _dummyPlayer.ResetEffects();
            if (item.accessory)
            {
                ItemLoader.UpdateAccessory(item, _dummyPlayer, false);
            }
            if (item.headSlot != -1 || item.bodySlot != -1 || item.legSlot != -1)
            {
                ItemLoader.UpdateEquip(item, _dummyPlayer);
            }
            _dummyPlayer.GetModPlayer<ArmorStatsPlayer>().GetStatTooltips(tooltips);
        }

        public void GetStatTooltips(List<TooltipLine> tooltips)
        {
            void AddLineIfDifferent(string name, float currentValue)
            {
                string comparison = GetComparison(name, currentValue);
                if (string.IsNullOrEmpty(comparison))
                    return;
                TooltipLine line = new TooltipLine(Mod, name, comparison);
                if (currentValue < 0)
                    line.OverrideColor = Color.IndianRed;
                tooltips.Add(line);
            }
            void AddLineIfDifferentInt(string name, int currentValue)
            {
                string comparison = GetComparison(name, currentValue);
                if (string.IsNullOrEmpty(comparison))
                    return;
                TooltipLine line = new TooltipLine(Mod, name, comparison);
                tooltips.Add(line);
            }

            //damage goes here
            AddLineIfDifferent("MeleeDamage", meleeDamage);
            AddLineIfDifferent("RangedDamage", rangedDamage);
            AddLineIfDifferent("MagicDamage", magicDamage);
            AddLineIfDifferent("MinionDamage", summonDamage);
            AddLineIfDifferentInt("MaxHealth", healthBonus);
            AddLineIfDifferent("CriticalStrikeChance", criticalStrikeChance);
            AddLineIfDifferent("CriticalStrikeDamage", criticalStrikeDamage);
            AddLineIfDifferentInt("Stamina", stamina);
            AddLineIfDifferentInt("ArmorPenetration", meleeArmorPenetration);
            AddLineIfDifferentInt("AccessorySlots", accessorySlots);
            AddLineIfDifferentInt("InventorySlots", inventorySlots);
            AddLineIfDifferent("MovementSpeed", movementSpeedBonus);
            AddLineIfDifferent("Endurance", generalEndurance);
            AddLineIfDifferent("BossEndurance", bossEndurance);
            AddLineIfDifferent("EnemyEndurance", enemyEndurance);
            AddLineIfDifferentInt("InsourceSlots", insourceSlots);
            AddLineIfDifferent("MeleeAttackSpeed", meleeAttackSpeed);
            AddLineIfDifferentInt("Defense", defenseBonus);
            AddLineIfDifferent("InsourceTime", insourceTimeBonus);
            AddLineIfDifferent("Aggressiveness", meleeAggressiveness);
            AddLineIfDifferent("BowChargeTime", rangedBowChargeTime);
            AddLineIfDifferentInt("Piercing", rangedPiercing);
            AddLineIfDifferentInt("GunAmmoAmount", rangedGunAmmoAmount);
            AddLineIfDifferentInt("Stealthiness", rangedStealthtiness);
            AddLineIfDifferent("SummonCastTime", summonCastTime);
            AddLineIfDifferent("MinionSlots", minionSlots);
            AddLineIfDifferent("MainMinionDamage", mainSummonDamage);
            AddLineIfDifferent("MainMinionHealth", mainSummonHealth);
            AddLineIfDifferent("MinionHealth", minionSummonHealth);
            AddLineIfDifferent("MinionAggressiveness", minionAggressiveness);
            AddLineIfDifferent("ArtifactManaReduction", artifactManaReduction);
            AddLineIfDifferent("WandCastTime", wandCastTime);
            AddLineIfDifferentInt("MaxMana", totalMana);
            AddLineIfDifferentInt("WandNormalEnchantmentSlots", wandNormalEnchantmentSlots);
            AddLineIfDifferentInt("WandTimerEnchantmentSlots", wandTimerEnchantmentSlots);
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
            Player.GetArmorPenetration(DamageClass.Melee) += meleeArmorPenetration;
            Player.aggro += meleeAggressiveness;
            Player.GetDamage(DamageClass.Ranged) += rangedDamage;
            Player.GetDamage(DamageClass.Magic) += magicDamage;
            Player.statManaMax2 += totalMana;
            Player.GetModPlayer<DashPlayer>().MaxDashCount += stamina;
            Player.GetModPlayer<FlaskPlayer>().maxInsourceCount += insourceSlots;
            Player.maxMinions += minionSlots;
            Player.aggro += meleeAggressiveness;
        }
    }
}
