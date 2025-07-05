using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.UI;

namespace Stellamod.Core.StructureSelector
{
    internal class MagicWandSlot : UIElement
    {
        private readonly int _context;
        private readonly int _index;
        private readonly float _scale;
        internal Item Item;
        internal MagicWandSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;

            //Set to Air
            Item = new Item();
            Item.SetDefaults(0);

            var inventoryBack9 = TextureAssets.InventoryBack9;
            Width.Set(inventoryBack9.Width() * scale, 0f);
            Height.Set(inventoryBack9.Height() * scale, 0f);
        }

        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        internal bool Valid(Item item)
        {
            return true;
        }

        internal void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                ItemSlot.Handle(ref Item, _context);
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
            Texture2D value = TextureAssets.InventoryBack9.Value;

            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(value, rectangle.TopLeft(), null, color2, 0f, default, _scale, SpriteEffects.None, 0f);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 32, Color.White);
            Main.inventoryScale = oldScale;
        }
    }
}
