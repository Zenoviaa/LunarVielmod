using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;
using static System.Net.Mime.MediaTypeNames;

namespace Stellamod.UI.Scripture
{
    public class ScripturePanel : UIState
    {
        public ScripturePopup Popup { get; private set; }
        public UIText Text { get; private set; }
        public override void OnInitialize()
        {
            UIPanel panel = new UIPanel();
            panel.Width.Set(612, 0);
            panel.Height.Set(768, 0);
            panel.HAlign = panel.VAlign = 0.5f;
            panel.BackgroundColor = Color.Transparent;
            panel.BorderColor = Color.Transparent;
            Append(panel);

            Popup = new ScripturePopup();
            panel.Append(Popup);

            Text = new UIText("Hello world!"); // 1
            Text.IsWrapped = true;
            Text.Width.Set(612, 0);
            Text.Height.Set(768, 0);
            Text.Activate();
            panel.Append(Text);
        }
    }
}
