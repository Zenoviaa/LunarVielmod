using Microsoft.Xna.Framework;
using Microsoft.Xna.Framework.Graphics;
using ReLogic.Content;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.UI.CauldronSystem
{
    public class CauldronUI : UIPanel
    {
        private UIPanel _panel;
        private UIImage _background;
        private UIImage _bigPot;
        public CauldronMoldSlot moldSlot;
        public CauldronMaterialSlot materialSlot;
        public CauldronPot cauldronPot;
        public CauldronMoldSlot moldSlot2;

        public const int width = 480;
        public const int height = 155;

        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2);
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2) - 32;

        public CauldronUI()
        {
            Asset<Texture2D> backgroundTexture = ModContent.Request<Texture2D>("Stellamod/UI/CauldronSystem/CauldronBackground");
            Asset<Texture2D> bigPotTexture = ModContent.Request<Texture2D>("Stellamod/UI/CauldronSystem/BigCauldronPot");
            _background = new UIImage(backgroundTexture);
            _bigPot = new UIImage(bigPotTexture);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 248;
            Height.Pixels = 100;
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
            _panel.Append(_background);

            moldSlot = new CauldronMoldSlot();
            _panel.Append(moldSlot);

            moldSlot2 = new CauldronMoldSlot();
            _panel.Append(moldSlot2);

            materialSlot = new CauldronMaterialSlot();
            _panel.Append(materialSlot);

            cauldronPot = new CauldronPot();
            _panel.Append(cauldronPot);

            _panel.Append(_bigPot);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);
            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            moldSlot.Left.Pixels = 0;
            moldSlot.Top.Pixels = Height.Pixels - 32;

            moldSlot2.Left.Pixels = Width.Pixels - 32;
            moldSlot2.Top.Pixels = Height.Pixels - 32;

            materialSlot.Left.Pixels = (Width.Pixels / 2) - 68 / 2;
            materialSlot.Top.Pixels = 48;

            cauldronPot.Left.Pixels = moldSlot.Left.Pixels + 64;
            cauldronPot.Top.Pixels = moldSlot.Top.Pixels + 64;

            _bigPot.Left.Pixels = (Width.Pixels / 2) - (68);
            _bigPot.Top.Pixels = -32;
        }
    }
}
