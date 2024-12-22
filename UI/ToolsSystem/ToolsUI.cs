using Microsoft.Xna.Framework;
using Stellamod.UI.StructureSelector;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.ToolsSystem
{
    internal class ToolsUI : UIPanel
    {
        private float _scale;
        private UIGrid _grid;

        internal const int width = 432;
        internal const int height = 280;

        internal int RelativeLeft => Main.screenWidth / 2;
        internal int RelativeTop => Main.screenHeight - 164;


        public Vector2 Offset;

        public override void OnInitialize()
        {
            base.OnInitialize();
            _scale = 1f;
            Width.Pixels = 48 * 5f;
            Height.Pixels = 48 * 16;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;



            _grid = new UIGrid();
            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0f;
            _grid.ListPadding = 2f;
            Append(_grid);

            FogButton fogButton = new FogButton();
            _grid.Add(fogButton);

            UndoButton undoButton = new UndoButton();
            _grid.Add(undoButton);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft + Offset.X - (Width.Pixels / 2);
            Top.Pixels = RelativeTop + Offset.Y;
        }
    }
}
