using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Consumables;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.ArmorReforgeSystem
{
    internal class ReforgePearl : UIPanel
    {
        private UIText _text;
        internal event Action<int> OnEmptyMouseover;
        private readonly float _scale = 1f;
        internal ReforgePearl()
        {
            float scale = 1f;
            var asset = ModContent.Request<Texture2D>(
                $"{ReforgeUISystem.RootTexturePath}Pearl", ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            _text = new UIText("0");
            _text.Width.Pixels = Width.Pixels;
            _text.Left.Pixels = 32;
            _text.Height.Pixels = Height.Pixels;
            _text.HAlign = 0.5f;
            _text.Top.Pixels = 0;
            Append(_text);
        }
        private void OnMouseHover(UIMouseEvent evt, UIElement listeningElement)
        {

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {

            CalculatedStyle dimensions = GetDimensions();
            Point point = new Point((int)dimensions.X, (int)dimensions.Y);
            Texture2D textureToDraw;
            textureToDraw = ModContent.Request<Texture2D>($"{ReforgeUISystem.RootTexturePath}Pearl").Value;
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            Player player = Main.LocalPlayer;
            int count = player.CountItem(ModContent.ItemType<GlisteningPearl>());
            if (_text != null)
            {
                _text.SetText(count.ToString());
            }

            Color drawColor = Color.White;
            Rectangle rect = new Rectangle(point.X, point.Y, textureToDraw.Width, textureToDraw.Height);
            float rotation = 0;
            spriteBatch.Draw(textureToDraw, rect, null, drawColor, rotation, Vector2.Zero, SpriteEffects.None, 0);
        }
    }
}