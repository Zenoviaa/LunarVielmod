using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.ItemBrowser
{
    public class ItemBrowserModFilterButton : UIPanel
    {
        private ItemBrowserMenu _menu;
        private Asset<Texture2D> _buttonTextureAsset;
        public ItemBrowserModFilterButton(ItemBrowserMenu menu) : base()
        {
            _menu = menu;
            OnLeftClick += SetSorting;
            string texturePath = typeof(ItemBrowserModFilterButton).DirectoryHere() + "/ModButton";
            _buttonTextureAsset = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(_buttonTextureAsset.Width(), 0f);
            Height.Set(_buttonTextureAsset.Height(), 0f);
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }

        private void SetSorting(UIMouseEvent evt, UIElement listeningElement)
        {
            _menu.modFilter = !_menu.modFilter;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 32;
            Height.Pixels = 32;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (IsMouseHovering && !Main.LocalPlayer.mouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = 1f;
            Rectangle rectangle = GetDimensions().ToRectangle();
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }


            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();

            //Enchantment Card
            color2 = _menu.modFilter ? Color.Yellow : Color.White;

            Texture2D cardTexture = _buttonTextureAsset.Value;

            int offset = (int)(cardTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(cardTexture, centerPos, null, color2, 0f, _buttonTextureAsset.Size() / 2f, 1f, SpriteEffects.None, 0f);

            Main.inventoryScale = oldScale;
        }
    }
}
