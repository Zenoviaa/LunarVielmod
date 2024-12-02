using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.MoonlightMagic;
using System;
using Terraria;
using Terraria.GameContent;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;


namespace Stellamod.UI.AdvancedMagicSystem
{
    internal class AdvancedMagicItemSlot : UIElement
    {
        private Item _prevItem;
        private static Item _prevTrashItem;
        internal Item Item;
        private static int _lastTrashedIndex;
        private readonly int _context;
        private readonly float _scale;

        internal event Action<int> OnEmptyMouseover;

        private int timer = 0;

        internal AdvancedMagicItemSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;

            _scale = scale;
            Item = new Item();
            Item.SetDefaults(0);

            var inventoryBack9 = TextureAssets.InventoryBack9;
            Width.Set(inventoryBack9.Width() * scale, 0f);
            Height.Set(inventoryBack9.Height() * scale, 0f);
        }

        public int index;

        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        internal bool Valid(Item item)
        {
            return item.ModItem is BaseMagicItem || item.IsAir;
        }

        internal void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                _prevItem = Item;

                //Handles all the click and hover actions based on the context
                ItemSlot.Handle(ref Item, _context);

                if (_prevTrashItem == null)
                {
                    _prevTrashItem = Main.LocalPlayer.trashItem;
                }

                bool isBothAir = _prevTrashItem.IsAir && Main.LocalPlayer.trashItem.IsAir;
                bool notTheSame = _prevTrashItem != Main.LocalPlayer.trashItem;
                bool sameItemType = Main.LocalPlayer.trashItem.type == Item.type;
                if (notTheSame && sameItemType && !isBothAir)
                {
                    _prevTrashItem = Main.LocalPlayer.trashItem;
                    Item = new Item();
                    Item.SetDefaults(0);
                }

                if (Item != _prevItem)
                {
                    SaveToBackpack();
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
            Texture2D value = ModContent.Request<Texture2D>("Stellamod/UI/AdvancedMagicSystem/EnchantmentSlot").Value;

            Vector2 centerPos = pos + rectangle.Size() / 2f;


            spriteBatch.Draw(value, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);
            //DrawHelper.DrawGlowInInventory(Item, spriteBatch, centerPos, Color.AliceBlue);
            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale, 32, Color.White);

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

        public void SaveToBackpack()
        {
            var player = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
            player.Backpack[index] = Item.Clone();
        }

        public void Refresh()
        {
            var player = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
            Item = player.Backpack[index].Clone();
            _prevItem = Item;
        }
    }
}
