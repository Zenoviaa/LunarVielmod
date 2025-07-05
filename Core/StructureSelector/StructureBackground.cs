using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.Core.StructureSelector
{
    internal class StructureBackground : UIPanel
    {
        public StructureBackground() : base()
        {

        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 350;
            Height.Pixels = 210;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
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
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();
            Texture2D cardTexture = ModContent.Request<Texture2D>(StructureSelectorUISystem.RootTexturePath + "StructureBackground").Value;
            spriteBatch.Draw(cardTexture, rectangle.TopLeft(), null, color2, 0f, default, 1f, SpriteEffects.None, 0f);
        }
    }
}
