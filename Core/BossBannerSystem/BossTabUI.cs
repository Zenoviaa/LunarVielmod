using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.Helpers;
using Stellamod.UI;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.BossBannerSystem
{
    /// <summary>
    /// Represents all of the banners that can be clicked on the left side of the book
    /// </summary>
    public class BossTabUI : UIPanel
    {
        private bool _init;
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _slotGrid;
        private FancyScrollbar _scrollbar;
        private BossPageUI _pageUI;
        public BossTabUI(BossPageUI pageUI) : base()
        {
            _pageUI = pageUI;
        }

        public int RelativeLeft => UIHelper.BookLeftPageX;
        public int RelativeTop => UIHelper.BookLeftPageY;
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 6f;
            Height.Pixels = 48 * 9;
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

            _slotGrid = new UIGrid();
            _slotGrid.Width.Set(0, 1f);
            _slotGrid.Height.Set(0, 1f);
            _slotGrid.ListPadding = 0;

            _panel.Append(_slotGrid);

            _scrollbar = new FancyScrollbar();
            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.98f);
            _scrollbar.Top.Set(0, 0.05f);

            float maxViewSize = 48 * 8f;
            _scrollbar.SetView(0, maxViewSize);
            Append(_scrollbar);


            _uiList = new UIList();
            _uiList.Width.Pixels = Width.Pixels;
            _uiList.Height.Pixels = Height.Pixels;
            _uiList.Add(_panel);
            _uiList.SetScrollbar(_scrollbar);
            Append(_uiList);


        }

        public override void Recalculate()
        {
            base.Recalculate();
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }

        private void InitializeButtons()
        {
            int length = Enum.GetNames<BossBannerType>().Length;
            for (int n = 0; n < length; n++)
            {
                BossBannerType banner = (BossBannerType)n;
                BossBannerButton btn = new BossBannerButton(_pageUI, banner);
                btn.Activate();
                _slotGrid.Add(btn);
            }
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            if (!_init)
            {
                InitializeButtons();
                _init = true;
            }

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            UIHelper.SizePanelandScrollbar(_scrollbar, _panel, Height.Pixels, _slotGrid.GetTotalHeight());
        }

        protected override void DrawSelf(SpriteBatch spriteBatch)
        {
            base.DrawSelf(spriteBatch);

        }
    }
}
