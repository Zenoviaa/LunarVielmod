using Terraria;
using Terraria.UI;

namespace Stellamod.Common.DevTools;

public class DevToolHotbarUIState : UIState
{
    public DevToolHotbarUIState()
    {
        Panel = new();
        TimePanel = new();
        FlagPanel = new();
    }
    public readonly DevToolTimeSliderUIPanel TimePanel;
    public readonly DevToolHotbarUIPanel Panel;
    public readonly DevToolFlagUIPanel FlagPanel;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Append(Panel);
        Append(TimePanel);
        Append(FlagPanel);
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        TimePanel.Left.Pixels = Main.screenWidth /2 - TimePanel.Width.Pixels / 2;
        TimePanel.Top.Pixels = Main.screenHeight - 180;
        if (!DevTools.showTimeSlider)
            TimePanel.Top.Pixels = 999999;

        FlagPanel.Left.Pixels = Main.screenWidth / 2 - FlagPanel.Width.Pixels / 2f;
        FlagPanel.Top.Pixels = Main.screenHeight / 2 - FlagPanel.Height.Pixels / 2f;
        if (!DevTools.showFlagPanel)
        {
            FlagPanel.Top.Pixels = 999999;
        }
    }
}
