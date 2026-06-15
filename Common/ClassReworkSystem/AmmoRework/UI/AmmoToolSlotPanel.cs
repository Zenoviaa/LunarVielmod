using Stellamod.Common.UI;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Common.ClassReworkSystem.AmmoRework.UI;

public class AmmoToolSlotPanel : UIPanel
{
    private Ability _ability;
    private UIPanel _panel;
    public AmmoToolSlot slot;

    public override void OnInitialize()
    {
        base.OnInitialize();
        Width.Pixels = 48;
        Height.Pixels = 48;
        Left.Pixels = -9999;
        Top.Pixels = -9999;
        BackgroundColor = Color.Transparent;
        BorderColor = Color.Transparent;

        _panel = new UIPanel();
        _panel.Width.Pixels = Width.Pixels;
        _panel.Height.Pixels = Height.Pixels;
        _panel.BackgroundColor = Color.Transparent;
        _panel.BorderColor = Color.Transparent;


        Width.Pixels = _panel.Width.Pixels = 96;
        Height.Pixels = _panel.Height.Pixels = 96;
        Append(_panel);

        slot = new();
        _panel.Append(slot);
        _ability = new Ability(this, 12);
        AbilityTray.TrayItems.Add(_ability);
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        ClassReworkPlayer classReworkPlayer = Main.LocalPlayer.GetModPlayer<ClassReworkPlayer>();
        if (classReworkPlayer.playerClass != PlayerClass.Ranger &&
            classReworkPlayer.playerClass != PlayerClass.Omni &&
            classReworkPlayer.playerClass != PlayerClass.God)
        {
            _ability.isActive = false;
        }
        else
        {
            _ability.isActive = true;
        }
    }
}
