using Stellamod.Common.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Common.ArmorReforgeSystem;

public class ReforgeUIState : UIState
{
    public ReforgeUI ui;
    public CommonBackButton backButton;
    public ReforgeUIState() : base()
    {

    }

    public override void OnInitialize()
    {


        ui = new ReforgeUI();
        Append(ui);

        backButton = new CommonBackButton(() => ModContent.GetInstance<ReforgeUISystem>().CloseUI());
        backButton.asXButton = true;
        Append(backButton);
    }

    public override void Update(GameTime gameTime)
    {
        base.Update(gameTime);
        int left = Main.screenWidth / 2 - (int)(backButton.Width.Pixels / 2) + 80;
        int top = Main.screenHeight / 2 - (int)(backButton.Height.Pixels / 2) - 184;
        backButton.Left.Pixels = left;
        backButton.Top.Pixels = top;
    }
}
