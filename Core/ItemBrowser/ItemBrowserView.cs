using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Helpers;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;
using static Terraria.GameContent.Animations.IL_Actions.Sprites;

namespace Stellamod.Core.ItemBrowser
{
    public class ItemBrowserView : UIPanel
    {
        private float _scale;
        private int _context;
        //Basically, instead of ceratgin 6800 slots or whatever
        //We have a single view that takes an array of items
        //Uses that to calculate draw offsets for each item and draws them
        public ItemBrowserView(Item[] items)
        {
            _scale = 1f;
            _context = ItemSlot.Context.BankItem;
            HoveringItem = new Item();
            HoveringItem.SetDefaults(0);
            string texturePath = this.GetType().DirectoryHere() + "/ItemBrowserSlot";
            SlotTextureAsset = ModContent.Request<Texture2D>(texturePath, ReLogic.Content.AssetRequestMode.ImmediateLoad);
            Width.Set(32, 0f);
            Height.Set(32, 0f);
            OnLeftClick += SpawnItem;
            OnRightClick += SpawnItemMaxStack;
            this.Items = items;
        }
        public Asset<Texture2D> SlotTextureAsset;
        public string SearchFilter;
        public bool ModFilter;
        public Item[] Items;
        public Item HoveringItem;
        private void SpawnItemMaxStack(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), HoveringItem.type, HoveringItem.maxStack);
        }

        private void SpawnItem(UIMouseEvent evt, UIElement listeningElement)
        {
            Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), HoveringItem.type);
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            float oldScale = Main.inventoryScale;
            Main.inventoryScale = _scale;
            Rectangle rectangle = GetDimensions().ToRectangle();
            if (IsMouseHovering && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (IsMouseHovering)
            {

                Main.HoverItem = HoveringItem;
                Main.hoverItemName = HoveringItem.HoverName;
            }
            Vector2 topLeft = rectangle.TopLeft();
            float availableWidth = GetInnerDimensions().Width;

            float tlPadding = 8;
            float top = tlPadding;
            float left = 0;
            float maxRowHeight = 0f;
            float listPadding = 8;
            Rectangle outerDimensions = new Rectangle(0, 0, 32, 32);
        
            //  Console.WriteLine(availableWidth);
            Point mousePoint = Main.MouseScreen.ToPoint();
            string filter = string.Empty;
            if (!string.IsNullOrEmpty(SearchFilter))
                filter = SearchFilter.TrimStart().ToLower();
            bool useFilter = !string.IsNullOrEmpty(filter);

            //We're basically just reusing the grid code here lol
            for (int i = 0; i < Items.Length; i++)
            {
                Item item = Items[i];
                if (useFilter)
                {
                    string itemLower = item.Name.ToLower();
                    if (!itemLower.Contains(filter))
                        continue;
                }

                if (ModFilter)
                {
                    if (item.ModItem == null)
                        continue;
                    if (item.ModItem.Mod != Stellamod.Instance)
                        continue;
                }

                if (left + outerDimensions.Width > availableWidth && left > 0)
                {
                    top += maxRowHeight + listPadding;
                    left = 0;
                    maxRowHeight = 0;
                }
                maxRowHeight = Math.Max(maxRowHeight, outerDimensions.Height);
                float l = left;
 
                left += outerDimensions.Width + listPadding;
                float t = top;

                //Enchantment Card
                Vector2 tl = topLeft;
                tl.X += l;
                tl.Y += t;
                Vector2 centerPos = tl + new Vector2(16);
                Color color2 = Main.inventoryBack;
                Texture2D slotTexture = SlotTextureAsset.Value;
                Vector2 drawOrigin = slotTexture.Size() / 2;
                Vector2 iconCenterPos = tl + slotTexture.Size() / 2;
                spriteBatch.Draw(slotTexture, iconCenterPos, null, Color.Lerp(Color.White, Color.Black, 0.75f), 0f, drawOrigin, _scale, SpriteEffects.None, 0f);
                ItemSlot.DrawItemIcon(item, _context, spriteBatch, centerPos, _scale * 1.25f, 32, Color.White);
                if (HoveringItem.stack > 1)
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(),
                        centerPos + new Vector2(0, 2) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);

                //Check if hovering for tooltip
                Rectangle hoverRectangle = new Rectangle((int)tl.X, (int)tl.Y, 32, 32);
                if (hoverRectangle.Contains(mousePoint))
                {
                    HoveringItem = item;
                    Main.HoverItem = item;
                    Main.hoverItemName = item.HoverName;

                }
            
            }

            //Add a bit of extra padding so the items don't get clipped
            Height.Pixels = top + 32;



            Main.inventoryScale = oldScale;
        }
    }
}
