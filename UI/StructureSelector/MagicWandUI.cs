using Microsoft.Xna.Framework;
using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.UI.StructureSelector
{
    internal class MagicWandUI : UIPanel
    {

        internal const int width = 480;
        internal const int height = 155;
        internal int RelativeLeft => Main.screenWidth / 2 - width / 2;
        internal int RelativeTop => Main.screenHeight / 2 - height / 2;
        public MagicWandSlot TargetTileSlot;
        public MagicWandSlot SwapTileSlot;
        public MagicWandButton MagicWandButton;
        public MagicWandUI() : base()
        {
            TargetTileSlot = new MagicWandSlot();
            SwapTileSlot = new MagicWandSlot();
            MagicWandButton = new MagicWandButton();
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

            TargetTileSlot.Left.Set(0, 0.2f);
            TargetTileSlot.Top.Set(0, 0.5f);
            SwapTileSlot.Left.Set(0, 0.8f);
            SwapTileSlot.Top.Set(0, 0.5f);
            MagicWandButton.Left.Set(0, 0.5f);
            MagicWandButton.Top.Set(0, 0.5f);

            Append(TargetTileSlot);
            Append(SwapTileSlot);
            Append(MagicWandButton);
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
