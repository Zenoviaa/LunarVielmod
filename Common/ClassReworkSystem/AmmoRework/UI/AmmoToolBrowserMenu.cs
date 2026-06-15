using ReLogic.Content;
using Stellamod.Common.MagicSystem.UI;
using Stellamod.Common.UI;
using Stellamod.Core;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework.UI;

/// <summary>
/// Creates a menu of all the items in the mod
/// </summary>
public class AmmoToolBrowserMenu : UIPanel
{
    private readonly Asset<Texture2D> _slotTextureAsset;
    private InventoryBackground _inventoryBackground;
    private UIGrid _grid;
    private UIPanel _panel;
    private UIScrollbar _scrollbar;
    private UIList _uiList;
    private GridItemBrowserView _view;
    private XButton _xButton;
    public AmmoToolBrowserMenu(UIScrollbar scrollbar)
    {
        _slotTextureAsset = AssetReferences.Common.WeaponTypes.CombatToolSlot.Asset;
        _xButton = new XButton(Close);
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
        //  Append(_xButton);

    }
    private string _lastSearchFilter;
    public void SetSearchFilter(string searchFilter)
    {
        //Set the text filter for items
        if (_lastSearchFilter == searchFilter)
            return;
        _lastSearchFilter = searchFilter;
        Refresh();
    }

    private void Refresh()
    {
        if (Main.gameMenu)
            return;


        _grid.Clear();

       
        var items = ModContent.GetContent<ModItem>();
        List<Item> itemList = new List<Item>();
        for (int i = 0; i < ItemLoader.ItemCount; i++)
        {
            Item item = new Item(i);
            if (item.ammo == AmmoID.None)
                continue;
            itemList.Add(new Item(i));
            //    ItemSearchInnerLoop(category, item, result);
        }
        _view = new(itemList.ToArray(), _slotTextureAsset, SelectCombatTool, ViewCombatTool);
        _view.SearchFilter = _lastSearchFilter;
        _view.Width.Pixels = Width.Pixels;
        _view.Height.Pixels = Height.Pixels;
        _view.Activate();
        _grid.Add(_view);

        _grid.Recalculate();
        base.Recalculate();
    }


    private void SelectCombatTool(Item item)
    {
        ClassReworkPlayer combatToolPlayer = Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>();
        if (!combatToolPlayer.HasAmmo(item))
            return;

        combatToolPlayer.QuiverAmmoItem = item.Clone();
        combatToolPlayer.QuiverAmmoItem.stack = 9999;
    }

    private bool ViewCombatTool(Item item)
    {
        return Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>().HasAmmo(item);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        if (_view == null)
            Refresh();
        _xButton.Left.Pixels = 0;
        _xButton.Top.Pixels = 0;
        _panel.Height.Pixels = _view.Height.Pixels + 32;
        float progress = _panel.Height.Pixels / Height.Pixels;
        progress = MathHelper.Clamp(progress, 0f, 1f);

        _inventoryBackground.drawColor = Color.Lerp(Color.White, Color.Black, 0.8f);
        _scrollbar.Height.Set(Height.Pixels * progress, 0);
        float scrollRatio = _scrollbar.ViewPosition;

        if (_view != null)
        {
            _view.ViewPosition = scrollRatio;
        }


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

    private void Close()
    {
        AmmoToolUISystem uiSystem = ModContent.GetInstance<AmmoToolUISystem>();
        uiSystem.CloseUI();
    }
}
