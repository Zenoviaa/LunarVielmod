using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.Map.UI
{
    internal class MapIconUI : UIPanel
    {
        private UIPanel _panel;
        public MapIcon mapIcon;


        internal const int width = 432;
        internal const int height = 280;

        internal int RelativeLeft => 444;
        internal int RelativeTop => 312;

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

            mapIcon = new MapIcon();
            _panel.Append(mapIcon);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Left.Pixels += 80;
            Top.Pixels = RelativeTop;

            if (Main.LocalPlayer.chest != -1 || Main.LocalPlayer.talkNPC != -1)
            {
                Top.Pixels += 154;
            }

        }
    }
}
