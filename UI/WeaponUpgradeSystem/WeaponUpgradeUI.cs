using Microsoft.Xna.Framework;
using Stellamod.UI.StructureSelector;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.UI.WeaponUpgradeSystem
{
    internal class WeaponUpgradeUI : UIPanel
    {
        private UIPanel _panel;

        public UpgradeBackground upgradeBackground;
        public WeaponUpgradeSlot reforgeSlot;
        public UpgradeButton reforgeButton;
        public MaterialToUse pearl;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 48;
        internal int RelativeTop => 0 + 256;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 328;
            Height.Pixels = 164;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            upgradeBackground = new UpgradeBackground();
            Append(upgradeBackground);

            _panel = new UIPanel();
            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            reforgeSlot = new WeaponUpgradeSlot();
            _panel.Append(reforgeSlot);

            reforgeButton = new UpgradeButton();
            _panel.Append(reforgeButton);

            pearl = new MaterialToUse();
            _panel.Append(pearl);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            reforgeSlot.Left.Set(0, 0.15f);
            reforgeSlot.Top.Set(0, 0f);

            reforgeButton.Left.Set(0, 0f);
            reforgeButton.Top.Set(0, 0f);

            upgradeBackground.Left.Set(0, 0.1f);
            upgradeBackground.Top.Set(0, 0.2f);
           // reforgeSlot.Left.Pixels = reforgeButton.Left.Pixels + 32;
           // reforgeSlot.Top.Pixels = reforgeButton.Top.Pixels + 24;

            // pearl.Left.Pixels = reforgeSlot.Left.Pixels + 82;
            //pearl.Top.Pixels = reforgeSlot.Top.Pixels + 48;
        }
    }
}
