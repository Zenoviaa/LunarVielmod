using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.MagicSystem.UI
{
    internal class InventoryBackground : UIElement
    {
        private readonly float _scale;
        internal InventoryBackground(float scale = 1f)
        {
            _scale = scale;

            string texturePath = GetType().DirectoryHere() + "/InventoryPanel";
            BackgroundAsset = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(BackgroundAsset.Width() * scale, 0f);
            Height.Set(BackgroundAsset.Height() * scale, 0f);
        }
        public Asset<Texture2D> BackgroundAsset { get; private set; }
        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();

            //Draw Backing
            Vector2 pos = rectangle.TopLeft();

            //Draw the background and then draw the item icon
            Texture2D value = BackgroundAsset.Value;
            spriteBatch.Draw(value, rectangle.TopLeft(), null, Color.White, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);
            Main.inventoryScale = oldScale;
        }
    }
}
