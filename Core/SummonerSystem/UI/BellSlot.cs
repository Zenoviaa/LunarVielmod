using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.SummonerSystem.UI
{
    public class BellSlot : UIElement
    {
        private Item _prevItem;
        private readonly int _context;
        private readonly float _scale;
        private readonly int _slot;
        private Asset<Texture2D> _slotTextureAsset;
        public Item Item;
        public BellSlot(int slot, int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            _slot = slot;

            Item = new Item();
            Item.SetDefaults(0);
            Item = Main.LocalPlayer.GetModPlayer<BellPlayer>().GetMinionAtIndex(slot);
            _slotTextureAsset = ModContent.Request<Texture2D>(
                this.GetType().DirectoryHere() + "/BellSlot", ReLogic.Content.AssetRequestMode.ImmediateLoad);

            Width.Set(_slotTextureAsset.Width() * scale, 0f);
            Height.Set(_slotTextureAsset.Height() * scale, 0f);
        }

        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        public bool Valid(Item item)
        {
            if (item.ModItem is BaseBellMinionItem bellItem)
            {
                return true;
            }
            if (item.IsAir)
                return true;
            return false;
        }


        public void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                _prevItem = Item;
                ItemSlot.Handle(ref Item, _context);
                if (Main.mouseLeft && Main.mouseLeftRelease)
                {
                    var flaskPlayer = Main.LocalPlayer.GetModPlayer<BellPlayer>();
                    flaskPlayer.SetMinionAtIndex(Item.Clone(), _slot);
                }
            }
        }


        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                HandleMouseItem();
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();

            Texture2D backingTexture = _slotTextureAsset.Value;
            int offset = (int)(backingTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);

            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 32, Color.White);
            Main.inventoryScale = oldScale;
        }
    }
}
