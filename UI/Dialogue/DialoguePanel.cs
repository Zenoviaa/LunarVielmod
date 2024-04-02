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
        public UIPanel UIPanel { get; private set; }
        public UIImage Portrait { get; private set; }
        public UIImage Box { get; private set; }
        public UIText Text { get; private set; }
        public float PresetVAlign { get; private set; }

        public override void OnInitialize()
        {
            UIPanel = new UIPanel();
            UIPanel.Width.Set(701, 0);
            UIPanel.Height.Set(200, 0);
            UIPanel.HAlign = 0.5f;
            PresetVAlign = UIPanel.VAlign = 0.9f;

            UIPanel.BackgroundColor = Color.Transparent;
            UIPanel.BorderColor = Color.Transparent;
            Append(UIPanel);

            Box = new UIImage(ModContent.Request<Texture2D>("Stellamod/UI/Dialogue/DialogueBox"));
            Box.Color = Color.White * 0.66f;
            Box.Width.Set(701, 0);
            Box.Height.Set(200, 0);
            UIPanel.Append(Box);

            Portrait = new UIImage(ModContent.Request<Texture2D>("Stellamod/UI/Dialogue/ExampleDialoguePortrait"));
            Portrait.Top.Set(25, 0);
            Portrait.Left.Set(16, 0);
            Portrait.PaddingRight = (701 / 2) - 32;
            Portrait.PaddingTop = 25;
            Portrait.Width.Set(150, 0);
            Portrait.Height.Set(150, 0);
            UIPanel.Append(Portrait);

            float textScale = 1f;
            Text = new UIText("Hi There :D", textScale); // 1
            Text.IsWrapped = true;
            Text.PaddingTop = 32;
            Text.PaddingLeft = 172;
            Text.Width.Set(681, 0);
            Text.Height.Set(237, 0);
         
            Text.Activate();
            UIPanel.Append(Text);      
        }
    }
}
