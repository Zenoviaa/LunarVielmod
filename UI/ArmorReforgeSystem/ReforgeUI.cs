using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.UI.ArmorReforgeSystem
{
    internal class ReforgeUI : UIPanel
    {
        private UIPanel _panel;

        public ReforgeSlot reforgeSlot;
        public ReforgeButton reforgeButton;
        public ReforgePearl pearl;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 48;
        internal int RelativeTop => 0 + 256;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 5f;
            Height.Pixels = 48 * 16;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _panel = new UIPanel();
            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            reforgeSlot = new ReforgeSlot();
            _panel.Append(reforgeSlot);

            reforgeButton = new ReforgeButton();
            _panel.Append(reforgeButton);

            pearl = new ReforgePearl();
            _panel.Append(pearl);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            reforgeSlot.Left.Pixels = reforgeButton.Left.Pixels + 32;
            reforgeSlot.Top.Pixels = reforgeButton.Top.Pixels + 24;

            pearl.Left.Pixels = reforgeSlot.Left.Pixels + 82;
            pearl.Top.Pixels = reforgeSlot.Top.Pixels + 48;
        }
    }
}
