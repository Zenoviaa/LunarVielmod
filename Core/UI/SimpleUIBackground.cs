using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.UI
{
    internal abstract class SimpleUIBackground : UIElement
    {
        public string Texture => (GetType().Namespace + "." + GetType().Name).Replace('.', '/');
        public Color Color;
        public Asset<Texture2D> TextureAsset;
        public SimpleUIBackground() : base()
        {
            Color = Color.White;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            TextureAsset = ModContent.Request<Texture2D>(Texture, AssetRequestMode.ImmediateLoad);
            Width.Pixels = TextureAsset.Width();
            Height.Pixels = TextureAsset.Height();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            Rectangle rectangle = GetDimensions().ToRectangle();

            //Draw Backing
            Vector2 pos = rectangle.TopLeft();
            spriteBatch.Draw(TextureAsset.Value, rectangle.TopLeft(), null, Color, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
        }
    }
}
