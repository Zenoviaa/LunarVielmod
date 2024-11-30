using Microsoft.Xna.Framework;
using Stellamod.Common.ArmorShop;
using Stellamod.Common.QuestSystem;
using Stellamod.UI.CollectionSystem.Quests;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.ArmorShopSystem
{
    internal class ArmorShopUI : UIPanel
    {
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _slotGrid;
        private FancyScrollbar _scrollbar;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2 + 76;
        internal int RelativeTop => Main.screenHeight / 2 - height / 2 - 196;
        public float Glow { get; set; }
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
            _slotGrid.ListPadding = 2f;

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

        public override void Recalculate()
        {

            //Recalculate the UI when there is some sort of update
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            if (Main.gameMenu)
                return;
            ArmorShopGroups groups = ModContent.GetInstance<ArmorShopGroups>();
            if (_slotGrid == null)
                return;
            if (_slotGrid.Count == 0)
            {
                _slotGrid.Clear();
                foreach(var set in groups.Armors)
                {
                    ArmorShopCost cost = new ArmorShopCost();
                    cost.Item = set.material;
                    cost.armorSet = set;
                    cost.Activate();
                    _slotGrid.Add(cost);
        
                    ArmorShopSlot lSlot = new ArmorShopSlot();
                    lSlot.Item = set.legs[0];
                    lSlot.Activate();
                    _slotGrid.Add(lSlot);

                    ArmorShopSlot bSlot = new ArmorShopSlot();
                    bSlot.Item = set.bodies[0];
                    bSlot.Activate();
                    _slotGrid.Add(bSlot);

                    ArmorShopSlot hSlot = new ArmorShopSlot();
                    hSlot.Item = set.heads[0];
                    hSlot.Activate();
                    _slotGrid.Add(hSlot);

                    BuyArmorButton buyArmorButton = new BuyArmorButton();
                    buyArmorButton.armorSet = set;
                    buyArmorButton.Activate();
                    _slotGrid.Add(buyArmorButton);
                }

                _slotGrid.Recalculate();
            }


            //We just need to get the number of unique materials since that's how we're sorting things
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            Width.Pixels = 48 * 7;
        
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            Glow *= 0.985f;
        }
    }
}
