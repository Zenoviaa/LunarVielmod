using ReLogic.Content;
using Stellamod.UI;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.XixianFlaskSystem.UI;

public class InsourceInventoryMenu : UIElement
{
    private UIPanel _panel;
    private UIScrollbar _scrollbar;
    private UIImage _background;
    private UIList _uiList;
    private InsourceBrowserView _view;
    public InsourceInventoryMenu(UIScrollbar scrollbar)
    {
        Asset<Texture2D> backgroundTexture =
            ModContent.Request<Texture2D>(XixianFlaskUISystem.RootTexturePath + "InsourceInventoryPanel");
        _background = new UIImage(backgroundTexture);
        _panel = new UIPanel();
        _scrollbar = scrollbar;
        _uiList = new UIList();
        _view = new InsourceBrowserView();
    }

    public override void OnActivate()
    {
        base.OnActivate();
        _view.SetCollection(ItemHelper.Insources);
    }

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 118;
        Height.Pixels = 216;
        Append(_background);

        _scrollbar.Width.Set(20, 0);
        _scrollbar.Height.Set(340, 0);
        _scrollbar.Left.Set(0, 1);
        _scrollbar.Top.Set(0, 0f);

        float maxViewSize = 48 * 8f;
        _scrollbar.SetView(0, maxViewSize);

        _panel.Width.Pixels = Width.Pixels;
        _panel.Height.Pixels = Height.Pixels;
        _panel.BackgroundColor = Color.Transparent;
        _panel.BorderColor = Color.Transparent;
        Append(_panel);

        _view.ElementsPerRow = 2;
        _view.Width.Pixels = 80;
        _view.Height.Pixels = Height.Pixels;
        _panel.Append(_view);

        _uiList.Width.Pixels = Width.Pixels;
        _uiList.Height.Pixels = Height.Pixels;
        _uiList.Add(_panel);
        _uiList.SetScrollbar(_scrollbar);
        Append(_uiList);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        _panel.Height.Pixels = _view.Height.Pixels + 32;
        float progress = _panel.Height.Pixels / Height.Pixels;
        progress = MathHelper.Clamp(progress, 0f, 1f);

        _scrollbar.Height.Set(Height.Pixels * progress * 0.9f, 0);
        float scrollRatio = _scrollbar.ViewPosition;

        _view.Left.Pixels = 20;
        _view.Top.Pixels = 8;
        _view.ViewPosition = scrollRatio;

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
