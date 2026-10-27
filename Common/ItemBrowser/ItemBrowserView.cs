using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Helpers;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Common.ItemBrowser
{
    /// <summary>
    /// Setups a view that lets you look over a massive grid of items
    /// </summary>
    public class ItemBrowserView : UIPanel
    {
        private float _scale;
        private int _context;
        private bool _oldModFilter;
        private string _oldSearchFilter;
        //Basically, instead of ceratgin 6800 slots or whatever
        //We have a single view that takes an array of items
        //Uses that to calculate draw offsets for each item and draws them
        public ItemBrowserView(Item[] items)
        {
            _scale = 1f;
            _context = ItemSlot.Context.BankItem;
            ElementsPerRow = 9;

            //Set up the items we're going to iterate over
            Items = items;
            HoveringItem = new Item();
            HoveringItem.SetDefaults(ItemID.None);

            //Setup mouse interactions
            OnLeftClick += SpawnItem;
            OnRightClick += SpawnItemMaxStack;

            //Setup drawing
            string texturePath = this.GetType().DirectoryHere() + "/ItemBrowserSlot";
            SlotTextureAsset = ModContent.Request<Texture2D>(texturePath, AssetRequestMode.AsyncLoad);
            Width.Set(32, 0f);
            Height.Set(32, 0f);
        }

        public Item[] Items;
        public Item[] SearchFilterItems;
        public Item HoveringItem;
        public Asset<Texture2D> SlotTextureAsset;
        public string SearchFilter;
        public bool ModFilter;
        public float ViewPosition;
        public int ElementsPerRow;
        private void SpawnItemMaxStack(UIMouseEvent evt, UIElement listeningElement)
        {
            if (InputHelper.KeyDown(Keys.LeftShift))
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), HoveringItem.type);
            }
            else
            {
                var item = new Item(HoveringItem.type);
                Main.mouseItem = item;
            }

        }

        private void SpawnItem(UIMouseEvent evt, UIElement listeningElement)
        {

            if (InputHelper.KeyDown(Keys.LeftShift))
            {
                Main.LocalPlayer.QuickSpawnItem(Main.LocalPlayer.GetSource_FromThis(), HoveringItem.type, HoveringItem.maxStack);
            }
            else
            {
                var item = new Item(HoveringItem.type);
                Main.mouseItem = item;
                Main.mouseItem.stack = item.maxStack;
            }
        }

        private bool NeedsUpdateCollection()
        {
            return _oldSearchFilter != SearchFilter || _oldModFilter != ModFilter;
        }
        private void UpdateCollection()
        {
            IEnumerable<Item> collection = Items;
            string filter = string.Empty;
            if (!string.IsNullOrEmpty(SearchFilter))
            {
                filter = SearchFilter.TrimStart().ToLower();
                collection = collection.Where(x => x.Name.ToLower().Contains(filter));
            }

            if (ModFilter)
            {
                collection = collection.Where(x => x.ModItem != null && x.ModItem.Mod == Stellamod.Instance);
            }

            SearchFilterItems = collection.ToArray();
            _oldSearchFilter = SearchFilter;
            _oldModFilter = ModFilter;
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
            topLeft += new Vector2(24);
     
            float availableWidth = GetInnerDimensions().Width;
            float listPadding = 12;

            var size = 32;
            Rectangle outerDimensions = new Rectangle(0, 0, size, size);
            Point mousePoint = Main.MouseScreen.ToPoint();
            string filter = string.Empty;
            if (!string.IsNullOrEmpty(SearchFilter))
                filter = SearchFilter.TrimStart().ToLower();
            bool useFilter = !string.IsNullOrEmpty(filter);

            //We're basically just reusing the grid code here lol
            //There's currently 9 items per row
            //To optimize this, we can calculate the placement of an element with some simple math based on its index
            //Instead of using left and top variables
            //So let's do that

            ElementsPerRow = 8;
            //We only want to draw the items that are actually in view
            //So we should calculate a starting inde


            //Define our width variables

            if (NeedsUpdateCollection())
            {
                UpdateCollection();
            }
            Item[] itemArr = SearchFilterItems;
            int elementsPerRow = ElementsPerRow;
          

            float elementWidth = outerDimensions.Width;
            float viewWidth = availableWidth;
            float elementHeight = outerDimensions.Height;

            //Calculate the maximum height of the grid
            int itemRows = (itemArr.Length / elementsPerRow);
            float maximumHeight = itemRows * (elementHeight + listPadding);
            Height.Pixels = maximumHeight + 128;

            Texture2D slotTexture = SlotTextureAsset.Value;
            Color drawColor = Color.Lerp(Color.White, Color.Black, 0.5f);
            float drawScale = 1.2f;
            Vector2 drawOrigin = slotTexture.Size() / 2;

            //The view position is the y offset of the scrollbar
            //So to figure out where to start from
            //We just divide the offset by 
            //Caculate a starting and ending index for which items to draw
            int numRowsDownward = (int)(ViewPosition / (elementHeight + listPadding));
            int startIndex = numRowsDownward * elementsPerRow;
            int endIndex = startIndex + elementsPerRow * 12;

            startIndex -= 16;
            if (startIndex <= 0)
                startIndex = 0;

    
            //Now we're only loading the items that are in view! Yippee! Optimization!
            for (int i = startIndex; i < endIndex && i < itemArr.Length; i++)
            {
                Item item = itemArr[i];
                //Asynchronous load if it's not loaded to avoid lag spikes
                //Draw functions will force immediate load otherwise
                if (TextureAssets.Item[item.type].State == AssetState.NotLoaded)
                    Main.Assets.Request<Texture2D>(TextureAssets.Item[item.type].Name, AssetRequestMode.AsyncLoad);
                //Remmeber 9 elements per row
                //We can use the modulus operator to get this to keep looping, since all elements are the same size
                float leftOffset = i % elementsPerRow * (elementWidth + listPadding);
                float topOffset = i / elementsPerRow * (elementHeight + listPadding);

                //Enchantment Card
                Vector2 tl = topLeft;
                tl.X += leftOffset;
                tl.Y += topOffset;
                Vector2 centerPos = tl + new Vector2(16);

                Vector2 iconCenterPos = tl + slotTexture.Size() / 2;
                spriteBatch.Draw(slotTexture, iconCenterPos, null, drawColor, 0f, drawOrigin, _scale * 1.5F, SpriteEffects.None, 0f);
                ItemSlot.DrawItemIcon(item, _context, spriteBatch, centerPos, drawScale * 0.9f, size, Color.White);
                if (HoveringItem.stack > 1)
                {
                    ChatManager.DrawColorCodedStringWithShadow(spriteBatch, FontAssets.ItemStack.Value, item.stack.ToString(),
                        centerPos + new Vector2(0, 2) * _scale, Color.White, 0f, Vector2.Zero, new Vector2(_scale), -1f, _scale);
                }

                //Check if hovering for tooltip
                Rectangle hoverRectangle = new Rectangle((int)tl.X, (int)tl.Y, (int)size, (int)size);
                if (hoverRectangle.Contains(mousePoint))
                {
                    HoveringItem = item;
                    Main.HoverItem = item;
                    Main.hoverItemName = item.HoverName;
                }
            }

            Main.inventoryScale = oldScale;
        }
    }
}
