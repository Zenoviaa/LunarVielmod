using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.Foggy;
using Stellamod.UI.PopupSystem;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.ToolsSystem
{
    internal class FogButton : UIPanel
    {

        private UIPanel _button;

        internal const int width = 42;
        internal const int height = 56;

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Pixels = width;
            Height.Pixels = height;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            FogSystem fogSystem = ModContent.GetInstance<FogSystem>();
            fogSystem.doDraws = !fogSystem.doDraws;
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
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw;
            if (IsMouseHovering)
            {
                textureToDraw = ModContent.Request<Texture2D>(ToolsUISystem.RootTexturePath + "PaperSelected").Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>(ToolsUISystem.RootTexturePath + "Paper").Value;
            }
            spriteBatch.Draw(textureToDraw, new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height), null, Color.White);
        }
    }
}
