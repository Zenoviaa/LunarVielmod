using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;
using Terraria.UI;

namespace Stellamod.UI.Dialogue
{
    internal class DialoguePanel : UIState
    {
        public DialoguePortrait Portrait { get; private set; }
        public DialogueBox Box { get; private set; }
        public UIText Text { get; private set; }
        public override void OnInitialize()
        {
            UIPanel panel = new UIPanel();
            panel.Width.Set(912, 0);
            panel.Height.Set(261, 0);
            panel.HAlign = 0.5f;
            panel.VAlign = 0.9f;
            panel.BackgroundColor = Color.Transparent;
            panel.BorderColor = Color.Transparent;
            Append(panel);

            Box = new DialogueBox();
            panel.Append(Box);

            Portrait = new DialoguePortrait();
            panel.Append(Portrait);

            Text = new UIText("Hello world!"); // 1
            Text.IsWrapped = true;
            Text.PaddingLeft = 231;
            Text.Width.Set(681, 0);
            Text.Height.Set(237, 0);
            Text.Activate();
            panel.Append(Text);
        }
    }
}
