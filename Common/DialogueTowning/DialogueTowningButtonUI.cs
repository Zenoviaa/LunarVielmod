using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core;
using System;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.Localization;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.DialogueTowning
{
    public class DialogueTowningButtonUI : UIPanel
    {
        private float _realHoverAlpha;
        private float _hoverAlpha;
        private float _timer;
        private float _spriteAlpha;
        public DialogueTowningButtonUI()
        {
            realText = string.Empty;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 200;
            Height.Pixels = 42;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            OnLeftClick += OnButtonClick;
            OnMouseOver += OnMouseHover;
            
        }

        public int index;
        public string realText;
        public Action onClickEvent;
        public float alpha;
        public SpeechBoxTalkingParameters talkingParameters;
        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundStyle soundStyle = SoundID.MenuTick;
            SoundEngine.PlaySound(soundStyle);
            onClickEvent?.Invoke();
        }

        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
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
            _hoverAlpha = _timer / 0.12f;
            _realHoverAlpha = MathHelper.Lerp(_realHoverAlpha, _hoverAlpha, (float)gameTime.ElapsedGameTime.TotalSeconds * 32);
            _spriteAlpha = MathHelper.Lerp(_spriteAlpha, alpha, (float)gameTime.ElapsedGameTime.TotalSeconds * 12);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            Height.Pixels = 42;
            var terrariaYellow = new Color(255, 230, 71);
            var textColor = Color.Lerp(Color.White, terrariaYellow, _realHoverAlpha);
            CalculatedStyle dimensions = GetDimensions();
            Rectangle rect = dimensions.ToRectangle();
            rect.Y += 8;

            rect = rect.CenterPad((int)(16 * _realHoverAlpha));
            //Daraw the background for the button
            var nameTagDrawer = SpritebatchDrawer.FromTextureAsset(AssetReferences.Content.GooberPortraits.NameTag.Asset, Vector2.Zero);

            nameTagDrawer.worldPosition = Main.screenPosition + rect.Center();
            nameTagDrawer.worldPosition.Y += MathHelper.Lerp(16, 0, _spriteAlpha);
       //     nameTagDrawer.worldPosition.Y += MathHelper.Lerp(0, -8, _realHoverAlpha);

            nameTagDrawer.color = Color.White * ExtraMath.Osc(0.8f, 1f, speed: 3, offset: index) * _spriteAlpha;
            var scale = rect.Size() / AssetReferences.Content.GooberPortraits.NameTag.Asset.Value.Size() ;
            scale *= MathHelper.Lerp(1f, 0.9f, EasingFunction.InOutSine(_spriteAlpha));
            nameTagDrawer.scale = scale;
            nameTagDrawer.CenterOrigin();



            var gradientPass = AssetReferences.Effects.CrystalShaders.SimpleGradient.CreatePixelPass();
            Color startColor = Color.Lerp(talkingParameters.profile.startGradientColor, Color.Black, 0.25f);
            Color endColor = Color.Lerp(talkingParameters.profile.endGradientColor, Color.Black, 0.25f);
            gradientPass.Parameters.startGradientColor = startColor.ToVector4();
            gradientPass.Parameters.endGradientColor = endColor.ToVector4();
            gradientPass.Apply();

            using (new SpritebatchContext(spriteBatch, spriteBatch.Parameters with { effect = gradientPass.Shader }))
            {
                nameTagDrawer.color *= 0.5f;
                spriteBatch.Draw(nameTagDrawer);
            }
            var nameTagOutlineDrawer = nameTagDrawer;
            nameTagOutlineDrawer.texture = AssetReferences.Content.GooberPortraits.NameTagOutline.Asset.Value;
            nameTagOutlineDrawer.color = Color.Lerp(Color.White, terrariaYellow, _realHoverAlpha) * _spriteAlpha;
            nameTagOutlineDrawer.color *= 0.25f;
            spriteBatch.Draw(nameTagOutlineDrawer);

            Vector2 pos = rect.Center();
            pos -= FontAssets.DeathText.Value.MeasureString(realText) * 0.5f * 0.65f;
        pos.Y += MathHelper.Lerp(16, 0, _spriteAlpha);
            var snippets = TextHelper.ParseMessage(FontAssets.DeathText.Value, realText, pos, Vector2.One * 0.65f, lineSpacing: 65, maxWidth: 400);
            for(var i = 0; i < snippets.Length; i++)
            {
                ref var snippet = ref snippets[i];
                snippet.characterColor = textColor * _spriteAlpha;
            }


            TextHelper.DrawStringIndividually(FontAssets.DeathText.Value, spriteBatch, snippets);
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


            spriteBatch.Draw(textureToDraw, point.ToVector2(), null,
                drawColor * _spriteAlpha, 0f, Vector2.Zero, 0.5f, SpriteEffects.None, 0f);*/
        }
    }
}
