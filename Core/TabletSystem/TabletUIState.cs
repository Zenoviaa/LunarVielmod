using Stellamod.Common.UI;
using Terraria;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TabletSystem
{
    public class TabletUIState : UIState
    {
        public TabletUI tabletUI;
        public CommonBackButton backButton;
        public TabletUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            tabletUI = new TabletUI();
            Append(tabletUI);

            backButton = new CommonBackButton(ModContent.GetInstance<TabletUISystem>().CloseUI);
            Append(backButton);
//            Append(backButton);

        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            backButton.Left.Pixels = Main.screenWidth / 2 - backButton.Width.Pixels / 2;
            backButton.Top.Pixels = Main.screenHeight / 2 - backButton.Height.Pixels / 2;
            backButton.Top.Pixels += tabletUI.Height.Pixels / 2 + 32;
        }
    }
}
