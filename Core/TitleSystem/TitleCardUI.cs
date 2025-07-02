using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers.Math;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Core.TitleSystem
{
    internal class TitleCardUI : UIPanel
    {
        private UIPanel _panel;
        private UIText _text;
        private UIImage _image;
        private float _timer;
        private float _duration;
        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2;
        internal int RelativeTop => 0 + 32;

        public bool IsFinished => _timer >= _duration;
        public Asset<Texture2D> LineTexture;

        public override void OnInitialize()
        {
            base.OnInitialize();
            LineTexture = ModContent.Request<Texture2D>(TitleCardUISystem.RootTexturePath + "Underline");
            Width.Pixels = 48 * 5f;
            Height.Pixels = 48 * 16;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _panel = new UIPanel();
            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _text = new UIText("Wave 1", large: true);
            _text.Width.Pixels = Width.Pixels;
            _text.Left.Pixels = -120;
            _text.Height.Pixels = Height.Pixels;
            _text.HAlign = 0.5f;
            _text.Top.Pixels = 0;

            Append(_text);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;

            float progress = _timer / _duration;
            float easedProgress = EasingFunction.QuadraticBump(progress);
            float pixels = MathHelper.Lerp(64, 128, easedProgress);
            _text.Top.Pixels = pixels;
            _text.TextColor = Color.Lerp(Color.Transparent, Color.White, easedProgress);
        }

        public void ShowWave(string text, float duration = 5)
        {
            _text.SetText(text);
            _timer = 0;
            _duration = duration;
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            float progress = _timer / _duration;
            float easedProgress = EasingFunction.QuadraticBump(progress);
            Texture2D texture = LineTexture.Value;

            float startY = 64 / (float)Main.screenHeight;
            float endY = 128 / (float)Main.screenHeight;
            Vector2 ratioPos = new Vector2(50, MathHelper.Lerp(startY, endY, easedProgress) * 100);
            Vector2 drawPos = ratioPos;
            drawPos.X = (int)(drawPos.X * 0.01f * Main.screenWidth);
            drawPos.Y = (int)(drawPos.Y * 0.01f * Main.screenHeight);
            drawPos.Y += 82;
            Vector2 drawScale = Vector2.One;

            //Fix the position
            drawPos.X -= texture.Width / 2f;
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.Additive, default, default, default, default, Main.UIScaleMatrix);

            spriteBatch.Draw(texture, drawPos, null, Color.White * easedProgress, 0, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, default, default, default, default, Main.UIScaleMatrix);

        }
    }
}
