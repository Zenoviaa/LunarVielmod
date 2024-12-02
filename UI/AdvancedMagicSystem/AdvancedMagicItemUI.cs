using Stellamod.Items.MoonlightMagic;
using Microsoft.Xna.Framework;
using System;
using System.Collections.Generic;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.UI;

namespace Stellamod.UI.AdvancedMagicSystem
{
    internal class AdvancedMagicItemUI : UIPanel
    {
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _enchantmentsSlotGrid;
        private UIScrollbar _scrollbar;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 470 + 108;
        internal int RelativeTop => 0 + 12;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48*5f;
            Height.Pixels = 48*16;
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

            _enchantmentsSlotGrid = new UIGrid();
            _enchantmentsSlotGrid.Width.Set(0, 1f);
            _enchantmentsSlotGrid.Height.Set(0, 1f);
            _enchantmentsSlotGrid.HAlign = 0.5f;
            _enchantmentsSlotGrid.ListPadding = 2f;
            _panel.Append(_enchantmentsSlotGrid);

            _scrollbar = new UIScrollbar();
            _scrollbar.Width.Set(20, 0);
            _scrollbar.Height.Set(340, 0);
            _scrollbar.Left.Set(0, 0.9f);
            _scrollbar.Top.Set(0, 0.1f);
         
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
            //Recalculate the UI when there is some sort of update
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _enchantmentsSlotGrid?.Clear();
            if (Main.gameMenu)
                return;

            var magicPlayer = Main.LocalPlayer.GetModPlayer<AdvancedMagicPlayer>();
            for (int i = 0; i < magicPlayer.Backpack.Count; i++)
            {
                AdvancedMagicItemSlot slot = new AdvancedMagicItemSlot();
                slot.index = i;
                slot.Item = magicPlayer.Backpack[i].Clone();

                _enchantmentsSlotGrid.Add(slot);
            }

            _enchantmentsSlotGrid.Recalculate();
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }
    }
}
