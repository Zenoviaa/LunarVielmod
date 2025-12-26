using Terraria;
using Terraria.GameInput;
using Terraria.UI;

namespace Stellamod.Core.Utilities
{
    public static class InterfaceUtils
    {
        /// <summary>
        /// Checks if the mouse is within the point of the ui element and sets the mouse interface to true
        /// </summary>
        /// <param name="uiElement"></param>
        /// <returns></returns>
        public static bool QuickMouseInteraction(this UIElement uiElement)
        {
            bool contains = uiElement.ContainsPoint(Main.MouseScreen);
            if (contains && !PlayerInput.IgnoreMouseInterface)
            {
                Main.LocalPlayer.mouseInterface = true;
            }
            return contains;
        }
    }
}
