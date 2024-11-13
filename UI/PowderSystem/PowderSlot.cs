using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Weapons.Igniters;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.PowderSystem
{
    internal class PowderSlot : UIElement
    {
        private Item _prevItem;
        private readonly BaseIgniterCard _card;
        private readonly int _context;
        private readonly int _index;
        private readonly float _scale;

        internal Item Item;
        internal Func<Item, bool> ValidItemFunc;

        internal event Action<int> OnEmptyMouseover;

        private int timer = 0;

        internal PowderSlot(BaseIgniterCard card, int index, int context = ItemSlot.Context.BankItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            _card = card;
            _index = index; 
            var asset = ModContent.Request<Texture2D>($"{PowderUISystem.RootTexturePath}PowderSlot");
            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
        }


        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        internal bool Valid(Item item)
        {
            if (item.ModItem is BasePowder powder)
            {
                return true;
            }

            if (item.IsAir)
            {
                return true;
            }
            return false;
        }

        internal void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                _prevItem = Item;
                //Handles all the click and hover actions based on the context
                ItemSlot.Handle(ref Item, _context);
                if (Item != _prevItem)
                {
                    UpdateHand();
                }
            }
        }

        private void GetHand()
        {
            Item = _card.Powders[_index].Clone();
        }

        private void UpdateHand()
        {
            _card.Powders[_index] = Item.Clone();
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            if (Item == null)
                GetHand();

            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            bool contains = ContainsPoint(Main.MouseScreen);
            //   Console.WriteLine(rectangle.Width);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
                HandleMouseItem();
            }

            //Draw Backing
            Color color2 = Main.inventoryBack;
            Vector2 pos = rectangle.TopLeft();

            Texture2D backingTexture = ModContent.Request<Texture2D>($"{PowderUISystem.RootTexturePath}PowderSlot").Value;
            int offset = (int)(backingTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);

            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 32, Color.White);

            //ItemSlot.Draw(spriteBatch, ref Item, _context, centerPos);
            if (contains && Item.IsAir)
            {
                timer++;
                OnEmptyMouseover?.Invoke(timer);
            }
            else if (!contains)
            {
                timer = 0;
            }

            Main.inventoryScale = oldScale;
        }
    }
}