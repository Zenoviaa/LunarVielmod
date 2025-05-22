using Microsoft.Xna.Framework;
using Stellamod.Items.MoonlightMagic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.AdvancedMagicSystem
{
    internal class AdvancedMagicItemUI : UIPanel
    {
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _grid;
        private UIScrollbar _scrollbar;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 470 + 108;
        internal int RelativeTop => 0 + 12;

        public AdvancedMagicItemUI() : base()
        {
            _panel = new UIPanel();
            _grid = new UIGrid();
            _scrollbar = new FancyScrollbar();
            _uiList = new UIList();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 5f;
            Height.Pixels = 48 * 16;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

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
            _scrollbar.Left.Set(0, 0.9f);
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

        public override void Recalculate()
        {
            //Recalculate the UI when there is some sort of update
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            PopulateBackpackGrid();
            base.Recalculate();
        }

        private void PopulateBackpackGrid()
        {
            if (Main.gameMenu)
                return;
            var magicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
            bool shouldRePopulate = magicPlayer.Backpack.Count != _grid.Count;
            if (!shouldRePopulate)
                return;

            _grid.Clear();
            for (int i = 0; i < magicPlayer.Backpack.Count; i++)
            {
                AdvancedMagicItemSlot slot = new AdvancedMagicItemSlot(i);
                _grid.Add(slot);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _panel.Height.Pixels = _grid.GetTotalHeight() + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            _scrollbar.Height.Set(Height.Pixels * progress, 0);


            //Hacky way to get invisible scrollbar when there's no need for it
            if(_panel.Height.Pixels < Height.Pixels)
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
