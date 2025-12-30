using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Stellamod.Common.XixianFlaskSystem;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.Common.XixianFlaskSystem.UI
{
    public class InsourceInventoryMenu : UIElement
    {
        private UIGrid _grid;
        private UIPanel _panel;
        private UIScrollbar _scrollbar;
        private UIImage _background;
        private UIList _uiList;
        public InsourceInventoryMenu()
        {
            Asset<Texture2D> backgroundTexture =
                ModContent.Request<Texture2D>(XixianFlaskUISystem.RootTexturePath + "InsourceInventoryPanel");
            _background = new UIImage(backgroundTexture);
            _panel = new UIPanel();
            _grid = new UIGrid();
            _scrollbar = new FancyScrollbar();
            _uiList = new UIList();
        }


        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 118;
            Height.Pixels = 216;
            Append(_background);

            _panel.Width.Pixels = Width.Pixels;
            _panel.Height.Pixels = Height.Pixels;
            _panel.BackgroundColor = Color.Transparent;
            _panel.BorderColor = Color.Transparent;
            Append(_panel);

            _grid.Width = Width;
            _grid.Height = Height;
            _grid.HAlign = 0;
            _grid.VAlign = 0;
            _grid.ListPadding = 24;
            _grid.PaddingLeft = 1;
            _grid.PaddingTop = 0;
            _grid.PaddingRight = 0;
            _grid.PaddingBottom = 0;
            _panel.Append(_grid);

            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 1);
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
        public void SetInsources()
        {
            _grid.Clear();

            IEnumerable<ModItem> insources = ModContent.GetContent<InsourceItem>();
            foreach (var insource in insources)
            {
                Item template = insource.Item;
                Item newItem = new Item(insource.Type);
                var slot = new InsourceInventorySlot(newItem);
                _grid.Add(slot);
            }

            _grid.Recalculate();
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Width.Pixels = 118;
            Height.Pixels = 216;
            _panel.Height.Pixels = _grid.GetTotalHeight();
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
