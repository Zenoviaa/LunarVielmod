using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.XixianFlaskSystem.UI
{
    internal class XixianFlaskUI : UIPanel
    {
        private UIPanel _panel;

        public XixianFlaskSlot slot;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 32;
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

            slot = new XixianFlaskSlot();
            _panel.Append(slot);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }
    }
}
