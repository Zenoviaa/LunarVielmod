using Terraria.UI;

namespace Stellamod.Core.ArmorShop.UI
{
    internal class ArmorShopUIState : UIState
    {
        public ArmorShopUI ui;
        public ArmorShopUIState() : base()
        {

        }

        public override void OnInitialize()
        {
            ui = new ArmorShopUI();
            Append(ui);
        }
    }
}
