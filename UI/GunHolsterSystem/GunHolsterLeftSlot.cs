using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Common.ScorpionMountSystem;
using Stellamod.Items.Weapons.Ranged.GunSwapping;
using System;
using Terraria;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.GunHolsterSystem
{
    internal class GunHolsterLeftSlot : UIElement
    {
        private GunHolster _gunHolster;
        private BaseScorpionItem _scorpionItem;
        private Item _prevItem;
        private readonly int _context;
        private readonly float _scale;

        internal Item Item;

        internal GunHolsterLeftSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;

            var asset = ModContent.Request<Texture2D>(
                $"{GunHolsterUISystem.RootTexturePath}LeftSlot", ReLogic.Content.AssetRequestMode.ImmediateLoad);

            Width.Set(asset.Width() * scale, 0f);
            Height.Set(asset.Height() * scale, 0f);
            Item = new Item();
            Item.SetDefaults(0);
        }

        public int scorpionIndex = -1;
        /// <summary>
        /// Returns true if this item can be placed into the slot (either empty or a pet item)
        /// </summary>
        internal bool Valid(Item item)
        {
            if (item.ModItem is MiniGun miniGun)
            {
                if (miniGun.LeftHand || miniGun.TwoHands)
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
                SaveUI();
            }
        }

        public void OpenUI(GunHolster gunHolster, BaseScorpionItem scorpionItem)
        {
            _gunHolster = gunHolster;
            _scorpionItem = scorpionItem;
            if (scorpionIndex == -1)
            {
                Item = gunHolster.LeftHand.Clone();
            }
            else
            {
                Item = _scorpionItem.leftHandedGuns[scorpionIndex].Clone();
            }
        }

        public void SaveUI()
        {
            if (scorpionIndex == -1)
            {
                _gunHolster.LeftHand = Item.Clone();
                _gunHolster.Item.NetStateChanged();
            }
            else
            {
                _scorpionItem.leftHandedGuns[scorpionIndex] = Item.Clone();
                _scorpionItem.Item.NetStateChanged();
            }
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
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

            Texture2D backingTexture = ModContent.Request<Texture2D>($"{GunHolsterUISystem.RootTexturePath}LeftSlot").Value;
            int offset = (int)(backingTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            centerPos.Y -= 4;
            spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);

            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale * 2f, 32, Color.White);
            Main.inventoryScale = oldScale;
        }
    }
}
