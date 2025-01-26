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
    internal class DialogueTowningButtonUI : UIPanel
    {
        private Color _targetColor;
        private float _timer;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Text = new UIText("This is placeholder text", 0.5f, true);
            Text.Height.Pixels = 44;
            Text.Width.Pixels = 214;
            Text.IsWrapped = true;
            Text.Top.Pixels = 22;
            Append(Text);

            Width.Pixels = 214;
            Height.Pixels = 44;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public UIText Text { get; set; }
        public string RealText { get; set; }
        public Action OnClickEvent { get; set; }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundStyle soundStyle = SoundID.MenuTick;
            SoundEngine.PlaySound(soundStyle);
            OnClickEvent?.Invoke();
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
            if (Text == null)
                return;
            Text.SetText(RealText);
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
            Text.TextColor = textColor;
            Text.Top.Pixels = 5;

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
                drawColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
        }
    }
}
