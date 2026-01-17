using ReLogic.Content;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria.ModLoader;

namespace Stellamod.Common.BossBannerSystem
{


    public class BossBanner : ModType,
        ILocalizedModType
    {
        public string LocalizationCategory => "BossBanners";
        public string DisplayName
        {
            get
            {
                return LangText.BossBanners(this, "DisplayName");
            }
        }

        public string Description
        {
            get
            {
                return LangText.BossBanners(this, "Description");
            }
        }


        public BossPage[] Pages;
        public static Asset<Texture2D> BannerTextureAsset;
        protected sealed override void Register()
        {
            ModTypeLookup<BossBanner>.Register(this);
        }

        public override void Unload()
        {
            base.Unload();
            BannerTextureAsset = null;
        }
        public sealed override void SetupContent()
        {
            base.SetupContent();
            SetStaticDefaults();
            this.GetLocalization(nameof(DisplayName), () => "");
            this.GetLocalization(nameof(Description), () => "");
  
        }

        public static BossPage[] GetBossPages(BossBannerType banner)
        {
            List<BossPage> pages = new List<BossPage>();
            foreach (var page in ModContent.GetContent<BossPage>())
            {
                if (page.banner == banner)
                {
                    pages.Add(page);
                }
            }
            return pages.ToArray();
        }

        public static Asset<Texture2D> RequestTexture(string fileName)
        {
            Type type = typeof(BossBanner);
            return ModContent.Request<Texture2D>(type.DirectoryHere() + "/" + fileName, AssetRequestMode.ImmediateLoad);
        }

        public static Asset<Texture2D> RequestBannerTexture()
        {
            Type type = typeof(BossBanner);
            return RequestTexture(type.Name);
        }
        public static Asset<Texture2D> RequestTreasureTexture()
        {
            return RequestTexture("Treasure");
        }

        public static Asset<Texture2D> RequestGlassTexture()
        {
            return RequestTexture("Glass");
        }

        public static Asset<Texture2D> RequestScrollTexture()
        {
            return RequestTexture("Scroll");
        }

        public static Asset<Texture2D> RequestStarTexture()
        {
            return RequestTexture("Star");
        }
        public static Asset<Texture2D> RequestFogTexture()
        {
            return RequestTexture("Fog");
        }
        public static Rectangle GetTreasureFrame(BossPageRewardType type)
        {
            int frameIndex = (int)type;
            const int Frame_Height = 26;
            Rectangle frame = new Rectangle(0, frameIndex * Frame_Height, 28, Frame_Height);
            return frame;
        }
        public static Rectangle GetBannerFrame(BossBannerType type)
        {
            BannerTextureAsset ??= RequestBannerTexture();
            int frameIndex = (int)type;
            const int Frame_Height = 128;
            Rectangle frame = new Rectangle(0, frameIndex * Frame_Height, BannerTextureAsset.Value.Width, Frame_Height);
            return frame;
        }
    }
}
