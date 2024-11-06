using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.GunHolsterSystem
{
    internal class GunHolsterRightSlot : UIElement
    {
        private Item _prevItem;
        private readonly int _context;
        private readonly float _scale;

        internal Item Item;
        internal Func<Item, bool> ValidItemFunc;

        internal event Action<int> OnEmptyMouseover;

        private int timer = 0;

        internal GunHolsterRightSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            var asset = ModContent.Request<Texture2D>(
                $"{GunHolsterUISystem.RootTexturePath}RightSlot", ReLogic.Content.AssetRequestMode.ImmediateLoad);

            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
        }

        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        internal bool Valid(Item item)
        {
            if(item.ModItem is MiniGun miniGun)
            {
                if (miniGun.RightHand || miniGun.TwoHands)
                    return true;
            }
            if (item.IsAir)
                return true;
            return false;
        }

        internal void HandleMouseItem()
        {
            if (Valid(Main.mouseItem))
            {
                _prevItem = Item;
                ItemSlot.Handle(ref Item, _context);
                UpdateHand();
            }
        }

        private void GetHand()
        {
            GunPlayer gunPlayer = Main.LocalPlayer.GetModPlayer<GunPlayer>();
            Item = gunPlayer.RightHand.Clone();
        }

        private void UpdateHand()
        {
            GunPlayer gunPlayer = Main.LocalPlayer.GetModPlayer<GunPlayer>();
            gunPlayer.RightHand = Item.Clone();
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

            Texture2D backingTexture = ModContent.Request<Texture2D>($"{GunHolsterUISystem.RootTexturePath}RightSlot").Value;
            int offset = (int)(backingTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);

            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos + new Vector2(0, 3), _scale, 32, Color.White);

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
