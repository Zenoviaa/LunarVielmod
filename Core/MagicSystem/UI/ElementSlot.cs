using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.MagicSystem.UI
{
    internal class ElementSlot : UIElement
    {
        private StaffEditingContext _ctx;
        private readonly int _context;
        private readonly float _scale;

        internal Item Item;
        internal ElementSlot(int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;


            //Set to Air
            Item = new Item();
            Item.SetDefaults(0);

            string texturePath = GetType().DirectoryHere() + "/ElementSlot";
            ElementSlotAsset = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(ElementSlotAsset.Width() * scale, 0f);
            Height.Set(ElementSlotAsset.Height() * scale, 0f);
        }

        public Asset<Texture2D> ElementSlotAsset { get; private set; }
        public void SetContext(StaffEditingContext ctx)
        {
            //Set the context
            _ctx = ctx;
        }

        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        internal bool Valid(Item item)
        {
            //No context, can't edit.
            if (_ctx == null)
                return false;
            return item.ModItem is Element || item.IsAir;
        }

        internal void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                ItemSlot.Handle(ref Item, _context);

                //Set the staff element when we swap
                _ctx.SetElement(Item);
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
            Vector2 pos = rectangle.TopLeft();

            //Draw the background and then draw the item icon
            Texture2D value = ElementSlotAsset.Value;
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(value, rectangle.TopLeft(), null, Color.White, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);

            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 64, Color.White);
            Main.inventoryScale = oldScale;
        }
    }
}
