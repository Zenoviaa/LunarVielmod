using Humanizer;
using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using Stellamod.UI.CauldronSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.Audio;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ID;
using Terraria.ModLoader.UI.Elements;
using Terraria.ModLoader;
using Stellamod.Items.Weapons.Igniters;

namespace Stellamod.UI.PowderSystem
{
    internal class PowderUI : UIPanel
    {
        private UIGrid _enchantmentsGrid;
        private BaseIgniterCard ActiveCard => ModContent.GetInstance<PowderUISystem>().Card;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 32;
        internal int RelativeTop => 0 + 256;

        public List<PowderSlot> PowderSlots { get; private set; } = new();

        internal PowderUI() : base()
        {

        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = width;
            Height.Pixels = height;
            SetPos();

            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            _enchantmentsGrid = new UIGrid();
            _enchantmentsGrid.Width.Set(0, 1f);
            _enchantmentsGrid.Height.Set(0, 1f);
            _enchantmentsGrid.HAlign = 0f;
            _enchantmentsGrid.ListPadding = 2f;
            Append(_enchantmentsGrid);
        }


        public override void OnDeactivate()
        {
            base.OnDeactivate();
            if (!Main.gameMenu)
            {
                SoundEngine.PlaySound(SoundID.MenuClose);
            }
        }

        public override void Recalculate()
        {
            var card = ActiveCard;
            SetPos();
            _enchantmentsGrid?.Clear();
            PowderSlots.Clear();
            if (card == null)
                return;

            for (int i = 0; i < card.Powders.Count; i++)
            {
                PowderSlot slot = new PowderSlot(card, _enchantmentsGrid._items.Count);
                _enchantmentsGrid.Add(slot);
                PowderSlots.Add(slot);
            }


            _enchantmentsGrid.Recalculate();
            base.Recalculate();
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