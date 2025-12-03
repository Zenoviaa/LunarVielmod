using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.BossBannerSystem
{
    /// <summary>
    /// Holds the icons for a boss
    /// </summary>
    public class BossBannerButton : UIPanel
    {
        private readonly BossBannerType _banner;
        private BossButton[] _bossButtons;
        public BossBannerButton(BossPageUI parent, BossBannerType banner)
        {
            _banner = banner;
            BossPage[] pages = BossBanner.GetBossPages(banner);
            _bossButtons = new BossButton[pages.Length];
            for(int b = 0; b < _bossButtons.Length; b++)
            {
                _bossButtons[b] = new BossButton(parent, pages[b]);
            }
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 226;
            Height.Pixels = 74;
            for(int i = 0; i < _bossButtons.Length; i++)
            {
                Append(_bossButtons[i]);
            }
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Asset<Texture2D> bossBannerTexture = BossBanner.RequestBannerTexture();
            Rectangle frame = BossBanner.GetBannerFrame(_banner);
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            spriteBatch.Draw(bossBannerTexture.Value, topLeft, frame, Color.White);
        
        }

    }
}
