using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.Items.MoonlightMagic;
using Stellamod.Content.Quests.ZuiQuest;
using Stellamod.Core;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.Localization;
using Terraria.ModLoader;

namespace Stellamod.Common.BossBannerSystem
{
    public enum BossPageRewardType : byte
    {
        Rewards,
        MasterModeRewards,
        NoHitRewards
    }

    public enum BossTooltipType : byte
    {
        WhereToFindThem,
        Lore,
        Treasure
    }

    public class BossTooltipItem : ModItem
    {
        public override string Texture => TextureRegistry.EmptyTexture;
        public BossTooltipType tooltipType;
        public override void ModifyTooltips(List<TooltipLine> tooltips)
        {
            base.ModifyTooltips(tooltips);
            tooltips.Clear();
            TooltipLine line;
            switch (tooltipType)
            {
                default:
                case BossTooltipType.WhereToFindThem:
                    line = new TooltipLine(Mod, "helpme", LangText.BossBanners("WhereHelp"));
                    break;
                case BossTooltipType.Lore:
                    line = new TooltipLine(Mod, "helpme", LangText.BossBanners("LoreHelp"));
                    break;
                case BossTooltipType.Treasure:
                    line = new TooltipLine(Mod, "helpme", LangText.BossBanners("TreasureHelp"));
                    break;
            }
            tooltips.Add(line);
        }
        public static void Hover(BossTooltipType tooltipType)
        {
            Main.hoverItemName = "123";
            var tooltipItem = ModContent.GetInstance<BossTooltipItem>();
            tooltipItem.tooltipType = tooltipType;
            Main.HoverItem = tooltipItem.Item;
        }

    }
    public class BossPage : ModType,
        ILocalizedModType
    {
        public string LocalizationCategory => "BossPages";
        public string DisplayName
        {
            get
            {
                return LangText.BossPages(this, "DisplayName");
            }
        }

        public string Lore
        {
            get
            {
                return LangText.BossPages(this, "Lore");
            }
        }

        public string WhereToFind
        {
            get
            {
                return LangText.BossPages(this, "WhereToFind");
            }
        }

        public List<Item> Rewards;
        public List<Item> MasterModeRewards;
        public List<Item> NoHitRewards;
        public int StarRanking;
        public BossBannerType banner;
        public ModNPC bossNPC;
        public float progression;
        public DownedBossFlag flag;
        public override void Unload()
        {
            base.Unload();
            Rewards = null;
            MasterModeRewards = null;
            NoHitRewards = null;
            bossNPC = null;
        }

        protected sealed override void Register()
        {
            ModTypeLookup<BossPage>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            Rewards = new List<Item>();
            MasterModeRewards = new List<Item>();
            NoHitRewards = new List<Item>();
            SetStaticDefaults();
            this.GetLocalization(nameof(DisplayName), () => "Who???");
            this.GetLocalization(nameof(WhereToFind), () => "In Your Mom");
            this.GetLocalization(nameof(Lore), () => "Birthed by your mom");
        }

        public bool CanClaimRewards()
        {
            return DownedBossTracker.IsDowned(flag);
        }
        public bool CanClaimMasterRewards()
        {
            return DownedBossTracker.IsDowned(flag) && Main.masterMode;
        }
        public bool CanClaimNoHitRewards()
        {
            return DownedBossTracker.IsNoHit(flag);
        }
        public bool HasUnclaimedRewards()
        {
            bool regularRewards = CanClaimRewards();
            bool masterRewards = CanClaimMasterRewards();
            bool noHitRewards = CanClaimNoHitRewards();
            DownedBossRewardPlayer rewardPlayer = Main.LocalPlayer.GetModPlayer<DownedBossRewardPlayer>();
            int type = (int)flag;
            if (regularRewards && !rewardPlayer.claimedRegularRewards[type])
                return true;
            if (masterRewards && !rewardPlayer.claimedMasterRewards[type])
                return true;
            if (noHitRewards && !rewardPlayer.claimedNoHit[type])
                return true;

            return false;
        }

        public static bool HasAnyUnclaimedRewards(Player player)
        {
            DownedBossRewardPlayer rewardPlayer = player.GetModPlayer<DownedBossRewardPlayer>();
            int length = Enum.GetNames<DownedBossFlag>().Length;
            for(int flag = 0; flag < length; flag++)
            {
                bool regularRewards = DownedBossTracker.IsDowned(flag);
                bool masterRewards = DownedBossTracker.IsDowned(flag) && Main.masterMode;
                bool noHitRewards = DownedBossTracker.IsNoHit(flag);
         

                if (regularRewards && !rewardPlayer.claimedRegularRewards[flag])
                    return true;
                if (masterRewards && !rewardPlayer.claimedMasterRewards[flag])
                    return true;
                if (noHitRewards && !rewardPlayer.claimedNoHit[flag])
                    return true;

            }
            return false;
        }
        public void AddReward<T>(int stack = 1) where T : ModItem
        {
            Item item = ModContent.GetInstance<T>().Item;
            Item clone = item.Clone();
            clone.stack=stack;
            Rewards.Add(clone);
        }

        public void AddMasterModeReward<T>(int stack = 1) where T : ModItem
        {
            Item item = ModContent.GetInstance<T>().Item;
            Item clone = item.Clone();
            clone.stack = stack;
            MasterModeRewards.Add(clone);
        }

        public void AddNoHitReward<T>(int stack = 1) where T : ModItem
        {
            Item item = ModContent.GetInstance<T>().Item;
            Item clone = item.Clone();
            clone.stack = stack;
            NoHitRewards.Add(clone);
        }

        public void Grant(List<Item> rewards)
        {
            foreach(Item item in rewards)
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), item.type, item.stack);
            }
        }

        public bool IsHidden()
        {
            return !DownedBossTracker.IsDowned(flag);
        }

        public List<Item> GetRewards(BossPageRewardType rewardType)
        {
            switch (rewardType)
            {
                default:
                case BossPageRewardType.Rewards:
                    return Rewards;
                case BossPageRewardType.MasterModeRewards:
                    return MasterModeRewards;
                case BossPageRewardType.NoHitRewards:
                    return NoHitRewards;
            }
        }
        public Asset<Texture2D> RequestBossPhoto()
        {
            Type type = this.GetType();
            string path = type.DirectoryHere() + "/" + type.Name;
            if(ModContent.RequestIfExists<Texture2D>(path, out var asset))
            {
                return asset;
            }
            return ModContent.Request<Texture2D>(ModContent.GetInstance<GoTrainII>().BigTexture);
        }

        public Asset<Texture2D> RequestBossIcon()
        {
            if(bossNPC is ScarletBoss boss)
            {
                return ModContent.Request<Texture2D>(boss.Texture_BossIcon);
            }
            return ModContent.Request<Texture2D>(TextureRegistry.EmptyTexture);
        }
    }
}
