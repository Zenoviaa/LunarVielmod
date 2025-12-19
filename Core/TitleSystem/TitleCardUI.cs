using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using ReLogic.Graphics;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Core.TitleSystem
{
    public class TitleCardUI : UIPanel
    {
        private UIPanel _panel;
        private UIText _text;
        public TitleCardUI()
        {
            _panel = new UIPanel();
            _text = new UIText("Wave 1", large: true);
        }


        private float _flashInAlpha;
        private float _timer;
        private float _duration;
        public const int width = 480;
        public const int height = 155;

        public int RelativeLeft => Main.screenWidth / 2;
        public int RelativeTop => 0 + 32;

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

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

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
            _flashInAlpha = MathHelper.Lerp(1f, 0f, EasingFunction.OutExpo(progress / 0.5f));
            
            float easedProgress = EasingFunction.QuadraticBump(progress);
            float pixels = MathHelper.Lerp(32, 64, easedProgress);
            _text.Top.Pixels = pixels;
            _text.TextColor = Color.Lerp(Color.Transparent, Color.White, easedProgress);
        }

        public void ShowWave(string text, float duration = 5)
        {
            _text.SetText(text);
            _timer = 0;
            _duration = duration;
        }


        private void DrawGlowText(SpriteBatch spriteBatch)
        {

        }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

            float progress = _timer / _duration;
            float easedProgress = EasingFunction.QuadraticBump(progress);
            Texture2D texture = LineTexture.Value;

            float startY = 32 / (float)Main.screenHeight;
            float endY = 64 / (float)Main.screenHeight;
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
            for(int i = 0; i < 3; i++)
            {
                spriteBatch.Draw(texture, drawPos + texture.Size() / 2f, null, Color.White * _flashInAlpha, 0, texture.Size() / 2f, drawScale + Vector2.One * _flashInAlpha * new Vector2(2, 1), SpriteEffects.None, 0f);
                spriteBatch.Draw(texture, drawPos + texture.Size() / 2f, null, Color.White * _flashInAlpha, 0, texture.Size() / 2f, drawScale + Vector2.One * _flashInAlpha, SpriteEffects.None, 0f);
            }

            spriteBatch.Draw(texture, drawPos, null, Color.White * easedProgress, 0, Vector2.Zero, drawScale, SpriteEffects.None, 0f);
            spriteBatch.End();
            spriteBatch.Begin(SpriteSortMode.Immediate, BlendState.AlphaBlend, Main.DefaultSamplerState, default, Main.Rasterizer, default, Main.UIScaleMatrix);
        
        }
    }
}
