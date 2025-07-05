using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal class SaveTileEntityButton : UIPanel
    {
        public SaveTileEntityButton()
        {
            Width.Pixels = 256;
            Height.Pixels = 36;
            BackgroundColor = Color.Blue * 0.5f;
            BorderColor = Color.Transparent;
            Text = new UIText("Save");
            Text.HAlign = 0.5f;
            OnLeftClick += OnButtonClick;
        }

        public UIText Text { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Append(Text);
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
                BackgroundColor = Color.Lerp(BackgroundColor, Color.Yellow * 0.5f, 0.1f);
            }
            else
            {
                BackgroundColor = Color.Lerp(BackgroundColor, Color.Blue * 0.5f, 0.1f);
            }
        }

        private void OnButtonClick(UIMouseEvent evt, UIElement listeningElement)
        {
            TileEntitySelector.SaveNClose = true;
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
}
