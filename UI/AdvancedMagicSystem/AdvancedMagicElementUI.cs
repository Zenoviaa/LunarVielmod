using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;
using Terraria.ModLoader;

namespace Stellamod.UI.AdvancedMagicSystem
{
    internal class AdvancedMagicElementUI : UIPanel
    {
        internal const int width = 480;
        internal const int height = 155;

        public AdvancedMagicElementSlot ElementSlot { get; private set; }
        public AdvancedMagicElementUI() : base()
        {
            ElementSlot = new();
        }

        public override void OnInitialize()
        {
            base.OnInitialize();

            Width.Pixels = width;
            Height.Pixels = height;
            SetPos();
            BackgroundColor = Color.Transparent;
            BorderColor = Color.Transparent;

            Append(ElementSlot);
        }

        public override void Update(GameTime gameTime)
        {
            base.Update(gameTime);

            //Constantly lock the UI in the position regardless of resolution changes
            SetPos();
        }


        private void SetPos()
        {
            var config = ModContent.GetInstance<LunarVeilClientConfig>();
            Vector2 ratioPos = new Vector2(config.EnchantMenuX, config.EnchantMenuY);
            if (ratioPos.X < 0f || ratioPos.X > 100f)
            {
                ratioPos.X = 50;
            }

            if (ratioPos.Y < 0f || ratioPos.Y > 100f)
            {
                ratioPos.Y = 3;
            }

            Vector2 drawPos = ratioPos;
            drawPos.X = (int)(drawPos.X * 0.01f * Main.screenWidth);
            drawPos.Y = (int)(drawPos.Y * 0.01f * Main.screenHeight);

            Left.Pixels = drawPos.X - 64;
            Top.Pixels = drawPos.Y - 64;
        }
    }
}
