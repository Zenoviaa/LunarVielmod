using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.BossBannerSystem
{
    /// <summary>
    /// Opens a window that shows where you can find the boss
    /// </summary>
    public class BossFindButtonUI : UIPanel
    {
        private readonly BossPageUI _parent;
        public BossFindButtonUI(BossPageUI parent)
        {
            //I love dependency injection
            _parent = parent;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            //Width/Height doesn't need to be accurate thankfully
            Width.Pixels = 32;
            Height.Pixels = 32;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            OnLeftClick += _parent.ToggleLocationWindow;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Now setting the position cause the page will set that
            //We just need the width I think?
            //Also click function
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            //Draw the glass texture
            Asset<Texture2D> texture = BossBanner.RequestGlassTexture();
            Rectangle rectangle = UIHelper.MouseInterfaceInteraction(this);
            Vector2 drawPosition = rectangle.TopLeft();

            //Adding a little bit of hover would be cool
            drawPosition.Y += ExtraMath.Osc(0f, 2f, speed: 1);

            //We also need a hover outline probably
            //I think I have a white shader somewhere
            Color drawColor = _parent.Page == 0 ? Color.White : Color.DarkGray;
            if (_parent.Page == 0)
            {
                UIHelper.QuickOutline(spriteBatch, texture.Value, drawPosition, Color.White);
            }
            if (IsMouseHovering)
            {
                UIHelper.QuickOutline(spriteBatch, texture.Value, drawPosition, Color.Yellow);
            }

            spriteBatch.Draw(texture.Value, drawPosition, null, drawColor, 0f, default, 1, SpriteEffects.None, 0f);


            if (IsMouseHovering)
            {
                Color hoverColor = Color.White;
                hoverColor.A = 0;
                hoverColor *= 0.5f;
                spriteBatch.Draw(texture.Value, drawPosition, null, hoverColor, 0f, default, 1, SpriteEffects.None, 0f);
            }
        }
    }
}
