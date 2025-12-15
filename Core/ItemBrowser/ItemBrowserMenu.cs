using Microsoft.Xna.Framework;
using Stellamod.Core.MagicSystem.UI;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.ItemBrowser
{
    /// <summary>
    /// Creates a menu of all the items in the mod
    /// </summary>
    public class ItemBrowserMenu : UIPanel
    {
        private InventoryBackground _inventoryBackground;
        private UIGrid _grid;
        private UIPanel _panel;
        private UIScrollbar _scrollbar;
        private UIList _uiList;
        private ItemBrowserView _view;
        public ItemBrowserMenu(UIScrollbar scrollbar)
        {
            _inventoryBackground = new InventoryBackground();
            _panel = new UIPanel();
            _grid = new UIGrid();
            _scrollbar = scrollbar;
            _uiList = new UIList();
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 428;
            Height.Pixels = 236;
            Append(_inventoryBackground);

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _grid.Left.Pixels = 10;
            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0.5f;
            _grid.VAlign = 0.5f;
            _grid.ListPadding = 2;
            _panel.Append(_grid);



            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);


        }
        private Category _lastCategory;
        private string _lastSearchFilter;
        public void SetSearchFilter(string searchFilter)
        {
            //Set the text filter for items
            if (_lastSearchFilter == searchFilter)
                return;
            _lastSearchFilter = searchFilter;
            Refresh();
        }

        public void AddElements(Category category)
        {
            _lastCategory = category;
            Refresh();
        }

        private void Refresh()
        {
            if (Main.gameMenu)
                return;

            if(_lastCategory == null)
            {
                return;
            }

            _grid.Clear();
            Item[] items = _lastCategory.items;
            _view = new ItemBrowserView(items);
            _view.SearchFilter = _lastSearchFilter;
            _view.Width.Pixels = Width.Pixels;
            _view.Height.Pixels = Height.Pixels;
            _view.Activate();
            _grid.Add(_view);

            _grid.Recalculate();
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
       
            _panel.Height.Pixels = _view.Height.Pixels + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            _scrollbar.Height.Set(Height.Pixels * progress, 0);

            //Hacky way to get invisible scrollbar when there's no need for it
            if (_panel.Height.Pixels < Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0, 0.2f);
            }
            _scrollbar.Left.Set(0, 0.83f);

            _grid.ListPadding = 16;
        }

    }
}
