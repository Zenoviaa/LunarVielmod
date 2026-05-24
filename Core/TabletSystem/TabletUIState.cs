using Stellamod.Common.UI;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.Core.TabletSystem
{
    public class TabletUIState : UIState
    {
        public TabletUI tabletUI;
        public TabletUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            tabletUI = new TabletUI();
            Append(tabletUI);


        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

        }
    }
}
