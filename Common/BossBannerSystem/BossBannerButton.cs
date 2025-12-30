using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.QuestSystem;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Common.BossBannerSystem
{
    /// <summary>
    /// Holds the icons for a boss
    /// </summary>
    public class BossBannerButton : UIPanel
    {
        private readonly BossBannerType _banner;
        private BossButton[] _bossButtons;
        private BossPageUI _parent;
        private UIText _bannerTitle;
        private static ProgressionComparer _progressionComparer;
        public BossBannerButton(BossPageUI parent, BossBannerType banner)
        {
            _parent = parent;
            _banner = banner;
            _bannerTitle = new UIText(LangText.BossBanners(banner, "DisplayName"), 1);
            _progressionComparer ??= new ProgressionComparer();
        }


        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 226;
            Height.Pixels = 122;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _bannerTitle.IgnoresMouseInteraction = true;
            Append(_bannerTitle);
            BossPage[] pages = BossBanner.GetBossPages(_banner);
            Array.Sort(pages, _progressionComparer);
            _bossButtons = new BossButton[pages.Length];
            for (int b = 0; b < _bossButtons.Length; b++)
            {
                _bossButtons[b] = new BossButton(_parent, pages[b]);
                Append(_bossButtons[b]);
            }

       
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
  
            float leftPixels = 0;
            for(int i = 0; i < _bossButtons.Length; i++)
            {
                var btn = _bossButtons[i];
                btn.Left.Pixels = leftPixels;
                btn.Top.Pixels = 74 / 2 - btn.Height.Pixels / 2;
                btn.Top.Pixels += 40;
                btn.Top.Pixels += ExtraMath.Osc(0f, -3f, 1, i);
                leftPixels += btn.Width.Pixels;
                leftPixels += 4;
            }



        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            Asset<Texture2D> bossBannerTexture = BossBanner.RequestBannerTexture();
            Rectangle frame = BossBanner.GetBannerFrame(_banner);
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            spriteBatch.Draw(bossBannerTexture.Value, topLeft, frame, Color.White);
        
        }

    }
    public class ProgressionComparer : IComparer<BossPage>
    {
        public int Compare(BossPage x, BossPage y)
        {
            return x.progression.CompareTo(y.progression);
        }
    }
}
