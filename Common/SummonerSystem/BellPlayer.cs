using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorRework;
using System;
using System.Collections.Generic;

using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Common.SummonerSystem
{
    public class BellExhaust : ModBuff
    {
        public override void SetStaticDefaults()
        {
            base.SetStaticDefaults();
            Main.pvpBuff[Type] = true; // This buff can be applied by other players in Pvp, so we need this to be true.

        }
    }

    public class BellPlayer : ModPlayer
    {
        private List<int> _itemTypes = new List<int>();
        private List<Item> _minions = new List<Item>();
        private List<Item> _unlockedminions = new List<Item>();
        public float castTimer;
        public float castingTime;
        public float GetCastingTime()
        {
            float baseTime = 60;
            for (int i = 0; i < Player.maxMinions && i < _minions.Count; i++)
            {
                var item = _minions[i];
                if (item.IsAir)
                    continue;
                var bellMinion = item.GetGlobalItem<BellMinionGlobalItem>();
                if (bellMinion.isBellMinion)
                    baseTime += bellMinion.addedCastingTime;
            }
            baseTime *= 1.0f - Player.GetModPlayer<ArmorStatsPlayer>().summonCastTime;
            return baseTime;
        }
        private Item _guardian;
        public Item Guardian
        {
            get
            {
                if (_guardian == null)
                {
                    _guardian = new Item();
                    _guardian.SetDefaults(0);
                }
                return _guardian;
   
            }
            set
            {
                _guardian = value;
            }
        }

        public bool isSummoning;
        public bool hasBellMinions;
        public bool hasGuardian;
        public float summonRatio => castTimer / GetCastingTime();
        public float standDamageBonus;
        public float incomingDamageMultiplier;
       
        public override void ResetEffects()
        {
            base.ResetEffects();
            castingTime = 60;
            standDamageBonus = 0f;
            incomingDamageMultiplier = 1f;

            isSummoning = false;
            hasBellMinions = false;
            hasGuardian = false;
        }


        public override void PreUpdateBuffs()
        {
            base.PreUpdateBuffs();
            if (Main.myPlayer == Player.whoAmI  && LunarVeilKeybinds.BellKeybind.Current)
            {
                Player.AddBuff(ModContent.BuffType<BellSummoning>(), 2);
            }
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Player.whoAmI)
                    continue;
                if (proj.ModProjectile is AbstractBellSummon)
                    hasBellMinions = true;
            }
            isSummoning = Player.HasBuff<BellSummoning>() && !Player.HasBuff<BellExhaust>();
        }

        public override void PostUpdateEquips()
        {
            base.PostUpdateEquips();

            if (isSummoning
                && Player.ownedProjectileCounts[ModContent.ProjectileType<SummoningCircle>()] == 0
                && Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<SummoningCircle>(), 1, 1, Player.whoAmI);
            }
            if (isSummoning
                && Player.ownedProjectileCounts[ModContent.ProjectileType<SummoningBar>()] == 0
                && Main.myPlayer == Player.whoAmI)
            {
                Projectile.NewProjectile(Player.GetSource_FromThis(), Player.Center, Vector2.Zero,
                    ModContent.ProjectileType<SummoningBar>(), 1, 1, Player.whoAmI);
            }
            if (isSummoning)
            {
                castTimer++;
                if (castTimer == 1)
                {
                    SoundStyle castingStart = new SoundStyle("Stellamod/Assets/Sounds/AuroraEnd");
                    castingStart.PitchVariance = 0.2f;
                    SoundEngine.PlaySound(castingStart, Player.position);
                }
                if (castTimer >= GetCastingTime())
                {
                    CompleteSummon();
                    castTimer = 0;
                }
            }
            else
            {
                castTimer = 0;
            }
        }

        public void CompleteSummon()
        {
            if (Main.myPlayer != Player.whoAmI)
                return;

            bool alreadyHasGuardian = false;
            foreach(var projectile in Main.ActiveProjectiles)
            {
                if (projectile.owner != Player.whoAmI)
                    continue;
                if(projectile.ModProjectile is AbstractBellSummon summon)
                {
                    if (summon.isGuardian)
                    {
                        alreadyHasGuardian = true;
                        break;
                    }
                    
                }
            }
            if(Guardian != null && !Guardian.IsAir && !alreadyHasGuardian)
            {
                var minionItem = Guardian;
                int newDamage = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(minionItem.damage);
                Vector2 startpos = Player.Bottom - new Vector2(0, 50);
                startpos.X += Main.rand.NextFloat(-100, 100);

                float health = minionItem.GetGlobalItem<BellMinionGlobalItem>().health;
                ArmorStatsPlayer statsPlayer = Player.GetModPlayer<ArmorStatsPlayer>();
                health *= 1.0f + statsPlayer.minionSummonHealth;
                SummoningBeam beam = Projectile.NewProjectileDirect(Player.GetSource_FromThis(), startpos, Vector2.Zero,
                    ModContent.ProjectileType<SummoningBeam>(), newDamage, minionItem.knockBack, Player.whoAmI,
                    ai1: minionItem.shoot, ai2: health).ModProjectile as SummoningBeam;
                beam.isGuardian = true;
            }

            for(int i = 0; i < Player.maxMinions && i < _minions.Count; i++)
            {
                var minionItem = _minions[i];
                int newDamage = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(minionItem.damage);
                Vector2 startpos = Player.Bottom - new Vector2(0, 50);
                startpos.X += Main.rand.NextFloat(-100, 100);

                float health = minionItem.GetGlobalItem<BellMinionGlobalItem>().health;
                ArmorStatsPlayer statsPlayer = Player.GetModPlayer<ArmorStatsPlayer>();
                health *= 1.0f + statsPlayer.minionSummonHealth;
                Projectile.NewProjectile(Player.GetSource_FromThis(), startpos, Vector2.Zero,
                    ModContent.ProjectileType<SummoningBeam>(), newDamage, minionItem.knockBack, Player.whoAmI,
                    ai1: minionItem.shoot, ai2: health);
            }

            Player.AddBuff(ModContent.BuffType<BellExhaust>(), 600);
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
            tag["guardian"] = ItemIO.Save(Guardian);
            tag["minions"] = _minions;
            tag["unlockedminions"] = _unlockedminions;
        }

        public override void LoadData(TagCompound tag)
        {
            base.LoadData(tag);
            _minions = new List<Item>();
            _minions = tag.Get<List<Item>>("minions");

            var u = tag.Get<List<Item>>("unlockedminions");
            _unlockedminions = u;
            Guardian = ItemIO.Load(tag.Get<TagCompound>("guardian"));
            ManageUnlockedMinions();
        }

        private void ManageUnlockedMinions()
        {
            _unlockedminions.RemoveAll(x => x.IsAir);
            _unlockedminions = _unlockedminions.Distinct().ToList();
            _itemTypes.Clear();
            foreach (var item in _unlockedminions)
            {
                _itemTypes.Add(item.type);
            }
        }
        public List<Item> GetMinions()
        {
            return _minions;
        }

        public void SetMinionAtIndex(Item item, int index)
        {
            List<Item> minions = GetMinions();
            while (minions.Count <= index)
            {
                Item emptyItem = new Item();
                emptyItem.SetDefaults(0);
                minions.Add(emptyItem);
            }
            minions[index] = item;
        }

        public Item GetMinionAtIndex(int index)
        {
            List<Item> minions = GetMinions();
            if (minions.Count > index)
            {
                return minions[index];
            }
            Item air = new Item(0);
            air.SetDefaults(0);
            return air;
        }

        public bool HasUnlocked(Item item)
        {
            return _itemTypes.Contains(item.type);
        }

        public bool HasUnlockedBell()
        {
            return true;
        }

        public void UnlockMinion(Item item)
        {
            _unlockedminions.Add(item);
            ManageUnlockedMinions();
        }
        public void UnlockFlask()
        {

        }

        public void ResetProgress()
        {
            _unlockedminions.Clear();
            ManageUnlockedMinions();
        }

        public void GrantAllProgress()
        {
            _unlockedminions.Clear();
            foreach (var item in ItemHelper.BellMinions)
            {
                _unlockedminions.Add(item);
            }

            ManageUnlockedMinions();
        }

        public bool CanUseFlask()
        {
            return true;
        }
    }
}
