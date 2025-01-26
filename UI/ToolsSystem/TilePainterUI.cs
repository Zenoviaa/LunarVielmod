
using Microsoft.Xna.Framework;
using System.Linq;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.UI.ToolsSystem
{
    internal class TilePainterUI : UIElement
    {
        private ModTile[] _modTiles;
        private UIGrid _grid;
        private UIList _uiList;
        private UIPanel _panel;
        private UIScrollbar _scrollbar;


        internal int RelativeLeft => 32;
        internal int RelativeTop => Main.screenHeight - (int)Height.Pixels * 2;
        public TilePainterUI() : base()
        {
            _grid = new();
            var modTiles = Stellamod.Instance.GetContent<ModTile>().ToList();
            var modWalls = Stellamod.Instance.GetContent<ModWall>().ToList();

            modTiles.Sort((x, y) => x.Name.CompareTo(y.Name));

            modWalls.Sort((x, y) => x.Name.CompareTo(y.Name));
            for (int i = 0; i < modTiles.Count; i++)
            {
                ModTile modTile = modTiles[i];

                TilePainterSlot slot = new TilePainterSlot(tileType: modTile.Type);
                _grid.Add(slot);
            }


            for (int i = 0; i < modWalls.Count; i++)
            {
                ModWall modWall = modWalls[i];
                TilePainterSlot slot = new TilePainterSlot(wallType: modWall.Type);
                _grid.Add(slot);
            }

            _scrollbar = new FancyScrollbar();
            _uiList = new();
            _panel = new();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 750;
            Height.Pixels = 280;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0.5f;
            _grid.ListPadding = 2f;
            _panel.Append(_grid);

            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.93f);
            _scrollbar.Top.Set(0, 0f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);

            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _panel.Height.Pixels = _grid.GetTotalHeight() + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            _scrollbar.Height.Set(Height.Pixels * progress, 0);


            //Hacky way to get invisible scrollbar when there's no need for it
            if (_panel.Height.Pixels < Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0, 0f);
            }
        }
    }
}
