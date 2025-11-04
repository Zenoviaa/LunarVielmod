using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;

using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.ModLoader;
using Terraria.ModLoader.IO;

namespace Stellamod.Core.SummonerSystem
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
                if (item.ModItem is BaseBellMinionItem bellMinion)
                {
                    baseTime += bellMinion.GetAddedCastingTime();
                }
            }
            return baseTime;
        }
        public bool isSummoning;
        public bool hasBellMinions;
        public float summonRatio => castTimer / GetCastingTime();

        public override void ResetEffects()
        {
            base.ResetEffects();
            castingTime = 60;


            isSummoning = false;
            hasBellMinions = false;
        }


        public override void PreUpdateBuffs()
        {
            base.PreUpdateBuffs();
            if (LunarVeilKeybinds.BellKeybind.Current && Main.myPlayer == Player.whoAmI)
            {
                Player.AddBuff(ModContent.BuffType<BellSummoning>(), 2);
            }
            foreach (var proj in Main.ActiveProjectiles)
            {
                if (proj.owner != Player.whoAmI)
                    continue;
                if (proj.ModProjectile is KillableMinion)
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

            for(int i = 0; i < Player.maxMinions && i < _minions.Count; i++)
            {
                var minionItem = _minions[i];
                int newDamage = (int)Player.GetTotalDamage(DamageClass.Summon).ApplyTo(minionItem.damage);
                Vector2 startpos = Player.Bottom - new Vector2(0, 50);
                startpos.X += Main.rand.NextFloat(-100, 100);
                Projectile.NewProjectile(Player.GetSource_FromThis(), startpos, Vector2.Zero,
                    ModContent.ProjectileType<SummoningBeam>(), newDamage, minionItem.knockBack, Player.whoAmI,
                    ai1: minionItem.shoot);
            }

            Player.AddBuff(ModContent.BuffType<BellExhaust>(), 600);
        }
        public override void SaveData(TagCompound tag)
        {
            base.SaveData(tag);
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
            IEnumerable<ModItem> insources = ModContent.GetContent<BaseBellMinionItem>();
            foreach (var insource in insources)
            {
                _unlockedminions.Add(insource.Item);
            }

            ManageUnlockedMinions();
        }

        public bool CanUseFlask()
        {
            return true;
        }
    }
}
