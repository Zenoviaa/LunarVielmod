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
    public class BossRewardsButtonUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossRewardsButtonUI(BossPageUI parent)
        {
            _parent = parent;
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 32;
            Height.Pixels = 32;
            OnLeftClick += _parent.CycleRewardsType;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Asset<Texture2D> glassTexture = BossBanner.RequestTreasureTexture();
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            Vector2 drawPosition = rectangle.TopLeft();
            drawPosition.Y += ExtraMath.Osc(0f, 2f);

            Rectangle frame = BossBanner.GetTreasureFrame(_parent.RewardType);


            Color drawColor = _parent.Page == 2 ? Color.White : Color.DarkGray;
            if (_parent.Page == 2)
            {
                UIHelper.QuickOutline(spriteBatch, glassTexture.Value, frame, drawPosition, Color.White);

            }
            if (IsMouseHovering)
            {
                UIHelper.QuickOutline(spriteBatch, glassTexture.Value, frame, drawPosition, Color.Yellow);
            }

     
            spriteBatch.Draw(glassTexture.Value, drawPosition, frame, drawColor, 0f, default, 1, SpriteEffects.None, 0f);


            if (IsMouseHovering)
            {
                Color hoverColor = Color.White;
                hoverColor.A = 0;
                hoverColor *= 0.5f;
                spriteBatch.Draw(glassTexture.Value, drawPosition, frame, hoverColor, 0f, default, 1, SpriteEffects.None, 0f);
            }
        }
    }
}
