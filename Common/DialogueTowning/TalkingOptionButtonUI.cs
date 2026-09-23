using Stellamod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.Common.DialogueTowning
{
    public class TalkingOptionButtonUI : UIPanel
    {
        private float _realHoverAlpha;
        private float _alpha;
        private float _timer;
        private ITalkingOption _talkingOption;
        public TalkingOptionButtonUI()
        {

        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            Width.Pixels = 214;
            Height.Pixels = 44;

            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
        }

        public void SetTalkingOption(ITalkingOption talkingOption)
        {
            _talkingOption = talkingOption;

        }

        public float Alpha { get; set; }
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            if (_alpha < 0.8f)
                return;

            SoundStyle soundStyle = SoundID.MenuTick;
            SoundEngine.PlaySound(soundStyle);
            _talkingOption?.Talk();
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _alpha = MathHelper.Lerp(_alpha, Alpha, 0.1f);
  
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            _realHoverAlpha = MathHelper.Lerp(_realHoverAlpha, IsMouseHovering ? 1 : 0, (float)gameTime.ElapsedGameTime.TotalSeconds * 8);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

            base.DrawSelf(spriteBatch);
            if (_talkingOption == null)
                return;

            CalculatedStyle dimensions = GetDimensions();
            Rectangle rect = dimensions.ToRectangle();
            rect.Y += 8;

            //Daraw the background for the button
            var nameTagDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.NameTag.Asset, Vector2.Zero);

            nameTagDrawer.worldPosition = Main.screenPosition + rect.Center();

            nameTagDrawer.color = Color.White * _alpha;
            var scale = rect.Size() / AssetReferences.Content.GooberPortraits.NameTag.Asset.Value.Size();
            nameTagDrawer.scale = scale;
            nameTagDrawer.CenterOrigin();

            var pos = rect.TopLeft();
            pos.X += 8;
            pos.X += MathHelper.Lerp(0, 32, _realHoverAlpha);
            nameTagDrawer.worldPosition.X += MathHelper.Lerp(0, 32, _realHoverAlpha);
            var textScale = new Vector2(0.5f);
            var text = _talkingOption.GetDisplayName();
            var textSize = FontAssets.DeathText.Value.MeasureString(text);
            var snippets = TextHelper.ParseMessage(FontAssets.DeathText.Value, text, pos, textScale);
            var terrariaYellow = new Color(255, 231, 71);
            var highlightColor = IsMouseHovering ? terrariaYellow : Color.White;
            var textColor = Color.Lerp(Color.White, highlightColor, ExtraMath.Osc(0f, 1f, speed: 6));
            for (int i = 0; i < snippets.Length; i++)
            {
                ref var snippet = ref snippets[i];
                snippet.characterColor = textColor * _alpha;
            }
            TextHelper.DrawStringIndividually(FontAssets.DeathText.Value, spriteBatch, snippets);
   
            var nameTagOutlineDrawer = nameTagDrawer;
            nameTagOutlineDrawer.texture = AssetReferences.Content.GooberPortraits.NameTagOutline.Asset.Value;
            nameTagOutlineDrawer.worldPosition.Y -= 14;
            nameTagOutlineDrawer.worldPosition.X += 18;
            nameTagOutlineDrawer.scale.X = 1.1f;
            if (IsMouseHovering)
                nameTagOutlineDrawer.color = highlightColor * _alpha;
            spriteBatch.Draw(nameTagOutlineDrawer);
            /*
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
            drawColor *= _alpha;

            spriteBatch.Draw(textureToDraw, point.ToVector2(), null,
                drawColor, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);
            _talkingOption?.Show(spriteBatch);*/

        }
    }
}
