using Microsoft.Xna.Framework;
using Terraria;
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
            panel.Width.Set(701, 0);
            panel.Height.Set(200, 0);
            panel.HAlign = 0.5f;

            float screenHeight = Main.screenHeight;
            float screenPercent = (screenHeight - 32) / Main.screenHeight;
            panel.VAlign = screenPercent;

            panel.BackgroundColor = Color.Transparent;
            panel.BorderColor = Color.Transparent;
            Append(panel);

            Box = new DialogueBox();
            panel.Append(Box);

            Portrait = new DialoguePortrait();
            panel.Append(Portrait);

            float textScale = 1f;
            Text = new UIText("Hello world!", textScale); // 1
            Text.IsWrapped = true;
            Text.PaddingTop = 16;
            Text.PaddingLeft = 172;
            Text.Width.Set(681, 0);
            Text.Height.Set(237, 0);
         
            Text.Activate();
            panel.Append(Text);      
        }
    }
}
