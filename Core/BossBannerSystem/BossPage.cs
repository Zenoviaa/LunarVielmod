using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Content.Quests.ZuiQuest;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.ModLoader;

namespace Stellamod.Core.BossBannerSystem
{
    public class BossPage : ModType,
        ILocalizedModType
    {
        public static List<BossPage> Pages { get; private set; }
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
        public List<Item> NoHitRewards;
        public int StarRanking;
        public BossBannerType banner;
        public ModNPC bossNPC;
        protected sealed override void Register()
        {
            ModTypeLookup<BossPage>.Register(this);
        }

        public sealed override void SetupContent()
        {
            base.SetupContent();
            Rewards = new List<Item>();
            NoHitRewards = new List<Item>();
            Pages ??= new List<BossPage>();
            Pages.Add(this);
            SetStaticDefaults();
            this.GetLocalization(nameof(DisplayName), () => "Who???");
            this.GetLocalization(nameof(WhereToFind), () => "In Your Mom");
            this.GetLocalization(nameof(Lore), () => "Birthed by your mom");
        }

        public void AddReward(Item item)
        {
            Rewards.Add(item);
        }

        public void AddNoHitReward(Item item)
        {
            NoHitRewards.Add(item);
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
