using Terraria;
using Terraria.GameContent.UI.Elements;

namespace Stellamod.Common.BossBannerSystem
{
    /// <summary>
    /// Base class for the right side page of the collection book
    /// </summary>
    public abstract class RightPageUI : UIPanel
    {
        public const int width = 480;
        public const int height = 155;

        public int RelativeLeft => Main.screenWidth / 2 - width / 2 + 280;
        public int RelativeTop => Main.screenHeight / 2 - height / 2 - 232;
        public int GetPageWidth()
        {
            return 320;
        }

        public int GetPageHeight()
        {
            return 444;
        }
    }
}
