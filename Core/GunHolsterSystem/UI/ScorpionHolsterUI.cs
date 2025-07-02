using Microsoft.Xna.Framework;
using Stellamod.Core.ScorpionMountSystem;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.GunHolsterSystem.UI
{
    internal class ScorpionHolsterUI : UIPanel
    {
        private UIGrid _grid;
        private UIPanel _panel;

        public GunHolsterLeftSlot leftSlot;
        public GunHolsterRightSlot rightSlot;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 32;
        internal int RelativeTop => 0 + 256;

        public ScorpionHolsterUI() : base()
        {
            _panel = new UIPanel();
            _grid = new UIGrid();
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
            Append(_grid);
        }

        public void OpenUI(BaseScorpionItem scorpionItem)
        {
            _grid.Clear();
            for (int i = 0; i < scorpionItem.leftHandedGuns.Count; i++)
            {
                var slot = new GunHolsterLeftSlot();
                slot.scorpionIndex = i;
                slot.OpenUI(scorpionItem);
                slot.Activate();
                _grid.Add(slot);
            }

            for (int i = 0; i < scorpionItem.rightHandedGuns.Count; i++)
            {
                var slot = new GunHolsterRightSlot();
                slot.scorpionIndex = i;
                slot.OpenUI(scorpionItem);
                slot.Activate();
                _grid.Add(slot);
            }
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
