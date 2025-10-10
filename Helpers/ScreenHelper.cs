using Terraria;
namespace Stellamod.Helpers
{
    public static class ScreenHelper
    {
        public static int TrueScreenWidth => Main.graphics.GraphicsDevice.Viewport.Width;
        public static int TrueScreenHeight => Main.graphics.GraphicsDevice.Viewport.Height;
    }
}
