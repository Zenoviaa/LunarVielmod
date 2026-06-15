using Stellamod.Helpers;
using Stellamod.UI;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework.UI;

/// <summary>
/// The full window of the item browser
/// </summary>
public class AmmoToolBrowserWindow : UIPanel
{
    private UIScrollbar _scrollbar;
    private XButton _xButton;
    private AmmoToolBrowserMenu _inventoryMenu;
    private UIInputTextField _textBox;

    public AmmoToolBrowserWindow() : base()
    {
        _scrollbar = new FancyScrollbar();
        _xButton = new XButton(Close);
        _inventoryMenu = new(_scrollbar);
        _textBox = new UIInputTextField("Search...");
    }

    public string SearchFilter => _textBox.Text;

    public int RelativeLeft => ScreenHelper.TrueScreenWidth / 2 - (int)Width.Pixels / 2;
    public int RelativeTop => ScreenHelper.TrueScreenHeight / 2 - (int)Height.Pixels / 2;

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 704;
        Height.Pixels = 704;
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;


        _inventoryMenu.HAlign = 0.5f;
        _inventoryMenu.VAlign = 0.5f;
        Append(_inventoryMenu);
        Append(_xButton);

        //Scrollbar
        _scrollbar.Width.Set(20, 0);
        _scrollbar.Height.Set(340, 0);
        _scrollbar.Left.Set(0, 0.95f);
        _scrollbar.Top.Set(0, 0f);

        float maxViewSize = 48 * 8f;
        _scrollbar.SetView(0, maxViewSize);
        Append(_scrollbar);

        _textBox.HAlign = 0.5f;
        _textBox.VAlign = 0.1f;
        _textBox.Width.Pixels = 128;
        Append(_textBox);
        SetPos();
    }

    public override void OnDeactivate()
    {
        base.OnDeactivate();
        if (!Main.gameMenu)
        {
            SoundEngine.PlaySound(SoundID.MenuClose);
        }
    }

    private void SetPos()
    {
        Left.Pixels = RelativeLeft;
        Top.Pixels = RelativeTop;

        _inventoryMenu.HAlign = 0.5f;
        _inventoryMenu.VAlign = 0.25f;
        _xButton.Top.Pixels = 64;
        _xButton.Left.Pixels = 164;

    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        //Constantly lock the UI in the position regardless of resolution changes
        _inventoryMenu.SetSearchFilter(SearchFilter);
        SetPos();
    }

    private void Close()
    {
        AmmoToolUISystem uiSystem = ModContent.GetInstance<AmmoToolUISystem>();
        uiSystem.CloseUI();
    }
}
