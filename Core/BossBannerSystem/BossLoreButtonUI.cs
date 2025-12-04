using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.BossBannerSystem
{
    /// <summary>
    /// Opens up the lore tab for the boss
    /// </summary>
    public class BossLoreButtonUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossLoreButtonUI(BossPageUI parent)
        {
            _parent = parent;
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 32;
            Height.Pixels = 32;
            OnLeftClick += _parent.ToggleLoreWindow;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch); 
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Asset<Texture2D> glassTexture = BossBanner.RequestScrollTexture();
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            Vector2 drawPosition = rectangle.TopLeft();
            drawPosition.Y += ExtraMath.Osc(0f, 2f);
            if (IsMouseHovering)
            {
                UIHelper.QuickOutline(spriteBatch, glassTexture, drawPosition, Color.Yellow);
            }

            spriteBatch.Draw(glassTexture.Value, drawPosition, null, Color.White, 0f, default, 1, SpriteEffects.None, 0f);
        }
    }
}
