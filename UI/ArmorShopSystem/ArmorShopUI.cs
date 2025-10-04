using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorShop;
using System.Text.RegularExpressions;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.ArmorShopSystem
{
    public class ArmorShopOption : UIPanel
    {
        private readonly ArmorShopSet _set;
        private ArmorShopCost _cost;
        private ArmorShopSlot _lSlot;
        private ArmorShopSlot _bSlot;
        private ArmorShopSlot _hSlot;
        private BuyArmorButton _buyArmorButton;
        public ArmorShopOption(ArmorShopSet set)
        {
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
            Width.Pixels = 384;
            Height.Pixels = 32;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;
            Append(_cost);
            Append(_lSlot);
            Append(_bSlot);
            Append(_hSlot);
            Append(_buyArmorButton);
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            const float spacing = 68;

            const float item_spacing = 56;
            _lSlot.Left.Pixels = _cost.Left.Pixels + spacing;
            _bSlot.Left.Pixels = _lSlot.Left.Pixels + item_spacing; 
            _hSlot.Left.Pixels = _bSlot.Left.Pixels + item_spacing;
            _buyArmorButton.Left.Pixels = _hSlot.Left.Pixels + spacing;
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

        internal int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2);
        internal int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2);
        public float Glow { get; set; }
        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 8;
            Height.Pixels = 256;
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
            foreach (var set in groups.Armors)
            {
                ArmorShopOption option = new ArmorShopOption(set);
             //   option.Activate();
                _slotGrid.Add(option);

            }

            _slotGrid.Recalculate();
            base.Recalculate();
        }
        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            _slotGrid.ListPadding = 32;
            _panel.Height.Pixels = _slotGrid.GetTotalHeight();

            float progress = _panel.Height.Pixels / Height.Pixels;
            progress = MathHelper.Clamp(progress, 0f, 1f);
            _scrollbar.Height.Set(Height.Pixels * progress, 0);
            IgnoresMouseInteraction = false;

            //Hacky way to get invisible scrollbar when there's no need for it
            if (_panel.Height.Pixels < Height.Pixels)
            {
                _scrollbar.Top.Set(500000, 0f);
            }
            else
            {
                _scrollbar.Top.Set(0, 0f);
            }
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            Glow *= 0.985f;
        }
    }
}
