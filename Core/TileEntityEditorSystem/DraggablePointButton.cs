using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.GameInput;
using Terraria.ID;
using Terraria.UI;

namespace Stellamod.Core.TileEntityEditorSystem
{
    internal class DraggablePointButton : UIPanel
    {
        public DraggablePointButton(string pointName, DraggablePointField draggablePoint)
        {
            Width.Pixels = 256;
            Height.Pixels = 36;
            BackgroundColor = Color.Blue * 0.5f;
            BorderColor = Color.Transparent;
            DraggablePoint = draggablePoint;
            Text = new UIText(pointName);
            Text.HAlign = 0.5f;
            OnLeftClick += OnButtonClick;
        }

        public UIText Text { get; set; }
        public DraggablePointField DraggablePoint { get; set; }
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
            if (DraggablePoint.Point == Point.Zero)
            {
                //So it's not offscreen
                DraggablePoint.X = (int)Main.LocalPlayer.position.X / 16;
                DraggablePoint.Y = (int)(Main.LocalPlayer.position.Y - 64) / 16;
            }

            if (TileEntitySelector.DraggablePoint == DraggablePoint)
            {
                TileEntitySelector.DraggablePoint = null;
            }
            else
            {
                TileEntitySelector.DraggablePoint = DraggablePoint;
            }
            SoundEngine.PlaySound(SoundID.MenuTick);
        }
    }
}
