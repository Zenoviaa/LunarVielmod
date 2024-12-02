using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;

using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.AdvancedMagicSystem
{
    internal class AdvancedMagicButtonUI : UIPanel
    {

        private UIPanel _button;

        internal const int width = 42;
        internal const int height = 56;

        internal int RelativeLeft => 515;
        internal int RelativeTop => Main.screenHeight / 2 - 64;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Pixels = width;
            Height.Pixels = height;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            AdvancedMagicUISystem uiSystem = ModContent.GetInstance<AdvancedMagicUISystem>();
            uiSystem.ToggleUI();
            // We can do stuff in here!
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {
            // AdvancedMagicUISystem uiSystem = ModContent.GetInstance<AdvancedMagicUISystem>();
            //  uiSystem.ToggleUI();
            // We can do stuff in here! 
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            BackgroundColor = Color.Transparent;
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw;
            if (IsMouseHovering)
            {
                point.X -= 2;
                point.Y -= 10;
                textureToDraw = ModContent.Request<Texture2D>("Stellamod/UI/AdvancedMagicSystem/PaperSelected").Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>("Stellamod/UI/AdvancedMagicSystem/Paper").Value;
            }
            spriteBatch.Draw(textureToDraw, new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height), null, Color.White);
        }
    }
}
