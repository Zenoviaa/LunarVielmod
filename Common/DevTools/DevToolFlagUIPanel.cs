using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Common.DevTools;

public class DevToolFlagUIPanel : UIPanel
{
    private UIGrid _grid;
    private UIPanel _panel;
    private UIScrollbar _scrollbar;
    private UIList _uiList;
    public DevToolFlagUIPanel()
    {
        Buttons = new();
        _panel = new UIPanel();
        _grid = new UIGrid();
        _scrollbar = new FancyScrollbar();
        _uiList = new UIList();
    }
    public readonly List<DevToolFlagButton> Buttons;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 428;
        Height.Pixels = 236;

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
        _scrollbar.Left.Set(0, 0.95f);
        _scrollbar.Top.Set(0, 0f);

        float maxViewSize = 48 * 8f;
        _scrollbar.SetView(0, maxViewSize);
        Append(_scrollbar);
        RefreshFlags();
    }

    public void RefreshFlags()
    {
        _grid.Clear();
        Buttons.Clear();
        Buttons.Add(new(LocalizationReferences.Mods.Stellamod.Items.Common.SlimeRain.KEY,
            () => Main.slimeRain,
            () => {
                if (Main.slimeRain)
                {
                    Main.StopSlimeRain();
                }
                else
                {
                    Main.StartSlimeRain();
                }
            }));

        foreach (var btn in Buttons)
        {
            _grid.Add(btn);
        }
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _panel.Height.Pixels = _grid.GetTotalHeight() + 32;
        float progress = _panel.Height.Pixels / Height.Pixels;
        progress = MathHelper.Clamp(progress, 0f, 1f);
        _scrollbar.Height.Set((Height.Pixels - 64) * progress, 0);

        //Hacky way to get invisible scrollbar when there's no need for it
        if (_panel.Height.Pixels < Height.Pixels)
        {
            _scrollbar.Top.Set(500000, 0f);
        }
        else
        {
            _scrollbar.Top.Set(0, 0.2f);
        }
    }
}
