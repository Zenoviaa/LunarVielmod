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
        public CauldronMoldSlot moldSlot;
        public CauldronMaterialSlot materialSlot;
        public CauldronPot cauldronPot;

        public const int width = 480;
        public const int height = 155;

        public int RelativeLeft => Main.screenWidth / 2 - (int)(Width.Pixels / 2);
        public int RelativeTop => Main.screenHeight / 2 - (int)(Height.Pixels / 2) - 32;

        public CauldronUI()
        {
            Asset<Texture2D> backgroundTexture = ModContent.Request<Texture2D>("Stellamod/UI/CauldronSystem/CauldronBackground");
            _background = new UIImage(backgroundTexture);
        }

        public override void OnInitialize()
        {
            base.OnInitialize();
            Width.Pixels = 248;
            Height.Pixels = 208;
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

            materialSlot = new CauldronMaterialSlot();
            _panel.Append(materialSlot);

            cauldronPot = new CauldronPot();
            _panel.Append(cauldronPot);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            Left.Pixels = RelativeLeft;
            Top.Pixels = RelativeTop;

            moldSlot.Left.Pixels = 22;
            moldSlot.Top.Pixels = 16;
            materialSlot.Left.Pixels = moldSlot.Left.Pixels + 156;
            materialSlot.Top.Pixels = moldSlot.Top.Pixels;

            cauldronPot.Left.Pixels = moldSlot.Left.Pixels + 64;
            cauldronPot.Top.Pixels = moldSlot.Top.Pixels + 64;
        }
    }
}
