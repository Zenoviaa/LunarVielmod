using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Common.BlackSystem
{
    public class BlackUI : UIPanel
    {
        public BlackUI()
        {
            Color = Color.White;
        }

        public Color Color;
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            ResizeUI();
            SetScreenTint();

        }
        private void ResizeUI()
        {
            Width.Pixels = Main.screenWidth;
            Height.Pixels = Main.screenHeight;
        }
        private void SetScreenTint()
        {
            Color fadeColor = Color;
            BackgroundColor = fadeColor;
            BorderColor = fadeColor;
        }
    }
}
