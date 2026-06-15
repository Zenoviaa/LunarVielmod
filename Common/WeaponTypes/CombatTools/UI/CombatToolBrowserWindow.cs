using Stellamod.Common.ClassReworkSystem.AmmoRework.UI;
using Stellamod.Helpers;
using Stellamod.UI;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader;

namespace Stellamod.Common.WeaponTypes.CombatTools.UI;


/// <summary>
/// The full window of the item browser
/// </summary>
public class CombatToolBrowserWindow : UIPanel
{

    public CombatToolBrowserWindow() : base()
    {
        InventoryMenu = new();
    }

    public CombatToolBrowserMenu InventoryMenu { get; private set; }
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


        InventoryMenu.HAlign = 0.5f;
        InventoryMenu.VAlign = 0.5f;
        Append(InventoryMenu);
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
        Left.Pixels = 0;
        Top.Pixels = RelativeTop;
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        Width.Pixels = InventoryMenu.Width.Pixels;
        Height.Pixels = InventoryMenu.Height.Pixels;
        InventoryMenu.HAlign = 0.5f;
        InventoryMenu.VAlign = 0.25f;
        SetPos();
    }

}
