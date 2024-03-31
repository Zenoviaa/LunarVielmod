using Terraria.UI;

namespace Stellamod.UI.Scripture
{
    public class ScripturePanel : UIState
    {
        public ScripturePopup Popup { get; private set; }

        public override void OnInitialize()
        {
            Popup = new ScripturePopup();
            Append(Popup);
        }
    }
}
