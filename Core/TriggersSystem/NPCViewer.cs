using log4net.Filter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Microsoft.Xna.Framework.Input;
using ReLogic.Content;
using Stellamod.Common.BossBannerSystem;
using Stellamod.Common.ItemBrowser;
using Stellamod.Core.NPCHelpers;
using Stellamod.Core.StructureSelector;
using Stellamod.UI;
using System;
using System.Collections.Generic;
using System.Linq;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;
using Terraria.UI.Chat;

namespace Stellamod.Core.TriggersSystem;

/// <summary>
/// Setups a view that lets you look over a massive grid of items
/// </summary>
public class NPCBrowserView : UIPanel
{
    private float _scale;
    private bool _oldModFilter;
    private string _oldSearchFilter;
    //Basically, instead of ceratgin 6800 slots or whatever
    //We have a single view that takes an array of items
    //Uses that to calculate draw offsets for each item and draws them
    public NPCBrowserView(NPC[] items, Action<NPC> onClick)
    {
        _scale = 1f;
        ElementsPerRow = 9;

        //Set up the items we're going to iterate over
        Items = items;

        //Setup mouse interactions
        OnLeftClick += SpawnItem;
        OnClick = onClick;
        Width.Set(32, 0f);
        Height.Set(32, 0f);
    }

    public Action<NPC> OnClick;
    public NPC[] Items;
    public NPC[] SearchFilterItems;
    public NPC HoveringItem;
    public string SearchFilter;
    public bool ModFilter;
    public float ViewPosition;
    public int ElementsPerRow;

    public bool BossFilter;
    private void SpawnItem(UIMouseEvent evt, UIElement listeningElement)
    {
        OnClick(HoveringItem);
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    private bool NeedsUpdateCollection()
    {
        return _oldSearchFilter != SearchFilter || _oldModFilter != ModFilter || _force;
    }
    private void UpdateCollection()
    {
        IEnumerable<NPC> collection = Items;
        string filter = string.Empty;
        if (!string.IsNullOrEmpty(SearchFilter))
        {
            filter = SearchFilter.TrimStart().ToLower();
            collection = collection.Where(x => x.FullName.ToLower().Contains(filter));
        }

        if (ModFilter)
        {
            collection = collection.Where(x => x.ModNPC != null && x.ModNPC.Mod == Stellamod.Instance);
        }

        if (BossFilter)
        {
            collection = collection.Where(x => x.boss);
        }

        _force = false;
        SearchFilterItems = collection.ToArray();
        _oldSearchFilter = SearchFilter;
        _oldModFilter = ModFilter;
    }

    bool _force;
    public void ForceRefresh()
    {
        _force = true;
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


        Vector2 topLeft = rectangle.TopLeft();
        topLeft += new Vector2(24);

        float availableWidth = GetInnerDimensions().Width;
        float listPadding = 12;

        var size = 16;
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

        ElementsPerRow = 1;
        //We only want to draw the items that are actually in view
        //So we should calculate a starting inde


        //Define our width variables

        if (NeedsUpdateCollection())
        {
            UpdateCollection();
        }
        NPC[] itemArr = SearchFilterItems;
        int elementsPerRow = ElementsPerRow;


        float elementWidth = outerDimensions.Width;
        float viewWidth = availableWidth;
        float elementHeight = outerDimensions.Height;

        //Calculate the maximum height of the grid
        int itemRows = (itemArr.Length / elementsPerRow);
        float maximumHeight = itemRows * (elementHeight + listPadding);
        Height.Pixels = maximumHeight + 128;

        //The view position is the y offset of the scrollbar
        //So to figure out where to start from
        //We just divide the offset by 
        //Caculate a starting and ending index for which items to draw
        int numRowsDownward = (int)(ViewPosition / (elementHeight + listPadding));
        int startIndex = numRowsDownward * elementsPerRow;
        int endIndex = startIndex + elementsPerRow * 16;

        startIndex -= 16;
        if (startIndex <= 0)
            startIndex = 0;


        //Now we're only loading the items that are in view! Yippee! Optimization!
        for (int i = startIndex; i < endIndex && i < itemArr.Length; i++)
        {
            NPC item = itemArr[i];
            float topOffset = i * (elementHeight + listPadding);

            //Enchantment Card
            Vector2 tl = topLeft;
            tl.Y += topOffset;
            var color = Color.White;
            var scale = 1f;
            //Check if hovering for tooltip
            Rectangle hoverRectangle = new Rectangle((int)tl.X, (int)tl.Y, (int)128, (int)size);
            if (hoverRectangle.Contains(mousePoint))
            {
                scale = 1.2f;
                HoveringItem = item;
                color = Color.Yellow;
            }

            ChatManager.DrawColorCodedStringWithShadow(spriteBatch,
                FontAssets.MouseText.Value, item.FullName, tl, color, 0, Vector2.Zero, Vector2.One * scale);

  
        }

        Main.inventoryScale = oldScale;
    }
}


public class NPCViewerUIState : UIState
{
    private UIInputTextField _textBox;
    private readonly Action<NPC> _onSelect;
    public NPCViewer ui;
    public FancyScrollbar scrollbar;

    public NPCBossButton bossButton;
    public NPCViewerUIState(Action<NPC> onSelect) : base()
    {
        _onSelect = onSelect;
    }

    public override void OnInitialize()
    {
        scrollbar = new FancyScrollbar();
        ui = new NPCViewer(_onSelect, scrollbar);
        Append(ui);
     
        Append(scrollbar);

        _textBox = new UIInputTextField("Search...");
        _textBox.OnUpdateText += (string text) =>
        {
            ui.SetFilter(text);
        };
        Append(_textBox);

        bossButton = new(ui);
        Append(bossButton);
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _textBox.Left.Pixels = Main.screenWidth / 2 - _textBox.Width.Pixels / 2;
        _textBox.Top.Pixels = Main.screenHeight / 2 - _textBox.Height.Pixels / 2 - 252;

        bossButton.Left.Pixels = Main.screenWidth / 2 - bossButton.Width.Pixels / 2 - 80;
        bossButton.Top.Pixels = Main.screenHeight / 2 - bossButton.Height.Pixels / 2 - 236;
        bossButton.Width.Pixels = 64;
        bossButton.Height.Pixels=32;
        scrollbar.Left.Pixels = Main.screenWidth / 2 + ui.Width.Pixels / 2 - 36;
        scrollbar.Top.Pixels += 262;
        scrollbar.Height.Pixels = 380;

        _textBox.Width.Pixels = 80;
    }
}

public class NPCBossButton : UIPanel
{
    public NPCBossButton(NPCViewer viewer)
    {
        Viewer = viewer;
    }
    public readonly NPCViewer Viewer;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 48;
        Height.Pixels = 24;
        OnLeftClick += ClickBoss;
    }

    private void ClickBoss(UIMouseEvent evt, UIElement listeningElement)
    {
        Viewer.ToggleBosses();
        SoundEngine.PlaySound(SoundID.MenuTick);
    }

    protected override void DrawSelf(SpriteBatch spriteBatch)
    {
        base.DrawSelf(spriteBatch);
        this.QuickMouseInteraction();
        var rect = GetDimensions().ToRectangle();
        var topLeft = rect.TopLeft();
        ChatManager.DrawColorCodedStringWithShadow(
            spriteBatch,
            FontAssets.MouseText.Value, "Boss", 
            topLeft + new Vector2(8),
            Viewer.onlyShowBosses ? Color.Yellow : Color.White,
            0, 
            Vector2.Zero,
            Vector2.One);
    
    }
}
public class NPCViewer : UIPanel
{
    private readonly Action<NPC> _onSelect;
    private UIList _uiList;
    private UIPanel _panel;
    private UIGrid _grid;
    private UIScrollbar _scrollbar;
    private NPCBrowserView _view;
    private readonly NPC[] _allNpcs;
    public NPCViewer(Action<NPC> onSelect, UIScrollbar scrollbar)
    {
        _onSelect = onSelect;

        _panel = new UIPanel();
        _grid = new UIGrid();
        _scrollbar = scrollbar;
        _uiList = new UIList();
        _allNpcs = GetNPCs();
        FilterText = string.Empty;
    }


    public int RelativeLeft => Main.screenWidth / 2 - (int)Width.Pixels / 2;
    public int RelativeTop => Main.screenHeight / 2 - (int)Height.Pixels / 2;

    public string FilterText;
    public bool onlyShowBosses;
    public bool onlyShowMod;
    public void ToggleBosses()
    {
        onlyShowBosses = !onlyShowBosses;
        _view.BossFilter = onlyShowBosses;
        _view.ForceRefresh();
    }
    public void SetFilter(string text)
    {
        FilterText = text;
        _view.SearchFilter = FilterText;
        _view.ForceRefresh();
    }
    public static NPC[] GetNPCs()
    {
        var npcList = new List<NPC>();
        for (int i = 0; i < NPCLoader.NPCCount; i++)
        {
            NPC npc = new NPC();
            npc.SetDefaults(i);
            npcList.Add(npc);
        }

        npcList = npcList.OrderBy(n => n.FullName).ToList();
        return npcList.ToArray();
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 600;
        Height.Pixels = 400;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Black * 0.5f;
        BorderColor = new Color(135, 97, 65) * 0.2f;

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

        _scrollbar.Width.Set(20, 0);
        _scrollbar.Height.Set(340, 0);
        _scrollbar.Top.Set(0, 0f);

        float maxViewSize = 48 * 8f;
        _scrollbar.SetView(0, maxViewSize);
        Refresh();
    }
    private void Refresh()
    {
        if (Main.gameMenu)
            return;
        _grid.Clear();

        _view = new(_allNpcs, _onSelect);
        _view.SearchFilter = FilterText;
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
        _panel.Width.Pixels = 350;
        Width.Pixels = 500;
        //Constantly lock the UI in the position regardless of resolution changes
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        _panel.Height.Pixels = _view.Height.Pixels;
        float progress = _panel.Height.Pixels / Height.Pixels;
        progress = MathHelper.Clamp(progress, 0f, 1f);

        _scrollbar.Height.Set(Height.Pixels * progress, 0);
        float scrollRatio = _scrollbar.ViewPosition;

        if (_view != null)
        {
            _view.ModFilter = onlyShowBosses;
            _view.ViewPosition = scrollRatio;
        }

        //Hacky way to get invisible scrollbar when there's no need for it
        if (_panel.Height.Pixels < Height.Pixels)
        {
            _scrollbar.Top.Set(500000, 0f);
        }
        else
        {
            _scrollbar.Top.Set(0, 0f);
        }
    }
}
