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
    internal class GunHolsterRightSlot : UIElement
    {
        private BaseScorpionItem _scorpionItem;
        private GunHolster _gunHolster;
        private Item _prevItem;
        private readonly int _context;
        private readonly float _scale;

        internal Item Item;

        internal GunHolsterRightSlot(int context = ItemSlot.Context.InventoryItem, float scale = 1f)
        {
            _context = context;
            _scale = scale;
            var asset = ModContent.Request<Texture2D>(
                $"{GunHolsterUISystem.RootTexturePath}RightSlot", ReLogic.Content.AssetRequestMode.ImmediateLoad);

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
                SaveUI();
            }
        }

        public void OpenUI(GunHolster gunHolster, BaseScorpionItem scorpionItem)
        {
            _gunHolster = gunHolster;
            _scorpionItem = scorpionItem;
            if (scorpionIndex == -1)
            {
                Item = gunHolster.RightHand.Clone();
            }
            else
            {
                Item = _scorpionItem.rightHandedGuns[scorpionIndex].Clone();
            }
        }

        public void SaveUI()
        {
            if (scorpionIndex == -1)
            {
                _gunHolster.RightHand = Item.Clone();
                _gunHolster.Item.NetStateChanged();
            }
            else
            {
                _scorpionItem.rightHandedGuns[scorpionIndex] = Item.Clone();
                _scorpionItem.Item.NetStateChanged();
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
            Color drawColor = Color.White;

            Vector2 pos = rectangle.TopLeft();
            if (scorpionIndex != -1)
            {
                Player player = Main.LocalPlayer;
                float remainingSlots = player.maxMinions - player.slotsMinions;
                bool canFire = (scorpionIndex + _scorpionItem.leftHandedGuns.Count + 1) <= remainingSlots;
                if (!canFire)
                {
                    drawColor = drawColor.MultiplyRGB(Color.DarkGray);
                    color2 = color2.MultiplyRGB(Color.DarkGray);
                }
            }

            Texture2D backingTexture = ModContent.Request<Texture2D>($"{GunHolsterUISystem.RootTexturePath}RightSlot").Value;
            int offset = (int)(backingTexture.Size().Y / 2);
            Vector2 centerPos = pos + rectangle.Size() / 2f;
            centerPos.Y -= 4;
            spriteBatch.Draw(backingTexture, rectangle.TopLeft(), null, color2, 0f, default(Vector2), _scale, SpriteEffects.None, 0f);


            ItemSlot.DrawItemIcon(Item, _context, spriteBatch, centerPos, _scale * 2f, 32, drawColor);
            Main.inventoryScale = oldScale;
        }
    }
}
