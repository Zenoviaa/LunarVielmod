using Terraria;
using Terraria.UI;

namespace Stellamod.Common.DevTools;

public class DevToolHotbarUIState : UIState
{
    public DevToolHotbarUIState()
    {
        Panel = new();
        TimePanel = new();
    }
    public readonly DevToolTimeSliderUIPanel TimePanel;
    public readonly DevToolHotbarUIPanel Panel;
    public override void OnInitialize()
    {
        base.OnInitialize();
        Append(Panel);
        Append(TimePanel);
    }
    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);

        TimePanel.Left.Pixels = Main.screenWidth - TimePanel.Width.Pixels - 450;
        TimePanel.Top.Pixels = 66;
        if (!DevTools.showTimeSlider)
            TimePanel.Top.Pixels = 999999;
    }
}
