﻿using Humanizer;
using Microsoft.Xna.Framework;
using Stellamod.UI.CauldronSystem;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.UI.CellConverterSystem
{
    internal class ConverterUI : UIPanel
    {
        private UIPanel _panel;

        public ConvertSlot convertSlot;
        public ConverterCrystal convertCrystal;

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

            convertSlot = new ConvertSlot();
            _panel.Append(convertSlot);

            convertCrystal = new ConverterCrystal();
            _panel.Append(convertCrystal);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            convertSlot.Left.Pixels = convertCrystal.Left.Pixels + 80;
            convertSlot.Top.Pixels = convertCrystal.Top.Pixels + 24;
        }
    }
}
