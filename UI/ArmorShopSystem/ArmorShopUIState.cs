using Terraria.UI;

namespace Stellamod.UI.ArmorShopSystem
{
    public class ArmorShopUIState : UIState
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
