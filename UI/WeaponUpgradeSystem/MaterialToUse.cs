using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.WeaponUpgradeSystem
{
    internal class MaterialToUse : UIPanel
    {
        private UIText _text;
        private UIText _requiredText;
        internal event Action<int> OnEmptyMouseover;
        private readonly float _scale = 1f;
        internal MaterialToUse()
        {
            float scale = 1f;
            var asset = ModContent.Request<Texture2D>(
                $"{WeaponUpgradeUISystem.RootTexturePath}Pearl", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Pixels = 54;
            Height.Pixels = 38;
            _text = new UIText("0");
            _text.Width.Pixels = Width.Pixels;
            _text.Left.Pixels = 32;
            _text.Height.Pixels = Height.Pixels;
            _text.HAlign = 0.5f;
            _text.Top.Pixels = 0;
            Append(_text);

            _requiredText = new UIText("0");
            _requiredText.Width.Pixels = Width.Pixels;
            _requiredText.Left.Pixels = 32;
            _requiredText.Height.Pixels = Height.Pixels;
            _requiredText.HAlign = 0.5f;
            _requiredText.Top.Pixels = 0;
            _requiredText.TextColor = Color.IndianRed;
            Append(_requiredText);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _text.Left.Pixels = 0;
            _text.Top.Pixels = 32;
            _requiredText.Left.Pixels = 0;
            Left.Set(0, 0.7f);
            Top.Set(0, 0.2f);
        }
        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw;

            WeaponUpgradeUISystem system = ModContent.GetInstance<WeaponUpgradeUISystem>();
            Asset<Texture2D> textureAsset = system.RequiredMaterialTexture;
            textureToDraw = textureAsset.Value;
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Player player = Main.LocalPlayer;
            int count = player.CountItem(system.RequiredMaterialType);
            _text.SetText(count.ToString());



            int requiredAmount = system.RequiredAmount;
            _requiredText.SetText(requiredAmount.ToString());

            Color drawColor = Color.White;

            float rotation = 0;

            Rectangle rectangle = GetDimensions().ToRectangle();
            Texture2D background = ModContent.Request<Texture2D>(WeaponUpgradeUISystem.RootTexturePath + "MaterialBox").Value;
            spriteBatch.Draw(background, rectangle, null, drawColor, rotation, Vector2.Zero, SpriteEffects.None, 0);

            Rectangle rect = new Rectangle(point.X + rectangle.Width / 2 - textureToDraw.Width / 2, point.Y - rectangle.Height, textureToDraw.Width, textureToDraw.Height);

            spriteBatch.Draw(textureToDraw, rect, null, drawColor, rotation, Vector2.Zero, SpriteEffects.None, 0);

        }
    }
}