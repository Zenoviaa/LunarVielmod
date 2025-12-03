using Terraria.GameContent.UI.Elements;

namespace Stellamod.Core.BossBannerSystem
{
    /// <summary>
    /// Base class for the right side page of the collection book
    /// </summary>
    public abstract class RightPageUI : UIPanel
    {
        public int RelativeLeft => 0;
        public int RelativeTop => 0;
        public int GetPageWidth()
        {
            return 200;
        }

        public int GetPageHeight()
        {
            return 250;
        }
    }
}
