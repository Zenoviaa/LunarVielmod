using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.UI;

namespace Stellamod.UI.Dialogue
{
    internal class DialoguePanel : UIState
    {
        public UIImage Portrait { get; private set; }
        public UIImage Box { get; private set; }
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

            Box = new UIImage(ModContent.Request<Texture2D>("Stellamod/UI/Dialogue/DialogueBox"));
            Box.Width.Set(701, 0);
            Box.Height.Set(200, 0);
            panel.Append(Box);

            Portrait = new UIImage(ModContent.Request<Texture2D>("Stellamod/UI/Dialogue/ExampleDialoguePortrait"));
            Portrait.HAlign = 0.02f;
            Portrait.VAlign = 1f;
            Portrait.Width.Set(150, 0);
            Portrait.Height.Set(150, 0); 
            panel.Append(Portrait);

            float textScale = 1f;
            Text = new UIText("Hi There :D", textScale); // 1
            Text.IsWrapped = true;
            Text.PaddingTop = 32;
            Text.PaddingLeft = 172;
            Text.Width.Set(681, 0);
            Text.Height.Set(237, 0);
         
            Text.Activate();
            panel.Append(Text);      
        }
    }
}
