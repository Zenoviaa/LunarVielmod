using Terraria;

namespace Stellamod.Common.Helpers
{
    internal static class ScreenHelper
    {
        public static int TrueScreenWidth => Main.graphics.GraphicsDevice.Viewport.Width;
        public static int TrueScreenHeight => Main.graphics.GraphicsDevice.Viewport.Height;
    }
}
