using Microsoft.Xna.Framework;
using Stellamod.Common.ScorpionMountSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.UI.GunHolsterSystem
{
    internal class ScorpionHolsterUI : UIPanel
    {
        private UIGrid _grid;
        private UIPanel _panel;
        private BaseScorpionItem ScorpionItem => ModContent.GetInstance<ScorpionHolsterUISystem>().scorpionItem;

        public GunHolsterLeftSlot leftSlot;
        public GunHolsterRightSlot rightSlot;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 32;
        internal int RelativeTop => 0 + 256;

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 48 * 5f;
            Height.Pixels = 48 * 16;
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

            _grid = new UIGrid();
            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0.5f;
            _grid.ListPadding = 2f;
            Append(_grid);
        }

        public override void Recalculate()
        {
            var scorpionItem = ScorpionItem;
            _grid?.Clear();
            if (scorpionItem == null)
                return;

            for (int i = 0; i < scorpionItem.leftHandedGuns.Count; i++)
            {
                var slot = new GunHolsterLeftSlot();
                slot.scorpionIndex = i;
                slot.Activate();
                _grid.Add(slot);
            }

            for (int i = 0; i < scorpionItem.rightHandedGuns.Count; i++)
            {
                var slot = new GunHolsterRightSlot();
                slot.scorpionIndex = i;
                slot.Activate();
                _grid.Add(slot);
            }

            _grid.Recalculate();
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
