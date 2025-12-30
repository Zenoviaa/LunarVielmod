using Microsoft.Xna.Framework;
using Stellamod.Core.Bases;
using Stellamod.Core.MagicSystem.UI;
using Stellamod.Core.SwingSystem;
using Stellamod.Core.XixianFlaskSystem;
using Stellamod.Items;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.ItemBrowser
{
    public class ItemBrowserTabMenu : UIPanel
    {
        private ItemBrowserMenu _menu;
        private InventoryBackground _inventoryBackground;
        private UIGrid _grid;
        private UIPanel _panel;
        private UIScrollbar _scrollbar;
        private UIList _uiList;
        private bool _needsCalculate;
        public ItemBrowserTabMenu(ItemBrowserMenu menu, UIScrollbar scrollbar)
        {
            _menu = menu;
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
        public Category Category;
        public Category LastParentCategory;
        public void SetCategory(Category category)
        {

            if (Category == category)
                return;
            Category = category;
            //Console.WriteLine($"Set Category to {category.displayName}");
     
            _menu.AddElements(category);
       
            if (category == null || category.subCategories.Length > 0)
            {
           
                _grid.Clear();
                Recalculate();
            }

        }


        public override void Recalculate()
        {
            base.Recalculate();
            if (Main.gameMenu)
                return;
            if(_grid.Count == 0)
            {
                if (Category == null)
                {
                    SetCategory(ItemCategoryUtility.All);
                }
                else if (Category != null)
                {
           
                    foreach (Category category in Category.GetCategories())
                    {
                        ItemBrowserSortButton btn = new ItemBrowserSortButton(this, category);
                        btn.Activate();
                        _grid.Add(btn);
                    }
                }
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
 
            _panel.Height.Pixels = _grid.GetTotalHeight() + 32;
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
                _scrollbar.Top.Set(0, 0.55f);
            }
            _scrollbar.Left.Set(0, 0.83f);

            _grid.ListPadding = 16;
        }
    }
}
