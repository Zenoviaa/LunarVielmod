using Microsoft.Xna.Framework;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.CauldronSystem
{
    internal class CauldronUI : UIPanel
    {
        private UIPanel _panel;

        public CauldronMoldSlot moldSlot;
        public CauldronMaterialSlot materialSlot;
        public CauldronPot cauldronPot;

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

            moldSlot = new CauldronMoldSlot();
            _panel.Append(moldSlot);

            materialSlot = new CauldronMaterialSlot();
            _panel.Append(materialSlot);

            cauldronPot = new CauldronPot();
            _panel.Append(cauldronPot);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            materialSlot.Left.Pixels = moldSlot.Left.Pixels + 156;
            materialSlot.Top.Pixels = moldSlot.Top.Pixels;

            cauldronPot.Left.Pixels = moldSlot.Left.Pixels + 64;
            cauldronPot.Top.Pixels = moldSlot.Top.Pixels + 64;
        }
    }
}
