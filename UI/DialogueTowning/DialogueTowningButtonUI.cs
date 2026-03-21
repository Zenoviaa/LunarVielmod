using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.DialogueTowning
{
    public class DialogueTowningButtonUI : UIPanel
    {
        private float _timer;
        private float _spriteAlpha;
        public DialogueTowningButtonUI()
        {
            realText = string.Empty;
            UIText = new UIText("This is placeholder text", 0.5f, true);
            UIText.Height.Pixels = 44;
            UIText.Width.Pixels = 214;
        }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Append(UIText);

            Width.Pixels = 214;
            Height.Pixels = 44;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public readonly UIText UIText;
        public string realText;
        public Action onClickEvent;
        public float alpha;
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundStyle soundStyle = SoundID.MenuTick;
            SoundEngine.PlaySound(soundStyle);
            onClickEvent?.Invoke();
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
            if (UIText == null)
                return;
            UIText.SetText(realText);
            if (IsMouseHovering)
            {
                _timer += (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer >= 0.12f)
                {
                    _timer = 0.12f;
                }
            }
            else
            {
                _timer -= (float)gameTime.ElapsedGameTime.TotalSeconds;
                if (_timer <= 0)
                {
                    _timer = 0f;
                }
            }
            Color textColor = Color.Lerp(Color.White, Color.Yellow, _timer / 0.12f);
            _spriteAlpha = MathHelper.Lerp(_spriteAlpha, alpha, (float)gameTime.ElapsedGameTime.TotalSeconds * 32);
            UIText.TextColor = textColor * _spriteAlpha;
            UIText.Top.Pixels = 5;

            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Color drawColor = Color.White.MultiplyRGB(Color.DarkGray);
            drawColor *= 0.2f;
            if (IsMouseHovering)
            {
                float progress = _timer / 0.12f;
                Color colorToMultiplyBy = Color.Lerp(Color.White, Color.LightGoldenrodYellow, progress);
                drawColor = drawColor.MultiplyRGB(colorToMultiplyBy);
            }
            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw = ModContent.Request<Texture2D>($"{DialogueTowningUISystem.RootTexturePath}DialogueBoxButton").Value;


            spriteBatch.Draw(textureToDraw, point.ToVector2(), null,
                drawColor * _spriteAlpha, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
    }
}
