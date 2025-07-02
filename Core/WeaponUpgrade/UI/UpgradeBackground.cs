using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Core.WeaponUpgrade.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;

namespace Stellamod.Core.WeaponUpgrade.UI
{
    internal class UpgradeBackground : UIPanel
    {
        public UpgradeBackground() : base()
        {

        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 248;
            Height.Pixels = 100;
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
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }


            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();
            Texture2D cardTexture = ModContent.Request<Texture2D>(WeaponUpgradeUISystem.RootTexturePath + "UpgradeBackground").Value;
            spriteBatch.Draw(cardTexture, rectangle.TopLeft(), null, color2, 0f, default(Vector2), 1f, SpriteEffects.None, 0f);
        }
    }
}
