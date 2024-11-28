using Microsoft.Xna.Framework;
using Stellamod.Common.QuestSystem;
using System;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.CollectionSystem.Quests
{
    internal class QuestTabUI : UIPanel
    {
        private UIList _uiList;
        private UIPanel _panel;
        private UIGrid _slotGrid;
        private UIScrollbar _scrollbar;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => Main.screenWidth / 2 - width / 2 - 64;
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

            _scrollbar = new UIScrollbar();
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
            QuestPlayer questPlayer = Main.LocalPlayer.GetModPlayer<QuestPlayer>();
            if (_slotGrid != null && _slotGrid.Count != (questPlayer.ActiveQuests.Count + questPlayer.CompletedQuests.Count + 1))
            {
                _slotGrid.Clear();
                foreach (var quest in questPlayer.ActiveQuests)
                {
                    QuestTabSlot slot = new QuestTabSlot();
                    slot.Quest = quest;
                    slot.Activate();
                    _slotGrid.Add(slot);
                }

                UIText separatorText = new UIText("Completed Quests");
                separatorText.Height.Pixels = 24;
                separatorText.Width.Pixels = 48 * 6f;
                separatorText.Top.Pixels = 4;
                separatorText.IsWrapped = false;
                _slotGrid.Add(separatorText);
                foreach (var quest in questPlayer.CompletedQuests)
                {
                    QuestTabSlot slot = new QuestTabSlot();
                    slot.Quest = quest;
                    slot.CompletedQuest = true;
                    slot.Activate();
                    _slotGrid.Add(slot);
                }
                _slotGrid.Recalculate();
            }
      
   
            //We just need to get the number of unique materials since that's how we're sorting things
            base.Recalculate();
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            Glow *= 0.985f;
        }
    }
}
