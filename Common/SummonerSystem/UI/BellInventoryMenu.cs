using log4net.Filter;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Mono.Cecil;
using ReLogic.Content;
using Stellamod.Common.ItemBrowser;
using Stellamod.Helpers;
using Stellamod.UI;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.Common.SummonerSystem.UI
{
    public class BellInventoryMenu : UIElement
    {
        private UIPanel _panel;
        private UIScrollbar _scrollbar;
        private UIImage _background;
        private UIList _uiList;
        private BellBrowserView _view;
        public BellInventoryMenu()
        {
            Asset<Texture2D> backgroundTexture =
                ModContent.Request<Texture2D>(this.GetType().DirectoryHere() + "/BellInventoryPanel");
            _background = new UIImage(backgroundTexture);
            _panel = new UIPanel();
            _scrollbar = new FancyScrollbar();
            _uiList = new UIList();

            _view = new BellBrowserView();
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

            _view.ElementsPerRow = 2;
            _view.Width.Pixels = 80;
            _view.Height.Pixels = Height.Pixels;
            _panel.Append(_view);

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

        public override void OnActivate()
        {
            base.OnActivate();
            Item[] items = ItemHelper.BellMinions.ToArray();
            Item[] cloneArr = new Item[items.Length];
            for(int i = 0; i < cloneArr.Length; i++)
            {
                cloneArr[i] = new Item(items[i].type);
            }
            _view.SetCollection(cloneArr);
        }


        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            _panel.Height.Pixels = _view.Height.Pixels + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);

            _scrollbar.Height.Set(Height.Pixels * progress, 0);
            float scrollRatio = _scrollbar.ViewPosition;

            _view.Left.Pixels = 6;
            _view.Top.Pixels = 8;
            _view.ViewPosition = scrollRatio;

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
