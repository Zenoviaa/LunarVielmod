using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI
{
    public class XButton : UIElement
    {
        private Action _onClick;
        public XButton(Action onClick)
        {
            _onClick = onClick;
            OnLeftClick += Click;
            XButtonTextureAsset = ModContent.Request<Texture2D>("Stellamod/UI/XButton", AssetRequestMode.ImmediateLoad);
            Width.Pixels = XButtonTextureAsset.Width();
            Height.Pixels = XButtonTextureAsset.Height();
        }

        public Asset<Texture2D> XButtonTextureAsset;
        private void Click(UIMouseEvent evt, UIElement listeningElement)
        {
            _onClick();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 1;
            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }


            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();

            //Enchantment Card
            spriteBatch.Draw(XButtonTextureAsset.Value, rectangle.TopLeft(), null, color2, 0f, default(Vector2), 1, SpriteEffects.None, 0f);
            Main.inventoryScale = oldScale;
        }
    }
}
