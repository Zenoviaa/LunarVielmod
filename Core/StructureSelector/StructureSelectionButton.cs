using Microsoft.Xna.Framework;
using Stellamod.Core.Helpers;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace Stellamod.Core.StructureSelector
{
    internal class StructureSelectionButton : UIElement
    {
        private bool IsSelected => Structurizer.SelectedStructure == _structurePath;
        private readonly string _structurePath;
        private readonly UIText _text;
        private readonly UIPanel _panel;
        internal const int width = 420;
        internal const int height = 28;

        public StructureSelectionButton(string structurePath)
        {
            _structurePath = structurePath.Replace(".str", "");
            _text = new UIText(structurePath);
            _panel = new UIPanel();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = width;
            Height.Pixels = height;
            OnLeftClick += OnButtonClick;



            _panel.Width.Pixels = Width.Pixels - 128;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            Append(_text);
            Append(_panel);
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            SoundEngine.PlaySound(SoundID.MenuTick);
            if (Structurizer.SelectedStructure == _structurePath)
            {
                Structurizer.SelectedStructure = string.Empty;
                Main.NewText($"Deselected {Structurizer.SelectedStructure}");
            }
            else
            {
                Structurizer.SelectedStructure = _structurePath;

                Main.NewText($"Selected {Structurizer.SelectedStructure}");
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            bool contains = ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }

            if (IsMouseHovering)
            {
                _panel.BackgroundColor = Color.Lerp(_panel.BackgroundColor, Color.Goldenrod * 0.5f, 0.1f);

            }
            else
            {
                if (IsSelected)
                {
                    _panel.BackgroundColor = Color.Lerp(_panel.BackgroundColor, Color.Green * 0.2f, 0.1f);
                }
                else
                {
                    _panel.BackgroundColor = Color.Lerp(_panel.BackgroundColor, Color.Transparent, 0.1f);
                }
            }


        }
    }
}
