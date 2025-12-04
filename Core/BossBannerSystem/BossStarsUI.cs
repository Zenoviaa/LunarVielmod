using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.BossBannerSystem
{
    /// <summary>
    /// Draws the star ranking of a boss
    /// </summary>
    public class BossStarsUI : UIPanel
    {
        private BossPage _bossPage;
        private readonly BossPageUI _parent;
        public BossStarsUI(BossPageUI parent)
        {
            _parent = parent;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width = _parent.Width;
            Height.Pixels = 32;
        }

        public void SetBossPage(BossPage bossPage)
        {
            _bossPage = bossPage;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            base.DrawSelf(spriteBatch);
            DrawStars(spriteBatch);
        }

        private void DrawStars(SpriteBatch spriteBatch)
        {
            Vector2 topLeft = GetDimensions().ToRectangle().TopLeft();
            var texture = BossBanner.RequestStarTexture();
            Color darkColor = Color.Lerp(Color.White, Color.Black, 0.8f);
            for (int i = 0; i < 7; i++)
            {
                float distanceBetween = 16;
                Vector2 drawPosition = topLeft + new Vector2(distanceBetween * i, 0);
                bool isLit = i < _bossPage.StarRanking;
                Color drawColor = isLit ? Color.White : darkColor;
                if (isLit)
                {
                    drawPosition.Y += ExtraMath.Osc(0f, 3f, speed: 2, offset: i);
                }

                spriteBatch.Draw(texture.Value, drawPosition, null, drawColor, 0f, default, 1, SpriteEffects.None, 0f);
            }
        }
    }
}
