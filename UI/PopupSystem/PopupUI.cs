using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Stellamod.UI.PopupSystem
{
    internal class PopupUI : UIPanel
    {
        private float _scale;
        private UIText _text;
        internal const int width = 432;
        internal const int height = 280;

        internal int RelativeLeft => Main.screenWidth / 2;
        internal int RelativeTop => 0 + 32;


        public Vector2 Offset;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _scale = 1f;
            Width.Pixels = 48 * 5f;
            Height.Pixels = 48 * 16;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _text = new UIText("This is placeholder dialogue", 0.5f, large: true);
            _text.Width.Pixels = Width.Pixels;
            _text.Height.Pixels = Height.Pixels;
            _text.HAlign = 0.5f;
            _text.Top.Pixels = 0;
            Append(_text);
        }

        public void SetText(string text)
        {
            string localizedText = LangText.TownDialogue(text);
            _text.SetText(localizedText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft + Offset.X - (Width.Pixels / 2);
            Top.Pixels = RelativeTop + Offset.Y;

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;

            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();
            Vector2 centerPos = pos + rectangle.Size() / 2f;

            Texture2D backgroundTexture = ModContent.Request<Texture2D>($"{PopupUISystem.RootTexturePath}PopupBackground").Value;

            //Draw the background thingy

            Vector2 drawOffset = Vector2.Zero;
            drawOffset.X = Width.Pixels / 2f;
            drawOffset.Y = 18;
            Vector2 drawOrigin = backgroundTexture.Size() / 2f;
            spriteBatch.Draw(backgroundTexture, rectangle.TopLeft() + drawOffset, null, Color.DarkGray, 0f, drawOrigin, 1f, SpriteEffects.None, 0f);
            Main.inventoryScale = oldScale;
        }
    }
}
