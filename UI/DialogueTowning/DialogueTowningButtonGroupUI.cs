using Microsoft.Xna.Framework;
using Stellamod.Helpers;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.DialogueTowning
{
    public class DialogueTowningButtonGroupUI : UIPanel
    {
        private UIGrid _buttonsGrid;
        internal int RelativeLeft => Main.screenWidth / 2;
        internal int RelativeTop => Main.screenHeight - 380;
        internal Vector2 DrawPos => new Vector2(Left.Pixels, Top.Pixels);

        public Vector2 Offset { get; set; }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 428 * 3;
            Height.Pixels = 128;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _buttonsGrid = new UIGrid();
            _buttonsGrid.Width.Set(0, 1f);
            _buttonsGrid.Height.Set(0, 1f);
            _buttonsGrid.HAlign = 0.5f;
            _buttonsGrid.ListPadding = 2f;
            Append(_buttonsGrid);
        }

        public override void Recalculate()
        {
            if (_buttonsGrid != null)
            {
                _buttonsGrid.Recalculate();
            }

            base.Recalculate();
        }

        public void ClearButtons()
        {
            _buttonsGrid.Clear();
            _buttonsGrid.Recalculate();
        }

        public void AddButton(string text, Action btn)
        {
            DialogueTowningButtonUI button = new DialogueTowningButtonUI();
            button.OnClickEvent = btn;
            button.RealText = LangText.TownDialogue(text);
            _buttonsGrid.Add(button);
        }

        public override void Update(GameTime gameTime)
        {
            Width.Pixels = 214 * (_buttonsGrid.Count) + 32;

            Height.Pixels = 100;
            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft - Width.Pixels / 2;
            Top.Pixels = RelativeTop;
            Left.Pixels += Offset.X;
            Top.Pixels += Offset.Y;
        }
    }
}
