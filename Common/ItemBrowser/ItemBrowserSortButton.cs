using Microsoft.Xna.Framework;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Stellamod.Common.ItemBrowser
{
    public class ItemBrowserSortButton : UIPanel
    {
        private ItemBrowserTabMenu _menu;
        private Category _category;
        private string _displayName;
        private UIText _displayText;
        public ItemBrowserSortButton(ItemBrowserTabMenu menu, Category category) : base()
        {
            _menu = menu;
            _category = category;
            _displayName = category.displayName;
            _displayText = new UIText(_displayName, 0.7f);
            OnLeftClick += SetSorting;
            OnRightClick += UnSetSorting;
        }

        private void SetSorting(UIMouseEvent evt, UIElement listeningElement)
        {
            _menu.SetCategory(_category);
        }
        private void UnSetSorting(UIMouseEvent evt, UIElement listeningElement)
        {
            Category child = _menu.Category;
            if (child.subCategories.Length == 0)
                child = child.parentCategory;
            Category parent = child.parentCategory;
            _menu.SetCategory(parent);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 80;
            Height.Pixels = 32;
            _displayText.HAlign = 0.5f;
            _displayText.VAlign = 0.5f;
            _displayText.DynamicallyScaleDownToWidth = true;
            _displayText.Width.Pixels = Width.Pixels;
            _displayText.Height.Pixels = Height.Pixels;
            _displayText.SetText(_displayName);
            Append(_displayText);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (_menu.Category.parentCategory == _category)
            {
                _displayText.TextColor = Color.Green;
            }
            else if (_menu.Category == _category)
            {
                _displayText.TextColor = Color.Yellow;
            }
            else
            {

                _displayText.TextColor = Color.Lerp(Color.White, Color.Black, 0.6f);
            }
           
            if(IsMouseHovering && !Main.LocalPlayer.mouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
        }
    }
}
