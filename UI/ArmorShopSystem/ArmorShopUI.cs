using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorShop;
using Stellamod.Items.Shrines.GovheilNAlca;
using System;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.UI.ArmorShopSystem
{
    public class ArmorShopOption : UIElement
    {
        private readonly ArmorShopSet _set;
        private int _index;
        private ArmorShopCost _cost;
        private ArmorShopSlot _lSlot;
        private ArmorShopSlot _bSlot;
        private ArmorShopSlot _hSlot;
        private BuyArmorButton _buyArmorButton;
        public ArmorShopOption(ArmorShopSet set, int index)
        {
            _index = index;
            _set = set;
            _cost = new ArmorShopCost();
            _cost.Item = set.material;
            _cost.armorSet = set;

            _lSlot = new ArmorShopSlot();
            _lSlot.Item = set.legs[0];

            _bSlot = new ArmorShopSlot();
            _bSlot.Item = set.bodies[0];

            _hSlot = new ArmorShopSlot();
            _hSlot.Item = set.heads[0];

            _buyArmorButton = new BuyArmorButton();
            _buyArmorButton.armorSet = set;
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 512;
            Height.Pixels = 32;

            Append(_cost);
            Append(_lSlot);
            Append(_bSlot);
            Append(_hSlot);
            Append(_buyArmorButton);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            const float spacing = 48;

            const float item_spacing = 60;

            _cost.Left.Pixels = 16;
            _cost.Top.Pixels = 24;
            _lSlot.Left.Pixels = _cost.Left.Pixels + spacing;
            _bSlot.Left.Pixels = _lSlot.Left.Pixels + item_spacing; 
            _hSlot.Left.Pixels = _bSlot.Left.Pixels + item_spacing;
            _buyArmorButton.Left.Pixels = _hSlot.Left.Pixels + item_spacing;
            _buyArmorButton.Top.Pixels = 9;
        }

        public override int CompareTo(object obj)
        {
            if(obj is ArmorShopOption ui)
            {
                return _index.CompareTo(ui._index);
            }
            return base.CompareTo(obj);
        }
    }
    internal class ArmorShopUI : UIPanel
    {
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _slotGrid;
        private FancyScrollbar _scrollbar;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2) - 200;
        internal int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2);
        public float Glow { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 8;
            Height.Pixels = 384;
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
            _panel.Append(_slotGrid);

            _scrollbar = new FancyScrollbar();
            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.9f);
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
            Glow = 1;
        }
        public void Rebuild()
        {
            ArmorShopGroups groups = ModContent.GetInstance<ArmorShopGroups>();
            _slotGrid.Clear();
            int index = 0;
            foreach (var set in groups.Armors)
            {
                ArmorShopOption option = new ArmorShopOption(set, index);
             //   option.Activate();
                _slotGrid.Add(option);
                index++;

            }

            _slotGrid.Recalculate();
            base.Recalculate();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            _panel.Height.Pixels = _slotGrid.GetTotalHeight() + 32;
            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            _scrollbar.Height.Set(Height.Pixels * progress, 0);
            _slotGrid.ListPadding = 20;

            //Hacky way to get invisible scrollbar when there's no need for it
            if (_panel.Height.Pixels < Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0, 0f);
            }
            Glow *= 0.985f;
        }
    }
}
