using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.UI
{
    internal abstract class UIButtonIcon : UIPanel
    {
        public string Texture => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');
        public string SelectedTexture => Texture + "_Selected";
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48;
            Height.Pixels = 48;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            OnLeftClick += OnButtonClick;
        }

        public virtual void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {

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
                textureToDraw = ModContent.Request<Texture2D>(SelectedTexture).Value;
            }
            else
            {
                textureToDraw = ModContent.Request<Texture2D>(Texture).Value;
            }
            spriteBatch.Draw(textureToDraw, new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height), null, Color.White);
        }
    }
}
