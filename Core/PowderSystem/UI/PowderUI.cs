using Microsoft.Xna.Framework;
using Terraria;
using Terraria.Audio;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI.Elements;

namespace Stellamod.Core.PowderSystem.UI
{
    internal class PowderUI : UIPanel
    {
        private UIGrid _grid;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 32;
        internal int RelativeTop => 0 + 256;

        internal PowderUI() : base()
        {
            _grid = new UIGrid();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = width;
            Height.Pixels = height;
            SetPos();

            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;


            _grid.Width.Set(0, 1f);
            _grid.Height.Set(0, 1f);
            _grid.HAlign = 0f;
            _grid.ListPadding = 2f;
            Append(_grid);
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }


        public void OpenUI(BaseIgniterCard card)
        {
            _grid.Clear();
            for (int i = 0; i < card.Powders.Count; i++)
            {
                PowderSlot slot = new PowderSlot(card, _grid._items.Count);
                _grid.Add(slot);
            }
        }

        private void SetPos()
        {
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            SetPos();
        }
    }
}