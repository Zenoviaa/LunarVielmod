using Humanizer;
using Microsoft.Xna.Framework;
using Stellamod.WorldG.StructureManager;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader.UI.Elements;
using Terraria.ModLoader;
using Terraria;

namespace Stellamod.UI.StructureSelector
{
    internal class SaveStructureUI : UIPanel
    {
        private SaveStructureButton _saveStructureButton;
        private UIInputTextField _textBox;

        internal const int width = 480;
        internal const int height = 155;

        internal int RelativeLeft => 470 + 108;
        internal int RelativeTop => 0 + 12;
        public string FileName => _textBox.Text;
        public UIInputTextField Textbox => _textBox;
        public SaveStructureUI() : base()
        {
            _saveStructureButton = new SaveStructureButton();
            _textBox = new UIInputTextField("SavedStruct");
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 420;
            Height.Pixels = 48 * 16;
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;


            _saveStructureButton.Width.Pixels = 16;
            _saveStructureButton.Height.Pixels = 16;
            _saveStructureButton.Left.Pixels = 320;
            _saveStructureButton.Top.Pixels = 8;
            Append(_textBox);
            Append(_saveStructureButton);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;
            _textBox.Width.Pixels = Width.Pixels;
            _textBox.Height.Pixels = 32;
        
        }

    }
}
