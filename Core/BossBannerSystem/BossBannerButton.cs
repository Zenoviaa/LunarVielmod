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
        private BossPageUI _parent;
        public BossBannerButton(BossPageUI parent, BossBannerType banner)
        {
            _parent = parent;
            _banner = banner;

        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 226;
            Height.Pixels = 74;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            BossPage[] pages = BossBanner.GetBossPages(_banner);
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
            for(int i = 0; i < _bossButtons.Length; i++)
            {
                var btn = _bossButtons[i];
                btn.Left.Pixels = i * 32;
                btn.Top.Pixels = Height.Pixels / 2 - 6;
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
}
