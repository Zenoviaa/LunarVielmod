using Microsoft.Xna.Framework;
using Stellamod.Common.MagicSystem.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.ItemBrowser;

public class ItemBrowserTabMenu : UIPanel
{
    private readonly ItemBrowserMenu _menu;
    private UIGrid _grid;
    private UIPanel _panel;
    private UIScrollbar _scrollbar;
    private UIList _uiList;
    public ItemBrowserTabMenu(ItemBrowserMenu menu, UIScrollbar scrollbar)
    {
        _menu = menu;
        _panel = new UIPanel();
        _grid = new UIGrid();
        _scrollbar = scrollbar;
        _uiList = new UIList();
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 236;
        Height.Pixels = 512;
        BackgroundColor = Color.Black * 0.4f;
        BorderColor = Color.Transparent;
        _panel.Width.Pixels = Width.Pixels;
        _panel.Height.Pixels = Height.Pixels;
        _panel.BackgroundColor = Color.Transparent;
        _panel.BorderColor = Color.Transparent;
        Append(_panel);


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
        if (_grid.Count == 0)
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
        _scrollbar.Height.Pixels = 310;
        //Hacky way to get invisible scrollbar when there's no need for it
        if (_panel.Height.Pixels < Height.Pixels)
        {
            _scrollbar.Top.Set(500000, 0f);
        }
        else
        {
            _scrollbar.Top.Set(0, 0f);
            _scrollbar.Top.Pixels += 16;
        }
        _scrollbar.Left.Set(0, 0f);
        _scrollbar.Left.Pixels = Width.Pixels + 3;

        _grid.ListPadding = 16;
    }
}
